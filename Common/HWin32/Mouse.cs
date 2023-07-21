using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace HWin32
{
    [Flags]
    public enum MouseEventFlags
    {
        Move = 0x0001,
        LeftDown = 0x0002,
        LeftUp = 0x0004,
        RightDown = 0x0008,
        RightUp = 0x0010,
        MiddleDown = 0x0020,
        MiddleUp = 0x0040,
        Wheel = 0x0800,
        Absolute = 0x8000
    }


    public static class Mouse
    {
        [DllImport("User32.dll")]
        public extern static void mouse_event(int dwFlags, int dx, int dy, int dwData, IntPtr dwExtraInfo);

        [DllImport("User32.dll")]
        public extern static int ShowCursor(bool bShow);

        #region 光标位置操作：以像素为单位。
        [DllImport("User32.dll")]
        public extern static void SetCursorPos(int x, int y);

        [DllImport("User32.dll")]
        public extern static bool GetCursorPos(out HPoint p); 
        #endregion
    }
}
