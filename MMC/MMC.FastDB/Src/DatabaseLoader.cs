using System;
using System.Collections.Generic;
using System.IO;

namespace MMC.FastDB
{
    /// <summary>
    /// 负责数据库元数据记录的装载和保存
    /// </summary>
    internal class DatabaseLoader : IDisposable
    {
        /// <summary>
        /// 使用一个引用，确保当前的数据库不会被异常修改。
        /// </summary>
        private readonly FileStream MetaStream;

        private readonly string MetaFileName;

        public DatabaseLoader(string metaFileName)
        {
            System.Diagnostics.Debug.Assert(!string.IsNullOrEmpty(metaFileName));
            System.Diagnostics.Debug.Assert(File.Exists(metaFileName));
            this.MetaStream = new FileStream(metaFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            this.MetaFileName = metaFileName;
        }

        public IList<MetaTableInfo> Load()
        {
            var tables = new List<MetaTableInfo>();
            using (var binaryReader = new BinaryReader(new FileStream(this.MetaFileName, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                var tableCount = binaryReader.ReadInt32();

                for (int i = 0; i < tableCount; i++)
                {
                    var tableGUID = binaryReader.ReadStringMM();
                    var tableName = binaryReader.ReadStringMM();
                    var filePath = binaryReader.ReadStringMM();

                    var fieldCount = binaryReader.ReadInt32();
                    var fields = new List<MetaFieldInfo>();

                    for (int j = 0; j < fieldCount; j++)
                    {
                        var fieldInfo = new MetaFieldInfo();
                        fieldInfo.FieldName = binaryReader.ReadStringMM();
                        fields.Add(fieldInfo);
                    }

                    var table = new MetaTableInfo(fields);
                    table.TableGUID = tableGUID;
                    table.TableName = tableName;
                    table.FilePath = filePath;
                    table.InitializeAccess = false;

                    tables.Add(table);
                }
            }

            return tables;
        }

        public void Save(IEnumerable<MetaTableInfo> tables)
        {
            using (var binaryWriter = new BinaryWriter(new FileStream(this.MetaFileName, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                binaryWriter.BaseStream.Seek(0, SeekOrigin.Begin);
                binaryWriter.Write(0);

                var tableNum = 0;
                foreach (var table in tables)
                {
                    binaryWriter.WriteStringMM(table.TableGUID);
                    binaryWriter.WriteStringMM(table.TableName);
                    binaryWriter.WriteStringMM(table.FilePath);

                    binaryWriter.Write(table.FieldCount);

                    foreach (var fieldInfo in table.FieldInfos)
                    {
                        binaryWriter.WriteStringMM(fieldInfo.FieldName);
                    }

                    tableNum ++;
                }

                binaryWriter.BaseStream.Seek(0, SeekOrigin.Begin);
                binaryWriter.Write(tableNum);
            }
        }

        public void Dispose()
        {
            this.MetaStream.Dispose();
        }
    }
}
