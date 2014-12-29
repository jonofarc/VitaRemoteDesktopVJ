using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using VitaRemoteServer;


namespace VitaRemoteServer.Input
{
    public class MouseInput
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern void mouse_event(uint dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        private const int MOUSEEVENTF_ABSOLUTE = 0x8000;
        private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const int MOUSEEVENTF_LEFTUP = 0x0004;
        private const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        private const int MOUSEEVENTF_MIDDLEUP = 0x0040;
        private const int MOUSEEVENTF_MOVE = 0x0001;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const int MOUSEEVENTF_RIGHTUP = 0x0010;
        private const int MOUSEEVENTF_WHEEL = 0x0800;
        private const int MOUSEEVENTF_XDOWN = 0x0080;
        private const int MOUSEEVENTF_XUP = 0x0100;

        public static void MouseClick(MouseButtons key, Point pt)
        {

            //mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE, getAbsoluteX(pt.X), getAbsoluteY(pt.Y), 0, 0);
            MouseMove(pt);
            switch (key)
            {
                case MouseButtons.Left:
                    mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                    break;
                case MouseButtons.Right:
                    mouse_event(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
                    break;
                case MouseButtons.Middle:
                    mouse_event(MOUSEEVENTF_MIDDLEDOWN | MOUSEEVENTF_MIDDLEUP, 0, 0, 0, 0);
                    break;
            }
        }
        public static void MousePress(MouseButtons key, Point pt)
        {
            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE, getAbsoluteX(pt.X), getAbsoluteY(pt.Y), 0, 0);
            switch (key)
            {
                case MouseButtons.Left:
                    mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                    break;
                case MouseButtons.Right:
                    mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
                    break;
                case MouseButtons.Middle:
                    mouse_event(MOUSEEVENTF_MIDDLEDOWN, 0, 0, 0, 0);
                    break;
            }
        }
        public static void MouseUp(MouseButtons key, Point pt)
        {
            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE, getAbsoluteX(pt.X), getAbsoluteY(pt.Y), 0, 0);
            switch (key)
            {
                case MouseButtons.Left:
                    mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                    break;
                case MouseButtons.Right:
                    mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
                    break;
                case MouseButtons.Middle:
                    mouse_event(MOUSEEVENTF_MIDDLEUP, 0, 0, 0, 0);
                    break;
            }
        }
        public static void MouseMove(Point pt)
        {

            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE, getAbsoluteX(pt.X), getAbsoluteY(pt.Y), 0, 0);
            //Cursor.Position = pt;
        }
        public static void MouseDoubleClick(MouseButtons key, Point pt)
        {
            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE, getAbsoluteX(pt.X), getAbsoluteY(pt.Y), 0, 0);
            switch (key)
            {
                case MouseButtons.Left:
                    mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                    mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                    break;
                case MouseButtons.Right:
                    mouse_event(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
                    mouse_event(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
                    break;
                case MouseButtons.Middle:
                    mouse_event(MOUSEEVENTF_MIDDLEDOWN | MOUSEEVENTF_MIDDLEUP, 0, 0, 0, 0);
                    mouse_event(MOUSEEVENTF_MIDDLEDOWN | MOUSEEVENTF_MIDDLEUP, 0, 0, 0, 0);
                    break;
            }
        }

        public static int getAbsoluteX(int x)
        {
            return (int)(((float)65535 / Settings.ScreenWidth) * x);
        }

        public static int getAbsoluteY(int y)
        {
            return (int)(((float)65535 / Settings.ScreenHeight) * y);
        }
    }
}
