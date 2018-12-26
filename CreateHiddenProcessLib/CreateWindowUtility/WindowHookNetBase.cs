using System;
using CreateHiddenProcessLib.CreateWindowUtility.Delegates;
using CreateHiddenProcessLib.CreateWindowUtility.Model;

namespace CreateHiddenProcessLib.CreateWindowUtility
{
    public class WindowHookNetBase : IDisposable
    {
        protected bool _isNotDisposed;

        public WindowHookNetBase()
        {
            _isNotDisposed = true;
        }

        protected bool IsAttachedAnyHandler
        {
            get
            {
                if (null == InnerWindowCreated &&
                    null == InnerWindowDestroyed)
                {
                    return false;
                }
                return true;
            }
        }

        public void Dispose()
        {
            Shutdown();
            _isNotDisposed = false;
        }

        ~WindowHookNetBase()
        {
            if (_isNotDisposed)
            {
                Shutdown();
            }
        }

        protected event WindowHookDelegate InnerWindowCreated;
        protected event WindowHookDelegate InnerWindowDestroyed;

        protected void OnWindowCreated(WindowHookEventArgs aArgs)
        {
            InnerWindowCreated?.Invoke(this, aArgs);
        }

        protected void OnWindowDestroyed(WindowHookEventArgs aArgs)
        {
            InnerWindowDestroyed?.Invoke(this, aArgs);
        }

        public void Shutdown()
        {
            if (_isNotDisposed)
            {
                _isNotDisposed = false;
            }
        }

        /// <summary>
        ///     register to this event if you want to be informed about
        ///     the creation of a window
        /// </summary>
        public virtual event WindowHookDelegate WindowCreated
        {
            add => InnerWindowCreated += value;
            remove => InnerWindowCreated -= value;
        }

        /// <summary>
        ///     register to this event, if you want to be informed about
        ///     the destruction of a window
        /// </summary>
        public virtual event WindowHookDelegate WindowDestroyed
        {
            add => InnerWindowDestroyed += value;
            remove => InnerWindowDestroyed -= value;
        }
    }
}