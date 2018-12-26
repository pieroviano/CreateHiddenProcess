using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using CreateHiddenProcessLib.CreateWindowUtility.Delegates;
using CreateHiddenProcessLib.CreateWindowUtility.Model;
using CreateHiddenProcessLib.CreateWindowUtility.Win32;

namespace CreateHiddenProcessLib.Utility
{
    public static class WindowUtility
    {
        public const int MaxTitle = 255;

        public static void EnumerateWindows(Dictionary<IntPtr, WindowHookEventArgs> newWindows)
        {
            EnumDelegate enumfunc = (hWnd, lParam) => EnumWindowsProc(newWindows, hWnd, lParam);
            var hDesktop = IntPtr.Zero; // current desktop
            var success = Win32Interop.EnumDesktopWindows(hDesktop, enumfunc, IntPtr.Zero);

            if (!success)
            {
                // Get the last Win32 error code
                var errorCode = Marshal.GetLastWin32Error();

                var errorMessage = $"EnumDesktopWindows failed with code {errorCode}.";
                throw new Exception(errorMessage);
            }
        }

        public static bool EnumWindowsProc(Dictionary<IntPtr, WindowHookEventArgs> newWindows, IntPtr hWnd, int lParam)
        {
            var tArgument = new WindowHookEventArgs
            {
                Handle = hWnd,
                WindowTitle = GetWindowText(hWnd),
                WindowClass = GetClassName(hWnd)
            };


            newWindows.Add(tArgument.Handle, tArgument);
            return true;
        }

        public static string GetClassName(IntPtr hWnd)
        {
            var title = new StringBuilder(MaxTitle);
            var titleLength = Win32Interop.GetClassName(hWnd, title, title.Capacity + 1);
            title.Length = titleLength;

            return title.ToString();
        }

        /// <summary>
        ///     Returns the caption of a window by given HWND identifier.
        /// </summary>
        public static string GetWindowText(IntPtr hWnd)
        {
            var titleLength = WindowTextUtility.GetWindowTextTimeout(hWnd, 10);

            return titleLength;
        }
    }
}