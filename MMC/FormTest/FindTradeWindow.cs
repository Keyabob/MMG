using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FormTest
{
    

    class FindTradeWindow
    {
        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "FindWindowEx", SetLastError = true)]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, string lParam);

        [DllImport("User32.dll")]
        private extern static int GetWindow(IntPtr hWnd, int wCmd);

        [DllImport("user32.dll")]
        private static extern bool GetWindowText(IntPtr hWnd, StringBuilder title, int maxBufSize);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private extern static int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32", EntryPoint = "GetWindowThreadProcessId")]
        private static extern int GetWindowThreadProcessId(IntPtr hwnd, out int pid);

        [DllImport("user32.dll")]
        public static extern int EnumChildWindows(int hWndParent, EnumWindowsProc lpfn, int lParam);

        [DllImport("user32.dll", ExactSpelling = true)]
        private static extern bool EnumChildWindows(IntPtr hwndParent, WNDENUMPROC lpEnumFunc, int lParam); 

        public delegate bool EnumWindowsProc(int hWnd, int lParam);

        private delegate bool WNDENUMPROC(IntPtr hWnd, int lParam);

        private const int WS_VISIBLE = 268435456;//窗体可见
        private const int WS_MINIMIZEBOX = 131072;//有最小化按钮
        private const int WS_MAXIMIZEBOX = 65536;//有最大化按钮
        private const int WS_BORDER = 8388608;//窗体有边框
        private const int GWL_STYLE = (-16);//窗体样式
        private const int GW_HWNDFIRST = 0;
        private const int GW_HWNDNEXT = 2;
        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;

        public static int Find(IntPtr handle) 
        {
            var hwd = GetWindow(handle, GW_HWNDFIRST);
            while (hwd > 0)
            {
                int length = GetWindowTextLength(new IntPtr(hwd));
                StringBuilder stringBuilder = new StringBuilder(2 * length + 1);
                var result = GetWindowText(new IntPtr(hwd), stringBuilder, stringBuilder.Capacity);
                if (result == true)
                {
                    var s = stringBuilder.ToString();
                    /*Encoding utf8 = Encoding.GetEncoding("UTF-8");
                    Encoding gb2312 = Encoding.GetEncoding("gb2312");
                    var encode = Encoding.Convert(utf8, gb2312, utf8.GetBytes(s));
                    s = gb2312.GetString(encode);
                    Console.WriteLine(s);*/
                    if (s.Contains("通达信网上交易"))
                    {
                        // 找到了窗体
                        break;
                    }
                }

                hwd = GetWindow(new IntPtr(hwd), GW_HWNDNEXT);
            }

            return hwd;
        }
    }
}
