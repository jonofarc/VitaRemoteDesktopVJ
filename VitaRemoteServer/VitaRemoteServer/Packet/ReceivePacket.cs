using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using VitaRemoteServer.Input;

namespace VitaRemoteServer
{
    class ReceivePacket
    {
        private static int MouseSensitivity = 10;
        private static int motionSensitivity = 3;

        private static int leftTriggerBuffer = 0;
        private static int RightTriggerBuffer = 0;


        public static int MouseSpeed
        {
            get { return MouseSensitivity; }
            set { MouseSensitivity = value; }
        }

        // for tapping with 1 finger
        public static void Tap(Point pt)
        {
            if (gamePadInput.MOUSE_RELATIVE_ONE)
            {
                MouseInput.MouseClick((MouseButtons)gamePadInput.ONE_TAP, Cursor.Position);
            }
            else
            {
                MouseInput.MouseClick((MouseButtons)gamePadInput.ONE_TAP, pt);
            }
        }

        // for double tapping with 1 finger
        public static void DoubleTap(Point pt)
        {
            if (gamePadInput.MOUSE_RELATIVE_ONE)
            {
                MouseInput.MouseDoubleClick((MouseButtons)gamePadInput.ONE_TAP, Cursor.Position);
            }
            else
            {
                MouseInput.MouseDoubleClick((MouseButtons)gamePadInput.ONE_TAP, pt);
            }
        }

        // dragging on screen with 1 finger
        public static void Drag1(Point pt)
        {
            if (gamePadInput.ONE_TOUCH_MOVEMENT == 1)
            {
                MouseInput.MouseMove(pt, gamePadInput.MOUSE_RELATIVE_ONE);
            }
            if (gamePadInput.ONE_TOUCH_MOVEMENT == 2)
            {
                MouseInput.ScreenMove(pt);
            }
        }

        // dragging on screen with 2 fingers
        public static void Drag2(Point pt)
        {
            if (gamePadInput.TWO_TOUCH_MOVEMENT == 1)
            {
                MouseInput.MouseMove(pt, gamePadInput.MOUSE_RELATIVE_TWO);
            }
            if (gamePadInput.TWO_TOUCH_MOVEMENT == 2)
            {
                MouseInput.ScreenMove(pt);
            }
        }

        // mouse event for when you start dragging
        public static void DragStart(Point pt)
        {
        }

        // mouse event for when you stop dragging
        public static void DragEnd(Point pt)
        {
        }

        public static void LongPress(Point pt)
        {
            throw new NotImplementedException();
        }

        #region OLD
        public static void MouseRightClick(Point pt)
        {
            MouseInput.MouseClick(MouseButtons.Right, pt);
        }

        public static void MouseRightDoubleClick(Point pt)
        {
            MouseInput.MouseDoubleClick(MouseButtons.Right, pt);
        }

        public static void LeftMouseDown(Point pt)
        {
            MouseInput.MousePress(MouseButtons.Left, pt);
        }

        public static void LeftMouseUp(Point pt)
        {
            MouseInput.MouseUp(MouseButtons.Left, pt);

        }
        #endregion

        public static void MotionData(byte[] motionData)
        {
            int x, y, z, aX, aY, aZ;
            //x = z;
            x = BitConverter.ToInt16(motionData, 0);
            y = BitConverter.ToInt16(motionData, 2);
            z = BitConverter.ToInt16(motionData, 4);

            aX = BitConverter.ToInt16(motionData, 6);
            aY = BitConverter.ToInt16(motionData, 8);
            aZ = BitConverter.ToInt16(motionData, 10);

            Cursor.Position = new Point(Cursor.Position.X - (y * motionSensitivity), Cursor.Position.Y - (x * motionSensitivity));

            if (aX > 2)
                keyBoardInput.KeyPress((byte)Keys.D, true);

            if(aX < -2)
                keyBoardInput.KeyPress((byte)Keys.A, true);

            if (aX > -2 && aX < 2)
            {
                keyBoardInput.KeyPress((byte)Keys.A, false);
                keyBoardInput.KeyPress((byte)Keys.D, false);
            }

            System.Diagnostics.Debug.WriteLine(aX + " " + aY + " " + aZ);
        }

