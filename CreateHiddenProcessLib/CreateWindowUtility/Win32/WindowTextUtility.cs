using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CreateHiddenProcessLib.CreateWindowUtility.Win32
{
    public class WindowTextUtility
    {
        [DllImport("User32.dll", SetLastError = true)]
        public unsafe static extern int SendMessageTimeout(
            IntPtr hWnd,
            uint uMsg,
            uint wParam,
            StringBuilder lParam,
            uint fuFlags,
            uint uTimeout,
            void* lpdwResult);

        const int WM_GETTEXT = 0x000D;
        const int WM_GETTEXTLENGTH = 0x000E;

        public static unsafe string GetWindowTextTimeout(IntPtr hWnd, uint timeout)
        {
            int length;
            if (SendMessageTimeout(hWnd, WM_GETTEXTLENGTH, 0, null, 2, timeout, &length) == 0)
            {
                return null;
            }
            if (length == 0)
            {
                return null;
            }

            StringBuilder sb = new StringBuilder(length + 1);  // leave room for null-terminator
            if (SendMessageTimeout(hWnd, WM_GETTEXT, (uint)sb.Capacity, sb, 2, timeout, null) == 0)
            {
                return null;
            }

            return sb.ToString();
        }
    }
}
