using System;
using System.IO;

namespace MMC.FastDB
{
    internal class ConnectionConfiguration
    {
        public string DatabaseMetaFileName { get; private set; }

        public ConnectionConfiguration(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            this.DatabaseMetaFileName = Path.GetFullPath(connectionString);
        }
    }
}
