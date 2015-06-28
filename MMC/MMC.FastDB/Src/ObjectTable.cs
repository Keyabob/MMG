using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MMC.FastDB
{
    /// <summary>
    /// 数据表对象，每一个数据表对象可以存储某一个实体对象集合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectTable<T>
    {
        internal ObjectTable(TableAccessLock tal, MetaTableInfo metaTableInfo)
        {
            System.Diagnostics.Debug.Assert(tal != null);
            System.Diagnostics.Debug.Assert(metaTableInfo != null);

            //
            // 每一个数据表需要持有一个数据库分配的锁对象，来确保在全局范围内对该表的操作不会导致
            // 多线程错误
            //

            this.TAL = tal;
            this.MetaTableInfo = metaTableInfo;
        }

        private TableAccessLock TAL { get; set; }

        internal MetaTableInfo MetaTableInfo { get; private set; }

        /// <summary>
        /// 获取当前数据表中指定索引位置的数据
        /// </summary>
        /// <param name="index">指定索引位置</param>
        /// <returns>返回指定索引位置的数据对象</returns>
        public T this[int index]
        {
            get { return default(T); }
        }

        /// <summary>
        /// 往表格末尾追加一个新的数据元素
        /// </summary>
        /// <param name="element">指定的新数据元素</param>
        public void Insert(T element)
        {
            
        }

        /// <summary>
        /// 使用一个指定的超时时间对当前数据表加锁
        /// </summary>
        /// <param name="timeout">超时时间</param>
        /// <returns>返回加锁是否成功</returns>
        public bool TryLock(int timeout = int.MaxValue)
        {
            return this.TAL.TryLock(timeout);
        }

        /// <summary>
        /// 释放数据表锁定
        /// </summary>
        public void ReleaseLock()
        {
            this.TAL.ReleaseLock();
        }
    }
}
