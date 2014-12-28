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

        private static int prevX = -1;
        private static int prevY = -1;
        private static int CurX = -1;
        private static int CurY = -1;

        private static Point mousePosition = new Point();

        // handle a generic mouse button click
        // true = down
        // false = up
        public static void MouseButton(MouseButtons key, Point pt, bool bHow)
        {
            if (bHow) // down
            {
                MousePress(key, pt);
            }
            else // up
            {
                MouseUp(key, pt);
            }
        }
        public static void MouseClick(MouseButtons key, Point pt)
        {

            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE, getAbsoluteX(pt.X), getAbsoluteY(pt.Y), 0, 0);
            MouseMove(pt, false);
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
        public static void MouseMove(Point pt, bool bRelative)
        {
            if (bRelative)
            {
                Point pos = new Point();
                pos.X = Cursor.Position.X + pt.X;
                pos.Y = Cursor.Position.Y + pt.Y;
                Cursor.Position = pos;
            }
            else
            {
                Cursor.Position = pt;
            }
        }
        public static void ScreenMove(Point pt)
        {
            screenCapture.X -= pt.X;
            screenCapture.Y -= pt.Y;
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

        public static int getRelativeX(int x)
        {
            if (prevX == -1)
            {
                prevX = getAbsoluteX(x);
                CurX = prevX;
                return Cursor.Position.X;
            }

            prevX = CurX;
            CurX = getAbsoluteX(x);
            CurX += (prevX - CurX);
            return CurX;
        }
        public static int getRelativeY(int y)
        {
            if (prevY == -1)
            {
                prevY = getAbsoluteY(y);
                CurY = prevY;
                return Cursor.Position.Y;
            }

            prevY = CurY;
            CurY = getAbsoluteX(y);
            CurY += (prevY - CurY);
            return CurY;
        }
    }
}
