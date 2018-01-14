using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using CreateHiddenProcessLib.CreateWindowUtility.Delegates;
using CreateHiddenProcessLib.CreateWindowUtility.Model;
using CreateHiddenProcessLib.CreateWindowUtility.Win32;

namespace CreateHiddenProcessLib.CreateWindowUtility
{
    /// <summary>
    ///     allows you to get information about the creation and /or the destruction of
    ///     windows
    /// </summary>
    public class WindowHookNet: IDisposable
    {
        private const int MaxTitle = 255;

        private readonly List<WindowHookEventArgs> _eventsToFire = new List<WindowHookEventArgs>();

        private readonly Thread _iThread;

        private readonly Dictionary<IntPtr, WindowHookEventArgs> _newWindowList =
            new Dictionary<IntPtr, WindowHookEventArgs>();

        private readonly Dictionary<IntPtr, WindowHookEventArgs> _oldWindowList =
            new Dictionary<IntPtr, WindowHookEventArgs>();

        private bool _iRun;

        public WindowHookNet()
        {
            ThreadStart tStart = Run;
            _iThread = new Thread(tStart);
        }

        private void EnumerateWindows()
        {
            EnumDelegate enumfunc = EnumWindowsProc;
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

        private bool EnumWindowsProc(IntPtr hWnd, int lParam)
        {
            var tArgument = new WindowHookEventArgs
            {
                Handle = hWnd,
                WindowTitle = GetWindowText(hWnd),
                WindowClass = GetClassName(hWnd)
            };


            _newWindowList.Add(tArgument.Handle, tArgument);
            return true;
        }


        private void FireClosedWindows()
        {
            _eventsToFire.Clear();
            foreach (var tPtr in _oldWindowList.Keys)
            {
                // if the old list contains a key that is not
                // in the new list, that window has been destroyed
                // add it into the fire list
                if (!_newWindowList.ContainsKey(tPtr))
                {
                    _eventsToFire.Add(_oldWindowList[tPtr]);
                }
            }

            // you need to remove / add things later, because
            // you are not allowed to alter the dictionary during iteration
            foreach (var tArg in _eventsToFire)
            {
                _oldWindowList.Remove(tArg.Handle);
                OnWindowDestroyed(tArg);
            }
        }

        private void FireCreatedWindows()
        {
            _eventsToFire.Clear();
            foreach (var tPtr in _newWindowList.Keys)
            {
                // if the new list contains a key that is not
                // in the old list, that window has been created
                // add it into the fire list and to the "old" list
                if (!_oldWindowList.ContainsKey(tPtr))
                {
                    _eventsToFire.Add(_newWindowList[tPtr]);
                }
            }

            // you need to remove / add things later, because
            // you are not allowed to alter the dictionary during iteration
            foreach (var tArg in _eventsToFire)
            {
                _oldWindowList.Add(tArg.Handle, tArg);
                OnWindowCreated(tArg);
            }
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
            var title = new StringBuilder(MaxTitle);
            var titleLength = Win32Interop.GetWindowText(hWnd, title, title.Capacity + 1);
            title.Length = titleLength;

            return title.ToString();
        }

        private event WindowHookDelegate InnerWindowCreated;
        private event WindowHookDelegate InnerWindowDestroyed;

        private void OnWindowCreated(WindowHookEventArgs aArgs)
        {
            InnerWindowCreated?.Invoke(this, aArgs);
        }

        private void OnWindowDestroyed(WindowHookEventArgs aArgs)
        {
            InnerWindowDestroyed?.Invoke(this, aArgs);
        }

        private void Run()
        {
            try
            {
                while (_iRun)
                {
                    _newWindowList.Clear();
                    EnumerateWindows();
                    // if the hook has been freshly installed
                    // simply copy the new list to the "old" one
                    if (0 == _oldWindowList.Count)
                    {
                        foreach (var tKvp in _newWindowList)
                        {
                            _oldWindowList.Add(tKvp.Key, tKvp.Value);
                        }
                    }
                    else // the hook has been running for some time
                    {
                        FireClosedWindows();
                        FireCreatedWindows();
                    }
                    Thread.Sleep(500);
                }

                // if the hook has been uninstalled
                // delete the list of old windows
                // because when it is restarted you do not want to get a whole 
                // lot of events for windows that where already there

                _oldWindowList.Clear();
            }
            catch (Exception aException)
            {
                Console.Out.WriteLine("exception in thread:" + aException);
            }
        }

        public void Shutdown()
        {
            if (_iRun)
            {
                _iRun = false;
            }
        }

        /// <summary>
        ///     register to this event if you want to be informed about
        ///     the creation of a window
        /// </summary>
        public event WindowHookDelegate WindowCreated
        {
            add
            {
                InnerWindowCreated += value;
                if (!_iRun)
                {
                    _iRun = true;
                    _iThread.Start();
                }
            }
            remove
            {
                InnerWindowCreated -= value;

                // if no more listeners for the events
                if (null == InnerWindowCreated &&
                    null == InnerWindowDestroyed)
                {
                    _iRun = false;
                }
            }
        }

        /// <summary>
        ///     register to this event, if you want to be informed about
        ///     the destruction of a window
        /// </summary>
        public event WindowHookDelegate WindowDestroyed
        {
            add
            {
                InnerWindowDestroyed += value;
                if (!_iRun)
                {
                    _iRun = true;
                    _iThread.Start();
                }
            }
            remove
            {
                InnerWindowDestroyed -= value;

                // if no more listeners for the events
                if (null == InnerWindowCreated &&
                    null == InnerWindowDestroyed)
                {
                    _iRun = false;
                }
            }
        }

        public void Dispose()
        {
            Shutdown();
        }

        ~WindowHookNet()
        {
            if (_iRun)
            {
                Shutdown();
            }
        }
    }
}