        public static void ReceiveGamePadInput(byte[] data)
        {
            //uint gamePadInput = BitConverter.ToUInt32(data, 0);
            //gamePadInput.PSV_BTN_LEFT = ((gamePadInput & Input.GamePadButtons.LEFT) != 0) ? 1:0);
            //gamePadInput.PSV_BTN_UP = ((gamePadInput & Input.GamePadButtons.UP) != 0) ? 1:0);

            gamePadInput.PSV_BTN_LEFT =         (int)data[0];
            gamePadInput.PSV_BTN_RIGHT =        (int)data[1];
            gamePadInput.PSV_BTN_UP =           (int)data[2];
            gamePadInput.PSV_BTN_DOWN =         (int)data[3];
            gamePadInput.PSV_BTN_SQUARE =       (int)data[4];
            gamePadInput.PSV_BTN_CROSS =        (int)data[5];
            gamePadInput.PSV_BTN_CIRCLE =       (int)data[6];
            gamePadInput.PSV_BTN_TRIANGLE =     (int)data[7];
            gamePadInput.PSV_BTN_LTRIGGER =     (int)data[8];
            gamePadInput.PSV_BTN_RTRIGGER =     (int)data[9];
            gamePadInput.PSV_BTN_SELECT =       (int)data[10];
            gamePadInput.PSV_BTN_START =        (int)data[11];
            gamePadInput.PSV_LEFT_ANALOGX  =    (int)data[12];
            gamePadInput.PSV_LEFT_ANALOGY =     (int)data[13];
            gamePadInput.PSV_RIGHT_ANALOGX =    (int)data[14];
            gamePadInput.PSV_RIGHT_ANALOGY =    (int)data[15];

            updateInput();
        }

        public static void updateMouse()
        {
            Point deltaMouse = new Point(0, 0);
            // move the mouse if on the Left Stick
            if (gamePadInput.LEFT_MOUSE)
            {
                if (gamePadInput.PSV_LEFT_ANALOGX == 1)
                {
                    deltaMouse.X = MouseSensitivity;
                }
                if (gamePadInput.PSV_LEFT_ANALOGX == 2)
                {
                    deltaMouse.X = -MouseSensitivity;
                }
                if (gamePadInput.PSV_LEFT_ANALOGY == 1)
                {
                    deltaMouse.Y = MouseSensitivity;
                }
                if (gamePadInput.PSV_LEFT_ANALOGY == 2)
                {
                    deltaMouse.Y = -MouseSensitivity;
                }
            }
            // move the mouse if on the Right Stick
            else if (gamePadInput.RIGHT_MOUSE)
            {
                if (gamePadInput.PSV_RIGHT_ANALOGX == 1)
                {
                    deltaMouse.X = MouseSensitivity;
                }
                if (gamePadInput.PSV_RIGHT_ANALOGX == 2)
                {
                    deltaMouse.X = -MouseSensitivity;
                }
                if (gamePadInput.PSV_RIGHT_ANALOGY == 1)
                {
                    deltaMouse.Y = MouseSensitivity;
                }
                if (gamePadInput.PSV_RIGHT_ANALOGY == 2)
                {
                    deltaMouse.Y = -MouseSensitivity;
                }
            }
            MouseInput.MouseMove(deltaMouse, true);
        }

