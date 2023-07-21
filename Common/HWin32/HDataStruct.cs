using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace HWin32
{
    [StructLayout(LayoutKind.Sequential)]
    public struct HPoint
    {
        public int X;
        public int Y;
    }    

    [StructLayout(LayoutKind.Sequential)]
    public struct HRect
    {
        public int left;
        public int top;
        public int right;
        public int bottom;        
    }


}
