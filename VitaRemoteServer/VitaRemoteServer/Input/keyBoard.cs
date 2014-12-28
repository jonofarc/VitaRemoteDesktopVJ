using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VitaRemoteServer.Input
{
    public static class keyBoardInput
    {

        [DllImport("User32.dll")]
        public static extern uint SendInput(uint numberOfInputs, [MarshalAs(UnmanagedType.LPArray, SizeConst = 1)] INPUT[] input, int structSize);

        [DllImport("user32.dll")]
        public static extern byte VkKeyScan(char ch);

        [DllImport("user32.dll")]
        public static extern uint MapVirtualKey(uint uCode, uint uMapType);

        private const int INPUT_MOUSE = 0;
        private const int INPUT_KEYBOARD = 1;
        private const int INPUT_HARDWARE = 2;

        private const uint KEYEVENTF_EXTENDEDKEY            = 0x0001;
        private const uint KEYEVENTF_KEYUP                  = 0x0002;
        private const uint KEYEVENTF_UNICODE                = 0x0004;
        private const uint KEYEVENTF_SCANCODE               = 0x0008;

        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEINPUT
        {
            int dx;
            int dy;
            uint mouseData;
            uint dwFlags;
            uint time;
            IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HARDWAREINPUT
        {
            uint uMsg;
            ushort wParamL;
            ushort wParamH;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct INPUT
        {
            [FieldOffset(0)]
            public int type;
            [FieldOffset(4)] //*
            public MOUSEINPUT mi;
            [FieldOffset(4)] //*
            public KEYBDINPUT ki;
            [FieldOffset(4)] //*
            public HARDWAREINPUT hi;
        }


        public static void KeyPress(uint vk, bool press)
        {
            INPUT[] input = new INPUT[1];

            ushort scanCode = (ushort)MapVirtualKey(vk, 0);

            input[0].type = INPUT_KEYBOARD;
            //input[0].ki.wVk = vk;
            input[0].ki.wScan = scanCode;
            input[0].ki.dwFlags = ((press == true) ? 0 : KEYEVENTF_KEYUP) | KEYEVENTF_SCANCODE;

            SendInput(1, input, System.Runtime.InteropServices.Marshal.SizeOf(input[0]));
        }
    }
}
