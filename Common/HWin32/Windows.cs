using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace HWin32
{
    public static class Windows
    {
        /// <summary>
        /// The FindWindow function retrieves a handle to the top-level 
        /// window whose class name and window name match the specified strings.
        /// This function does not search child windows. This function does not perform a case-sensitive search.
        /// </summary>
        /// <param name="strClassName">the class name for the window to search for</param>
        /// <param name="strWindowName">the name of the window to search for</param>
        /// <returns></returns>
        [DllImport("User32.dll")]
        public static extern int FindWindow(string strClassName, string strWindowName);

        /// <summary>
        /// The FindWindowEx function retrieves a handle to a window whose class name
        /// and window name match the specified strings.
        /// The function searches child windows, beginning with the one following the specified child window.
        /// This function does not perform a case-sensitive search.
        /// </summary>
        /// <param name="hwndParent">a handle to the parent window </param>
        /// <param name="hwndChildAfter">a handle to the child window to start search after</param>
        /// <param name="strClassName">the class name for the window to search for</param>
        /// <param name="strWindowName">the name of the window to search for</param>
        /// <returns></returns>
        [DllImport("User32.dll")]
        public static extern int FindWindowEx(int hwndParent, int hwndChildAfter, string strClassName, string strWindowName);

        /// <summary>
        /// The FindWindowEx API
        /// </summary>
        /// <param name="parentHandle">a handle to the parent window </param>
        /// <param name="childAfter">a handle to the child window to start search after</param>
        /// <param name="className">the class name for the window to search for</param>
        /// <param name="windowTitle">the name of the window to search for</param>
        /// <returns></returns>
        [DllImport("User32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

        [DllImport("User32.dll")]
        public static extern bool GetWindowRect(HandleRef hwnd, out HRect rect);

        [DllImport("User32.dll")]
        public static extern bool GetClientRect(IntPtr hWnd, out HRect lpRect);

        [DllImport("User32.dll")]
        public static extern bool EnableWindow(IntPtr hWnd, bool bEnable);
    }
}
