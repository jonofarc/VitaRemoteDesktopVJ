using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VitaRemoteServer.Input
{
    public enum GamePadButtons : uint
    {
        LEFT = 1,
        UP = 2,
        RIGHT = 4,
        DOWN = 8,
        SQUARE = 0x10,
        TRIANGLE = 0x20,
        CIRCLE = 0x40,
        CROSS = 0x80,
        START = 0x100,
        SELECT = 0x200,
        L = 0x400,
        R = 0x800
    }

    public struct PSVitaInput
    {
        public InputType Type;
        public string Name;
        public ushort ScanCode;
        public int EndZoneX;
        public int EndZoneY;
        public int EndZoneZ;
    }

    public  enum InputType
    {
        keyboardMouseInput = 0,
        gamePadInput = 1
    }
    public static class gamePadInput
    {

        public static InputType inputType = InputType.keyboardMouseInput;

        public static int PSV_BTN_UP;
        public static int PSV_BTN_DOWN;
        public static int PSV_BTN_LEFT;
        public static int PSV_BTN_RIGHT;
        public static int PSV_BTN_CROSS;
        public static int PSV_BTN_SQUARE;
        public static int PSV_BTN_CIRCLE;
        public static int PSV_BTN_TRIANGLE;
        public static int PSV_BTN_LTRIGGER;
        public static int PSV_BTN_RTRIGGER;
        public static int PSV_BTN_SELECT;
        public static int PSV_BTN_START;
        public static int PSV_LEFT_ANALOGX;
        public static int PSV_LEFT_ANALOGY;
        public static int PSV_RIGHT_ANALOGX;
        public static int PSV_RIGHT_ANALOGY;

        public static uint UP = (uint)Keys.T;
        public static uint DOWN = (uint)Keys.G;
        public static uint LEFT = (uint)Keys.F;
        public static uint RIGHT = (uint)Keys.H;
        public static uint SQUARE = (uint)Keys.J;
        public static uint CROSS = (uint)Keys.K;
        public static uint CIRCLE = (uint)Keys.L;
        public static uint TRIANGLE = (uint)Keys.I;
        public static uint LTRIGGER = (uint)Keys.U;
        public static uint RTRIGGER = (uint)Keys.O;
        public static uint SELECT = (uint)Keys.Q;
        public static uint START = (uint)Keys.Enter;
 
    }
}
