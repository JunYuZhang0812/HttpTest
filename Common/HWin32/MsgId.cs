using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HWin32
{
    public static class MsgId
    {
        public const int WM_CLOSE = 16;

        public const int WM_MOUSEMOVE = 0x200;
        public const int WM_LBUTTONDOWN = 0x0201;
        public const int WM_LBUTTONUP = 0x0202;
        public const int WM_LBUTTONDBLCLK = 0x0203;

        public const int WM_RBUTTONDOWN = 0x0204;
        public const int WM_RBUTTONUP = 0x0205;
        public const int WM_RBUTTONDBLCLK = 0x0206;

        public const int WM_MBUTTONDOWN = 0x0207;
        public const int WM_MBUTTONUP = 0x0208;
        public const int WM_MBUTTONDBLCLK = 0x0209;

        public const int WM_MOUSELAST = 0x0209;
        public const int WM_MOUSEWHEEL = 0x020A;


        public const int WM_COPYDATA = 0x004A;

        public const int WM_COMMAND = 0x111;

        public const int BN_CLICKED = 245;
    }
}
