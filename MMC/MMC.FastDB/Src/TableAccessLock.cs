using System.Threading;

namespace MMC.FastDB
{
    internal class TableAccessLock
    {
        private readonly object _lockObj = new object();

        private Thread onwerThread;

        public bool TryLock(int timeout = int.MaxValue)
        {
            if (Monitor.TryEnter(this._lockObj, timeout))
            {
                this.onwerThread = Thread.CurrentThread;
                return true;
            }

            return false;
        }

        public bool IsLockedByMe
        {
            get { return this.onwerThread == Thread.CurrentThread; }
        }

        public void ReleaseLock()
        {
            if (this.onwerThread == Thread.CurrentThread)
            {
                this.onwerThread = null;
            }

            Monitor.Exit(this._lockObj);
        }
    }
}
