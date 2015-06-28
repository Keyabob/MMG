using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace MMC.FastDB
{
    /// <summary>
    /// 数据库对象，一个数据库可以包含多个表格，每一个数据表是一个用户定义的实体对象，实现上，每一个数据表存储为多个文件集合。
    /// 一个文件集合包含一个数据文件，一个索引池文件。所有无法解析为基础类型的，包括字节数组，字符串等，存储为二进制索引池
    /// 元素，并在数据文件中存储对应位置的索引。并且使用一个元数据文件定义和记录数据表属性信息，以及对应的文件集合信息
    /// </summary>
    public class Database : IDisposable
    {
        private static readonly IList<Type> SupportedTypes = new[]
        {
            typeof (byte), typeof (sbyte), typeof (short), typeof (ushort), typeof (int), typeof (uint), typeof (long),
            typeof (ulong), typeof (float), typeof (double), typeof (string)
        }; 

        /// <summary>
        /// 数据库元数据文件，记录数据库的属性，以及数据库的数据表信息
        /// </summary>
        public string DatabaseMetaFile
        {
            get { return this.Config.DatabaseMetaFileName; }
        }

        private ConnectionConfiguration Config { get; set; }

        private DatabaseLoader Loader { get; set; }

        private Dictionary<string, TableContext> MetaTableMap { get; set; }

        private IList<TableContext> RTTables { get; set; }

        private IEnumerable<MetaTableInfo> MetaTables
        {
            get
            {
                if (this.RTTables != null)
                {
                    foreach (var tableContext in RTTables)
                    {
                        yield return tableContext.MTTable;
                    }
                }
            }
        }

        /// <summary>
        /// 使用指定的连接字符串初始化一个数据库对象
        /// </summary>
        /// <param name="connectionString">指定的连接字符串</param>
        public Database(string connectionString)
        {
            //
            // 解析字符串，确定数据库元数据文件信息，确定数据库属性，以及存在的数据表
            // 由于Configuration会检测连接字符串是否合法，故这里不再检查。
            //

            this.Config = new ConnectionConfiguration(connectionString);

            if (!File.Exists(this.DatabaseMetaFile))
            {
                //
                // 如果不存在，则初始化一个新的数据库对象
                //

                this.InitializeNewDatabase(this.DatabaseMetaFile);

                System.Diagnostics.Debug.Assert(File.Exists(this.DatabaseMetaFile));
            }

            this.RTTables = new List<TableContext>();

            this.Loader = new DatabaseLoader(this.DatabaseMetaFile);
            var tables = this.Loader.Load();

            foreach (var table in tables)
            {
                this.AddNewTable(table);
            }
        }

        private void InitializeNewDatabase(string fileName)
        {
            System.Diagnostics.Debug.Assert(!File.Exists(fileName));

            using (var binaryWriter = new BinaryWriter(File.Create(fileName)))
            {
                //
                // 目前初始化新数据库只需要写入一个表数即可。
                //

                binaryWriter.Write(0);
            }
        }

        public ObjectTable<T> GetTable<T>()
        {
            var type = typeof (T);
            var tableName = type.FullName;

            TableContext table;
            if (!this.MetaTableMap.TryGetValue(tableName, out table))
            {
                var rtTable = this.CreateNewTable(type);

                table = this.AddNewTable(rtTable);
                
                this.Loader.Save(this.MetaTables);
            }

            System.Diagnostics.Debug.Assert(table != null);

            //
            // 加锁,并尝试初始化运行时表数据结构,确定属性都已经加载完成。
            //

            table.RTTable.AccessLock.TryLock();

            try
            {
                if (!table.RTTable.IsAccessInitialized)
                {
                    //
                    // 还没有开始初始化,此时开始初始化表运行时数据结构
                    //

                    this.InitializeTableContext(table);
                    table.RTTable.IsAccessInitialized = true;
                }
            }
            finally
            {
                table.RTTable.AccessLock.ReleaseLock();
            }

            return new ObjectTable<T>(table.RTTable.AccessLock, table.MTTable);
        }

        private void InitializeTableContext(TableContext tableContext)
        {
            //
            // 初始化运行时数据表
            // 1 初始化字段属性反射列表,同时验证字段是否完全匹配
            // 2 初始化文件流,确定当前是否有一个可写入的文件信息,如果不存在
            //   或者前一个文件已经写满,则创建一个新文件,并将文件信息写入配置
            //   列表
            //


        }

        private TableContext AddNewTable(MetaTableInfo table)
        {
            var tableContext = new TableContext();
            tableContext.MTTable = table;
            tableContext.RTTable = new RTTableInfo();
            tableContext.RTTable.IsAccessInitialized = false;
            tableContext.RTTable.TableStream = null;
            tableContext.RTTable.AccessLock = new TableAccessLock();

            this.MetaTableMap.Add(table.TableName, tableContext);
            this.RTTables.Add(tableContext);
            return tableContext;
        }

        private MetaTableInfo CreateNewTable(Type tableType)
        {
            //
            // 根据指定的类型，创建一个新的数据表，并添加到数据表集合，存储到数据库元数据信息文件中。
            //

            // 需要为table 的index 赋值

            var properties = tableType.GetProperties(BindingFlags.DeclaredOnly);

            var fields = new List<MetaFieldInfo>();

            var tableGUID = string.Empty;
            foreach (var propertyInfo in properties)
            {
                //
                // 能够作为实体字段的属性必须是可读，可写，并且支持的基础类型。
                //

                if (propertyInfo.CanRead && propertyInfo.CanWrite && IsTypeSupport(propertyInfo.PropertyType))
                {
                    var field = new MetaFieldInfo
                    {
                        FieldName = propertyInfo.Name, 
                        FieldPropertyInfo = propertyInfo
                    };

                    fields.Add(field);

                    tableGUID += propertyInfo.Name + "-";
                }
            }

            if (fields.Count == 0)
            {
                throw new ArgumentException("Entity type [" + tableType.FullName + "] is not persistable.");
            }

            var table = new MetaTableInfo(fields);
            table.TableName = tableType.FullName;
            table.TableGUID = tableGUID;
            table.Index = this.MetaTables.Count;

            //
            // 配置对象生成完成，开始生成对应的文件信息
            //

            table.FilePath = table.TableName + "_meta.hd";
            var tableMetaFilePath = Path.Combine(Path.GetDirectoryName(this.Config.DatabaseMetaFileName), table.FilePath);

            if (File.Exists(tableMetaFilePath))
            {
                File.Delete(tableMetaFilePath);
            }

            using (var binaryWriter = new BinaryWriter(File.Create(tableMetaFilePath)))
            {
                binaryWriter.Write(0); 
            }

            return table;
        }

        public DataCollection<T> QueryTable<T>()
        {
            var type = typeof(T);
            var tableName = type.FullName;

            MetaTableInfo table;
            if (this.MetaTableMap.TryGetValue(tableName, out table))
            {
                return new DataCollection<T>(table);
            }

            return null;
        }

        public void Dispose()
        {
            this.Loader.Dispose();   
        }

        private static bool IsTypeSupport(Type type)
        {
            return SupportedTypes.Contains(type);
        }
    }
}
