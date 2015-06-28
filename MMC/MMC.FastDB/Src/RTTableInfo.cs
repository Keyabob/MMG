using System.IO;

namespace MMC.FastDB
{
    /// <summary>
    /// 运行时数据表对象,存储运行时状态信息
    /// </summary>
    internal class RTTableInfo
    {
        public bool IsAccessInitialized { get; set; }

        public TableAccessLock AccessLock { get; set; }

        public FileStream TableStream { get; set; }
    }
}
