using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MMC.FastDB
{
    internal class MetaTableInfo
    {
        private readonly Dictionary<string, MetaFieldInfo> FieldMaps = new Dictionary<string, MetaFieldInfo>();

        private readonly MetaFieldInfo[] MetaFields;

        public MetaTableInfo(IEnumerable<MetaFieldInfo> fields)
        {
            this.MetaFields = fields.ToArray();

            foreach (var fieldInfo in this.MetaFields)
            {
                this.FieldMaps.Add(fieldInfo.FieldName, fieldInfo);
            }
        }

        public int FieldCount
        {
            get { return this.MetaFields.Length; }
        }

        public string TableName { get; set; }

        public string TableGUID { get; set; }

        /// <summary>
        /// 数据表相对路径
        /// </summary>
        public string FilePath { get; set; }

        public bool InitializeAccess { get; set; }

        public FileStream UsingStream { get; set; }

        public MetaFieldInfo GetField(string fieldName)
        {
            return this.FieldMaps[fieldName];
        }

        public IEnumerable<MetaFieldInfo> FieldInfos
        {
            get { return MetaFields; }
        }

    }
}
