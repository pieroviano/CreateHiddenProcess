using System;

namespace CreateHiddenProcessLib.CreateWindowUtility.Model
{
    /// <summary>
    ///     stores data about the raised event
    /// </summary>
    public class WindowHookEventArgs
    {
        public IntPtr Handle = IntPtr.Zero;
        public string WindowTitle;
        public string WindowClass;

        public override string ToString()
        {
            return "[WindowHookEventArgs|Title:" + WindowTitle + "|Class:"
                   + WindowClass + "|Handle:" + Handle + "]";
        }
    }
}