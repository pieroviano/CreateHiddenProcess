using CreateHiddenProcessLib.CreateWindowUtility.Model;

namespace CreateHiddenProcessLib.CreateWindowUtility.Delegates
{
    /// <summary>
    ///     use this to get informed about window creation / destruction events
    /// </summary>
    /// <param name="aSender"></param>
    /// <param name="aArgs">contains information about the window</param>
    public delegate void WindowHookDelegate(object aSender, WindowHookEventArgs aArgs);
}