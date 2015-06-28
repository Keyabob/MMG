using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace MMC.FastDB
{
    internal static class BinaryWriterEx
    {
        public static void WriteStringMM(this BinaryWriter writer, string str)
        {
            if (str == null)
            {
                writer.Write(-1);
            }
            else if (string.IsNullOrEmpty(str))
            {
                writer.Write(0);
            }
            else
            {
                writer.Write(str);
            }
        }

        public static string ReadStringMM(this BinaryReader reader)
        {
            var len = reader.ReadInt32();
            if (len == -1)
            {
                return null;
            }
            else if (len == 0)
            {
                return string.Empty;
            }
            else
            {
                reader.BaseStream.Seek(-4, SeekOrigin.Current);
                return reader.ReadString();
            }
        }
    }
}
