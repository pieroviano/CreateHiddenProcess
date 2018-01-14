using System;
using System.Runtime.InteropServices;
using System.Text;
using CreateHiddenProcessLib.CreateWindowUtility.Delegates;

namespace CreateHiddenProcessLib.CreateWindowUtility.Win32
{
    internal static class Win32Interop
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll", EntryPoint = "EnumDesktopWindows",
            ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool EnumDesktopWindows(IntPtr hDesktop,
            EnumDelegate lpEnumCallbackFunction, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "GetClassName", ExactSpelling = false,
            CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetClassName(IntPtr hwnd, StringBuilder lpClassName,
            int nMaxCount);

        [DllImport("user32.dll", EntryPoint = "GetWindowText",
            ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd,
            StringBuilder lpWindowText, int nMaxCount);
    }
}