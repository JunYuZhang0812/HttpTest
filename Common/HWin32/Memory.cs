using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace HWin32
{
    public static class Memory
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LocalAlloc(int flag, int size);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LocalFree(IntPtr p);
    }
}
