using System;
using System.Diagnostics;
using System.Threading;
using CreateHiddenProcessLib.CreateWindowUtility.Delegates;
using CreateHiddenProcessLib.CreateWindowUtility.Model;
using CreateHiddenProcessLib.CreateWindowUtility.Win32;

namespace CreateHiddenProcessLib.CreateWindowUtility
{
    public class WindowCreationHookerByClassName : IDisposable
    {
        private static WindowHookNet _getWindowHookNet;
        private string[] _windowclass;

        public Thread Thread => _getWindowHookNet?.Thread;

        public void Dispose()
        {
            _getWindowHookNet?.Dispose();
            _getWindowHookNet = null;
        }

        private static bool EvaluateCondition(ByRef<int> ffProcessId, uint processId)
        {
            return ffProcessId == null || processId == ffProcessId.Value;
        }

        private static bool EvaluateCondition(ByRef<Process> ffProcessId, uint processId)
        {
            return ffProcessId?.Value == null || processId == ffProcessId.Value.Id;
        }

        ~WindowCreationHookerByClassName()
        {
            _getWindowHookNet?.Dispose();
        }

        public event EventHandler<WindowHookEventArgs> FirefowWindowCreated;

        public static WindowCreationHookerByClassName GetInstance(params string[] classes)
        {
            var instance = new WindowCreationHookerByClassName {_windowclass = classes};
            return instance;
        }

        private static WindowHookNet GetWindowHookNet(WindowHookDelegate windowHookNetWindowCreated)
        {
            _getWindowHookNet = new WindowHookNet("WindowHookNet" + DateTime.Now.ToString("HH:mm:ss mmm"));
            _getWindowHookNet.WindowCreated += windowHookNetWindowCreated;
            return _getWindowHookNet;
        }

        public WindowHookNet HookFirefoxCreation(ByRef<int> ffProcessId,
            EventHandler<WindowHookEventArgs> firefowWindowCreated = null)
        {
            void WindowHookNetWindowCreated(object aSender, WindowHookEventArgs aArgs)
            {
                Debug.WriteLine($"{aArgs.WindowClass}-{aArgs.WindowTitle}");
                foreach (var windowclass in _windowclass)
                {
                    if (aArgs.WindowClass == windowclass)
                    {
                        uint processId;
                        Win32Interop.GetWindowThreadProcessId(aArgs.Handle, out processId);
                        if (EvaluateCondition(ffProcessId, processId))
                        {
                            FirefowWindowCreated?.Invoke(this, aArgs);
                            firefowWindowCreated?.Invoke(this, aArgs);
                        }
                    }
                }
            }

            return GetWindowHookNet(WindowHookNetWindowCreated);
        }

        public WindowHookNet HookFirefoxCreation(ByRef<Process> ffProcessId,
            EventHandler<WindowHookEventArgs> firefowWindowCreated = null)
        {
            void WindowHookNetWindowCreated(object aSender, WindowHookEventArgs aArgs)
            {
                Debug.WriteLine($"{aArgs.WindowClass}-{aArgs.WindowTitle}");
                foreach (var windowclass in _windowclass)
                {
                    if (aArgs.WindowClass == windowclass)
                    {
                        uint processId;
                        Win32Interop.GetWindowThreadProcessId(aArgs.Handle, out processId);
                        if (EvaluateCondition(ffProcessId, processId))
                        {
                            FirefowWindowCreated?.Invoke(this, aArgs);
                            firefowWindowCreated?.Invoke(this, aArgs);
                        }
                    }
                }
            }

            return GetWindowHookNet(WindowHookNetWindowCreated);
        }
    }
}