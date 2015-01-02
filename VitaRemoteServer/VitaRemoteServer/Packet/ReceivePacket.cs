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
        private const int MouseSensitivity = 10;
        private const int motionSensitivity = 3;
		
		public static int m_xbefore=0;
		public static int m_ybefore=0;
		
        public static void Tap(Point pt)
        {
            MouseInput.MouseClick(MouseButtons.Left, pt);
			
        }

        public static void DoubleTap(Point pt)
        {
            MouseInput.MouseDoubleClick(MouseButtons.Left, pt);//jonathan removed dubleclick to be able to use it as a button
			
        }

        public static void Drag(Point pt)
        {
			
			
			
			//jonathan 480 and 270 being the center of the screen  /50 is the speed of the movement 
			//so the farther you drag from the center of the screen the faster it moves the capturing area
			screenCapture.X += (480-pt.X)/50;
			screenCapture.Y += (270-pt.Y)/50;
			Console.WriteLine("ptY"+pt.Y);
			Console.WriteLine("ptX"+pt.X);
			
			
			Console.WriteLine(" drag");
			
           // System.Diagnostics.Debug.WriteLine(screenCapture.X.ToString() + "  " + screenCapture.Y.ToString());
           // screenCapture.X -= pt.X;
           // screenCapture.Y -= pt.Y;
			
        }
	 public static void LongPress(Point pt)
        {
			screenCapture.X = 0;
			screenCapture.Y = 0;
			Console.WriteLine(" longpress");

			
        }
        public static void DragStart(Point pt)
        {
	
			Console.WriteLine(" dragstart");
			

        }
        public static void DragEnd(Point pt)
        {
			

			Console.WriteLine(" dragend");
			
	
        }
		  public static void Drag2(Point pt)
        {
			//jonathan with this you can controll quality of image using  drag with 2 fingers horizontaly
			int _quality=0;
			 _quality = ((pt.X)*100)/980;
			
			
			if(_quality>100){
				_quality=100;
			}
			if(_quality<5){
				_quality=5;
			}
			screenCapture.Quality=_quality;
			
			
       
			
        }

        public static void MouseMove(Point pt)
        {
            MouseInput.MouseMove(pt);
        }

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
			
			//jonathan disabled this line so you can use Left trigger asa normal key intead of using it to controll mouse

          //  Cursor.Position = new Point(Cursor.Position.X - (y * motionSensitivity), Cursor.Position.Y - (x * motionSensitivity));
			
			//jonathan disabled this lines in order to be able to use leftstick while you hold down left Trigger
            if (aX > 2)
            //    keyBoardInput.KeyPress((byte)Keys.D, true);

            if(aX < -2)
             //   keyBoardInput.KeyPress((byte)Keys.A, true);

            if (aX > -2 && aX < 2)
            {
             //   keyBoardInput.KeyPress((byte)Keys.A, false);
              //  keyBoardInput.KeyPress((byte)Keys.D, false);
            }//

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
            if (gamePadInput.PSV_RIGHT_ANALOGX == 1)
            {
             Cursor.Position = new Point(Cursor.Position.X + MouseSensitivity, Cursor.Position.Y);
				 keyBoardInput.KeyPress((byte)Keys.C, true);
            }


            if (gamePadInput.PSV_RIGHT_ANALOGX == 2)
            {
                Cursor.Position = new Point(Cursor.Position.X - MouseSensitivity, Cursor.Position.Y);
                
				 keyBoardInput.KeyPress((byte)Keys.V, true);
			
            }
			
			if (gamePadInput.PSV_RIGHT_ANALOGX == 0)
            {
                
              
				 keyBoardInput.KeyPress((byte)Keys.C, false);
				 keyBoardInput.KeyPress((byte)Keys.V, false);
			
            }
			 


            if (gamePadInput.PSV_RIGHT_ANALOGY == 1)
            {
                Cursor.Position = new Point(Cursor.Position.X, Cursor.Position.Y + MouseSensitivity);
				keyBoardInput.KeyPress((byte)Keys.B, true);
            }

            if (gamePadInput.PSV_RIGHT_ANALOGY == 2)
            {
                Cursor.Position = new Point(Cursor.Position.X, Cursor.Position.Y - MouseSensitivity);
				keyBoardInput.KeyPress((byte)Keys.N, true);
            }
			 if (gamePadInput.PSV_RIGHT_ANALOGY == 0)
            {
              	keyBoardInput.KeyPress((byte)Keys.B, false);
				keyBoardInput.KeyPress((byte)Keys.N, false);
            }
			
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
                keyBoardInput.KeyPress(gamePadInput.LTRIGGER, (gamePadInput.PSV_BTN_LTRIGGER == 1) ? true : false);
                keyBoardInput.KeyPress(gamePadInput.RTRIGGER, (gamePadInput.PSV_BTN_RTRIGGER == 1) ? true : false);
                //if(gamePadInput.PSV_BTN_RTRIGGER == 1)
                //{
                  //  Point pt = Cursor.Position;
                   // MouseInput.MouseClick(MouseButtons.Left, pt);
                //}

                //keyBoardInput.KeyPress(gamePadInput.SELECT, (gamePadInput.PSV_BTN_SELECT == 1) ? true : false);
                   if (gamePadInput.PSV_BTN_SELECT == 1)
                    screenCapture.ShowCursor = !screenCapture.ShowCursor;
                if (gamePadInput.PSV_BTN_SELECT == 1){
					
					 // screenCapture.ShowCursor = !screenCapture.ShowCursor;  //show hide cursor
				
					screenCapture.ShowCursor=false;
					keyBoardInput.KeyPress((byte)Keys.Q, true);
					
				}
				if (gamePadInput.PSV_BTN_SELECT == 0){
					
					
					keyBoardInput.KeyPress((byte)Keys.Q, false);
					
				}
				
                keyBoardInput.KeyPress(gamePadInput.START, (gamePadInput.PSV_BTN_START == 1) ? true : false);

                if(gamePadInput.PSV_LEFT_ANALOGX == 1)
                    keyBoardInput.KeyPress((byte)Keys.D, true);

                if (gamePadInput.PSV_LEFT_ANALOGX == 2)
                    keyBoardInput.KeyPress((byte)Keys.A, true);

                if (gamePadInput.PSV_LEFT_ANALOGX == 0)
                {
                    keyBoardInput.KeyPress((byte)Keys.D, false);
                    keyBoardInput.KeyPress((byte)Keys.A, false);
                }

                if (gamePadInput.PSV_LEFT_ANALOGY == 1)
                    keyBoardInput.KeyPress((byte)Keys.S, true);

                if (gamePadInput.PSV_LEFT_ANALOGY == 2)
                    keyBoardInput.KeyPress((byte)Keys.W, true);

                if (gamePadInput.PSV_LEFT_ANALOGY == 0)
                {
                    keyBoardInput.KeyPress((byte)Keys.W, false);
                    keyBoardInput.KeyPress((byte)Keys.S, false);
                }


            }
            else
            {
                //gamepad input
            }
        }
    }
}
