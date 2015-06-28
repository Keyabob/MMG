using System.Threading;

namespace MMC.FastDB
{
    internal class TableAccessLock
    {
        private readonly object _lockObj = new object();

        public bool TryLock(int timeout)
        {
            return Monitor.TryEnter(this._lockObj, timeout);
        }

        public void ReleaseLock()
        {
            Monitor.Exit(this._lockObj);
        }
    }
}
