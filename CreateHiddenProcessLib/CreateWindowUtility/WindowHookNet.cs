using System;
using System.Collections.Generic;
using System.Threading;
using CreateHiddenProcessLib.CreateWindowUtility.Delegates;
using CreateHiddenProcessLib.CreateWindowUtility.Model;
using CreateHiddenProcessLib.Utility;

namespace CreateHiddenProcessLib.CreateWindowUtility
{
    /// <summary>
    ///     allows you to get information about the creation and /or the destruction of
    ///     windows
    /// </summary>
    public class WindowHookNet : WindowHookNetBase
    {
        private readonly List<WindowHookEventArgs> _eventsToFire = new List<WindowHookEventArgs>();

        private readonly Dictionary<IntPtr, WindowHookEventArgs> _newWindowList =
            new Dictionary<IntPtr, WindowHookEventArgs>();

        private readonly Dictionary<IntPtr, WindowHookEventArgs> _oldWindowList =
            new Dictionary<IntPtr, WindowHookEventArgs>();

        public WindowHookNet(string threadName)
        {
            _isNotDisposed = false;
            ThreadStart tStart = Run;
            Thread = new Thread(tStart) {Name = threadName};
        }

        public Thread Thread { get; set; }

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

        private void Run()
        {
            try
            {
                while (_isNotDisposed)
                {
                    _newWindowList.Clear();
                    WindowUtility.EnumerateWindows(_newWindowList);
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
                }

                // if the hook has been uninstalled
                // delete the list of old windows
                // because when it is restarted you do not want to get a whole 
                // lot of events for windows that where already there

                _oldWindowList.Clear();
                Thread = null;
            }
            catch (Exception aException)
            {
                Console.Out.WriteLine("exception in thread:" + aException);
            }
            Thread = null;
        }

        /// <summary>
        ///     register to this event if you want to be informed about
        ///     the creation of a window
        /// </summary>
        public override event WindowHookDelegate WindowCreated
        {
            add
            {
                base.WindowCreated += value;
                if (!_isNotDisposed)
                {
                    _isNotDisposed = true;
                    Thread.Start();
                }
            }
            remove
            {
                base.WindowCreated -= value;

                // if no more listeners for the events
                if (!IsAttachedAnyHandler)
                {
                    _isNotDisposed = false;
                }
            }
        }

        /// <summary>
        ///     register to this event, if you want to be informed about
        ///     the destruction of a window
        /// </summary>
        public override event WindowHookDelegate WindowDestroyed
        {
            add
            {
                base.WindowDestroyed += value;
                if (!_isNotDisposed)
                {
                    _isNotDisposed = true;
                    Thread.Start();
                }
            }
            remove
            {
                base.WindowDestroyed -= value;

                // if no more listeners for the events
                if (!IsAttachedAnyHandler)
                {
                    _isNotDisposed = false;
                }
            }
        }
    }
}