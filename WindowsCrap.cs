using System.Runtime.InteropServices;

namespace Celestial
{
    /// <summary>
    /// Utilities to deal with Windows-related crap
    /// </summary>
    public static class WindowsCrap
    {
        /// <summary>
        /// Hide the command window, because somehow this is the only way to run an app as the current user without a
        /// visible window being left open. Thanks Bill.
        /// </summary>
        public static void HideWindow()
        {
            IntPtr handle = GetConsoleWindow();
            if (handle != IntPtr.Zero)
            {
                ShowWindow(handle, 0);
            }
        }

        [DllImport("Kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();
        [DllImport("User32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int cmdShow);
    }
}