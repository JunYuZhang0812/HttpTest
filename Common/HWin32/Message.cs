using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;

namespace HWin32
{
    /// <summary>
    /// Contains message information from a thread's message queue.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Msg
    {
        public IntPtr hWnd;
        public uint msg;
        public IntPtr wParam;
        public IntPtr lParam;
        public uint time;
        public Point p;
    }

    /// <summary>
    /// Message For Copy DataOnly. Used For Interactive of Processes
    /// </summary>
    public struct CopyDataStruct
    {
        public IntPtr dwData;
        public int cbData;
        [MarshalAs(UnmanagedType.LPStr)]
        public string lpData;
    }


    public static class Message
    {
        public static IntPtr MakeLParam(int LoWord, int HiWord)
        {
            return (IntPtr)((HiWord << 16) | (LoWord & 0xffff));
        }

        /// <summary>
        /// The SendMessage function sends the specified message to a
        /// window or windows. It calls the window procedure for the specified
        /// window and does not return until the window procedure
        /// has processed the message.
        /// </summary>
        /// <param name="hWnd">handle to destination window</param>
        /// <param name="Msg">message</param>
        /// <param name="wParam">first message parameter</param>
        /// <param name="lParam">second message parameter</param>
        /// <returns></returns>
        [DllImport("User32.dll")]
        public static extern Int32 SendMessage(int hWnd, int Msg, int wParam, [MarshalAs(UnmanagedType.LPStr)] string lParam);

        /// <summary>
        /// Send Message
        /// </summary>
        /// <param name="hWnd">handle to destination window</param>
        /// <param name="Msg">message</param>
        /// <param name="wParam">first message parameter</param>
        /// <param name="lParam">second message parameter</param>
        /// <returns></returns>
        [DllImport("User32.dll")]
        public static extern Int32 SendMessage(int hWnd, int Msg, int wParam, int lParam);


        [DllImport("User32.dll")]
        public static extern Int32 SendMessage(int hWnd, int Msg, int wParam, uint lParam);

        /// <summary>
        /// The SendMessage API
        /// </summary>
        /// <param name="hWnd">handle to the required window</param>
        /// <param name="msg">the system/Custom message to send</param>
        /// <param name="wParam">first message parameter</param>
        /// <param name="lParam">second message parameter</param>
        /// <returns></returns>
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(int hWnd, int msg, int wParam, IntPtr lParam);

        /// <summary>
        /// The SendMessage API
        /// </summary>
        /// <param name="hWnd">handle to the required window</param>
        /// <param name="Msg">the system/Custom message to send</param>
        /// <param name="wParam">first message parameter</param>
        /// <param name="lParam">second message parameter</param>
        /// <returns></returns>
        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, ref CopyDataStruct lParam);


        /// <summary>
        /// The PeekMessage function dispatches incoming sent messages, 
        /// checks the thread message queue for a posted message, 
        /// and retrieves the message (if any exist).
        /// </summary>
        [System.Security.SuppressUnmanagedCodeSecurity] // We won't use this maliciously
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool PeekMessage(out Msg msg, IntPtr hWnd, uint messageFilterMin, uint messageFilterMax, uint flags);

    }
}
