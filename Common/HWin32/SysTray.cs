using HWin32Ex;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HWin32
{
    public static class SysTray
    {
        //取得系统托盘窗口的句柄
        private static int GetSysTrayWnd()
        {
            OSName osn = OS.GetVersion();
            int k = Windows.FindWindow("Shell_TrayWnd", null);
            k = Windows.FindWindowEx(k, 0, "TrayNotifyWnd", null);
            k = Windows.FindWindowEx(k, 0, "SysPager", null);
            k = Windows.FindWindowEx(k, 0, "ToolbarWindow32", null);

            if (osn == OSName.Win2000 || osn == OSName.WinXP || osn == OSName.Win2003)
            {
                if (osn == OSName.Win2000)
                {
                    //k = Windows.FindWindowEx(k, 0, "ToolbarWindow32", null);
                    return k;
                }
                else
                {
                    //k = Windows.FindWindowEx(k, 0, "SysPager", null);
                    //k = Windows.FindWindowEx(k, 0, "ToolbarWindow32", null);
                    return k;
                }
            }
            else
            {
                return k;
            }
        }

        //刷新托盘区域：已中止进程的图标将会被系统删除。
        public static void Refresh()
        {
            int hwnd = GetSysTrayWnd();
            HRect nr = new HRect();

            Windows.GetClientRect((IntPtr)hwnd, out nr);

            for (int x = 0; x < nr.right; x = x + 2)
            {
                for (int y = 0; y < nr.bottom; y = y + 2)
                {
                    Message.SendMessage(hwnd, MsgId.WM_MOUSEMOVE, 0, Message.MakeLParam(x, y));
                }
            }
        }
        private static Rectangle GetNotificationRect()
        {
            TrayStateMethods.RECT rect = new TrayStateMethods.RECT();

            IntPtr hwnd = TrayStateMethods.FindWindowEx(IntPtr.Zero, IntPtr.Zero, "Shell_TrayWnd", null);

            if (hwnd == IntPtr.Zero)
            {
                throw new Win32Exception();
            }

            hwnd = TrayStateMethods.FindWindowEx(hwnd, IntPtr.Zero, "TrayNotifyWnd", null);

            if (hwnd == IntPtr.Zero)
            {
                throw new Win32Exception();
            }

            hwnd = TrayStateMethods.FindWindowEx(hwnd, IntPtr.Zero, "SysPager", null);

            if (hwnd == IntPtr.Zero)
            {
                throw new Win32Exception();
            }

            hwnd = TrayStateMethods.FindWindowEx(hwnd, IntPtr.Zero, "ToolbarWindow32", null);

            if (hwnd == IntPtr.Zero)
            {
                throw new Win32Exception();
            }

            TrayStateMethods.GetWindowRect(hwnd, ref rect);

            return rect.Rect;
        }

    }


}