        public static void updateInput()
        {
            if (gamePadInput.inputType == InputType.keyboardMouseInput)
            {

                
                //keyboard input
                keyBoardInput.KeyPress(gamePadInput.UP, (gamePadInput.PSV_BTN_UP == 1) ? true : false);
                keyBoardInput.KeyPress(gamePadInput.DOWN, (gamePadInput.PSV_BTN_DOWN == 1) ? true : false);
                keyBoardInput.KeyPress(gamePadInput.LEFT, (gamePadInput.PSV_BTN_LEFT == 1) ? true : false);
                keyBoardInput.KeyPress(gamePadInput.RIGHT, (gamePadInput.PSV_BTN_RIGHT == 1) ? true : false);
                keyBoardInput.KeyPress(gamePadInput.SQUARE, (gamePadInput.PSV_BTN_SQUARE == 1) ? true : false);
                keyBoardInput.KeyPress(gamePadInput.CROSS, (gamePadInput.PSV_BTN_CROSS == 1) ? true : false);
                keyBoardInput.KeyPress(gamePadInput.CIRCLE, (gamePadInput.PSV_BTN_CIRCLE == 1) ? true : false);
                keyBoardInput.KeyPress(gamePadInput.TRIANGLE, (gamePadInput.PSV_BTN_TRIANGLE == 1) ? true : false);

                // handle of triggers are set for mouse clicks
                if (gamePadInput.TRIGGERS_AS_MOUSE_CLICKS)
                {
                    if (leftTriggerBuffer != gamePadInput.PSV_BTN_LTRIGGER)
                    {
                        MouseInput.MouseButton(MouseButtons.Left, Cursor.Position, (gamePadInput.PSV_BTN_LTRIGGER == 1) ? true : false);
                    }

                    if (RightTriggerBuffer != gamePadInput.PSV_BTN_RTRIGGER)
                    {
                        MouseInput.MouseButton(MouseButtons.Right, Cursor.Position, (gamePadInput.PSV_BTN_RTRIGGER == 1) ? true : false);
                    }

                    // save teh bufferz
                    leftTriggerBuffer = gamePadInput.PSV_BTN_LTRIGGER;
                    RightTriggerBuffer = gamePadInput.PSV_BTN_RTRIGGER;
                }
                else
                {
                    keyBoardInput.KeyPress(gamePadInput.LTRIGGER, (gamePadInput.PSV_BTN_LTRIGGER == 1) ? true : false);
                    keyBoardInput.KeyPress(gamePadInput.RTRIGGER, (gamePadInput.PSV_BTN_RTRIGGER == 1) ? true : false);
                }


                //keyBoardInput.KeyPress(gamePadInput.SELECT, (gamePadInput.PSV_BTN_SELECT == 1) ? true : false);
                if (gamePadInput.PSV_BTN_SELECT == 1)
                    screenCapture.ShowCursor = !screenCapture.ShowCursor;

                keyBoardInput.KeyPress(gamePadInput.START, (gamePadInput.PSV_BTN_START == 1) ? true : false);

                MouseButtons button = MouseButtons.None;
                bool autoButton = true;

                if (gamePadInput.MOUSE_AUTO_LEFT)
                {
                    button = MouseButtons.Left;
                }
                else if (gamePadInput.MOUSE_AUTO_RIGHT)
                {
                    button = MouseButtons.Right;
                }
                else
                {
                    autoButton = false;
                }

                // use keyboard keys on Left stick if it is not set for mouse
                if (gamePadInput.LEFT_MOUSE == false)
                {
                    if (gamePadInput.PSV_LEFT_ANALOGX == 1)
                        keyBoardInput.KeyPress((byte)gamePadInput.LEFT_RIGHT, true);

                    if (gamePadInput.PSV_LEFT_ANALOGX == 2)
                        keyBoardInput.KeyPress((byte)gamePadInput.LEFT_LEFT, true);

                    if (gamePadInput.PSV_LEFT_ANALOGX == 0)
                    {
                        keyBoardInput.KeyPress((byte)gamePadInput.LEFT_RIGHT, false);
                        keyBoardInput.KeyPress((byte)gamePadInput.LEFT_LEFT, false);
                    }

                    if (gamePadInput.PSV_LEFT_ANALOGY == 1)
                        keyBoardInput.KeyPress((byte)gamePadInput.LEFT_DOWN, true);

                    if (gamePadInput.PSV_LEFT_ANALOGY == 2)
                        keyBoardInput.KeyPress((byte)gamePadInput.LEFT_UP, true);

                    if (gamePadInput.PSV_LEFT_ANALOGY == 0)
                    {
                        keyBoardInput.KeyPress((byte)gamePadInput.LEFT_DOWN, false);
                        keyBoardInput.KeyPress((byte)gamePadInput.LEFT_UP, false);
                    }
                }
                else if(autoButton)
                {
                    if (gamePadInput.PSV_LEFT_ANALOGX == 0 && gamePadInput.PSV_LEFT_ANALOGY == 0)
                    {
                        // mouse button up
                        MouseInput.MouseUp(button, Cursor.Position);
                    }
                    else
                    {
                        // mouse button down
                        MouseInput.MousePress(button, Cursor.Position);
                    }
                }

                // use keyboard keys on Right stick if it is not set for mouse
                if (gamePadInput.RIGHT_MOUSE == false)
                {
                    if (gamePadInput.PSV_RIGHT_ANALOGX == 1)
                        keyBoardInput.KeyPress((byte)gamePadInput.RIGHT_RIGHT, true);

                    if (gamePadInput.PSV_RIGHT_ANALOGX == 2)
                        keyBoardInput.KeyPress((byte)gamePadInput.RIGHT_LEFT, true);

                    if (gamePadInput.PSV_RIGHT_ANALOGX == 0)
                    {
                        keyBoardInput.KeyPress((byte)gamePadInput.RIGHT_RIGHT, false);
                        keyBoardInput.KeyPress((byte)gamePadInput.RIGHT_LEFT, false);
                    }

                    if (gamePadInput.PSV_RIGHT_ANALOGY == 1)
                        keyBoardInput.KeyPress((byte)gamePadInput.RIGHT_DOWN, true);

                    if (gamePadInput.PSV_RIGHT_ANALOGY == 2)
                        keyBoardInput.KeyPress((byte)gamePadInput.RIGHT_UP, true);

                    if (gamePadInput.PSV_RIGHT_ANALOGY == 0)
                    {
                        keyBoardInput.KeyPress((byte)gamePadInput.RIGHT_DOWN, false);
                        keyBoardInput.KeyPress((byte)gamePadInput.RIGHT_UP, false);
                    }
                }
                else if (autoButton)
                {
                    if (gamePadInput.PSV_RIGHT_ANALOGX == 0 && gamePadInput.PSV_RIGHT_ANALOGY == 0)
                    {
                        // mouse button up
                        MouseInput.MouseUp(button, Cursor.Position);
                    }
                    else
                    {
                        // mouse button down
                        MouseInput.MousePress(button, Cursor.Position);
                    }
                }

            }
            else
            {
                //gamepad input
            }
        }
    }
}
