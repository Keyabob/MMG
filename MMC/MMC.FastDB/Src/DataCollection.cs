using System.Collections;
using System.Collections.Generic;

namespace MMC.FastDB
{
    /// <summary>
    /// 代表一个指定查询的数据集合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataCollection<T> : IReadOnlyCollection<T>
    {
        internal DataCollection(MetaTableInfo metaTableInfo)
        {
            System.Diagnostics.Debug.Assert(metaTableInfo != null);

            this.MetaTableInfo = metaTableInfo;
        }

        internal MetaTableInfo MetaTableInfo { get; private set; }

        private int _count;

        public T this[int index]
        {
            get { return default (T); }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return null;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count
        {
            get { return this._count; }
        }
    }
}
