using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Security.AccessControl;

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
        public volatile InputType Type;
        public volatile string Name;
        public volatile ushort ScanCode;
        public volatile int EndZoneX;
        public volatile int EndZoneY;
        public volatile int EndZoneZ;
    }

    public enum InputType
    {
        keyboardMouseInput = 0,
        gamePadInput = 1
    }
    public class gamePadInput
    {
        public const string szSubFolder = "VitaRemote";
        public volatile static InputType inputType = InputType.keyboardMouseInput;

        public volatile static int PSV_BTN_UP;
        public volatile static int PSV_BTN_DOWN;
        public volatile static int PSV_BTN_LEFT;
        public volatile static int PSV_BTN_RIGHT;
        public volatile static int PSV_BTN_CROSS;
        public volatile static int PSV_BTN_SQUARE;
        public volatile static int PSV_BTN_CIRCLE;
        public volatile static int PSV_BTN_TRIANGLE;
        public volatile static int PSV_BTN_LTRIGGER;
        public volatile static int PSV_BTN_RTRIGGER;
        public volatile static int PSV_BTN_SELECT;
        public volatile static int PSV_BTN_START;
        public volatile static int PSV_LEFT_ANALOGX;
        public volatile static int PSV_LEFT_ANALOGY;
        public volatile static int PSV_RIGHT_ANALOGX;
        public volatile static int PSV_RIGHT_ANALOGY;

        public volatile static uint UP = (uint)Keys.Up;
        public volatile static uint DOWN = (uint)Keys.Down;
        public volatile static uint LEFT = (uint)Keys.Left;
        public volatile static uint RIGHT = (uint)Keys.Right;
        public volatile static uint SQUARE = (uint)Keys.J;
        public volatile static uint CROSS = (uint)Keys.K;
        public volatile static uint CIRCLE = (uint)Keys.L;
        public volatile static uint TRIANGLE = (uint)Keys.I;
        public volatile static uint LTRIGGER = (uint)Keys.U;
        public volatile static uint RTRIGGER = (uint)Keys.O;
        public volatile static uint SELECT = (uint)Keys.Space;
        public volatile static uint START = (uint)Keys.NumPad0;

        // left analog
        public volatile static bool LEFT_MOUSE = false;
        public volatile static uint LEFT_UP = (uint)Keys.W;
        public volatile static uint LEFT_DOWN = (uint)Keys.S;
        public volatile static uint LEFT_LEFT = (uint)Keys.A;
        public volatile static uint LEFT_RIGHT = (uint)Keys.D;

        // right analog
        public volatile static bool RIGHT_MOUSE = false;
        public volatile static uint RIGHT_UP = (uint)Keys.T;
        public volatile static uint RIGHT_DOWN = (uint)Keys.G;
        public volatile static uint RIGHT_LEFT = (uint)Keys.F;
        public volatile static uint RIGHT_RIGHT = (uint)Keys.H;

        // mouse
        public volatile static bool TRIGGERS_AS_MOUSE_CLICKS = false;
        public volatile static bool MOUSE_AUTO_LEFT = false;
        public volatile static bool MOUSE_AUTO_RIGHT = false;

        // touch flags
        public volatile static bool ONE_TOUCH = false;
        public volatile static bool TWO_TOUCH = false;
        public volatile static bool ONE_TOUCH_BUFFER = false;
        public volatile static bool TWO_TOUCH_BUFFER = false;

        // NOTE: for these two:
        // 0 -> does nothing
        // 1 -> moves mouse
        // 2 -> pan screen
        public volatile static uint ONE_TOUCH_MOVEMENT = 1;
        public volatile static uint TWO_TOUCH_MOVEMENT = 2;

        // moves relative or obsolute mouse position
        // true == relative
        // false == absolute
        public volatile static bool MOUSE_RELATIVE_ONE = false; // for one touch
        public volatile static bool MOUSE_RELATIVE_TWO = true; // for two touch

        // these are going to be MouseButtons
        public volatile static uint ONE_DRAG_START   = (uint)MouseButtons.None;
        public volatile static uint ONE_DRAG_END     = (uint)MouseButtons.None;
        public volatile static uint ONE_TAP          = (uint)MouseButtons.Left;
        public volatile static uint TWO_DRAG_START   = (uint)MouseButtons.None;
        public volatile static uint TWO_DRAG_END     = (uint)MouseButtons.None;
        public volatile static uint TWO_TAP          = (uint)MouseButtons.Left;

        // save in appdata
        public static void Save(string szFileName)
        {
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string fullFilePath = appdata + "\\" + szSubFolder;

            // make the subfolder if it doesnt exist
            Directory.CreateDirectory(fullFilePath);

            fullFilePath += "\\" + szFileName;
            BinaryWriter writer = new BinaryWriter(File.Open(fullFilePath, FileMode.Create));

            
            writer.Write(UP                        );
            writer.Write(DOWN                      );
            writer.Write(LEFT                      );
            writer.Write(RIGHT                     );
            writer.Write(SQUARE                    );
            writer.Write(CROSS                     );
            writer.Write(CIRCLE                    );
            writer.Write(TRIANGLE                  );
            writer.Write(LTRIGGER                  );
            writer.Write(RTRIGGER                  );
            writer.Write(SELECT                    );
            writer.Write(START                     );
            writer.Write(LEFT_MOUSE                );
            writer.Write(LEFT_UP                   );
            writer.Write(LEFT_DOWN                 );
            writer.Write(LEFT_LEFT                 );
            writer.Write(LEFT_RIGHT                );
            writer.Write(RIGHT_MOUSE               );
            writer.Write(RIGHT_UP                  );
            writer.Write(RIGHT_DOWN                );
            writer.Write(RIGHT_LEFT                );
            writer.Write(RIGHT_RIGHT               );
            writer.Write(TRIGGERS_AS_MOUSE_CLICKS  );
            writer.Write(MOUSE_AUTO_LEFT           );
            writer.Write(MOUSE_AUTO_RIGHT          );
            writer.Write(ONE_TOUCH                 );
            writer.Write(TWO_TOUCH                 );
            writer.Write(ONE_TOUCH_BUFFER          );
            writer.Write(TWO_TOUCH_BUFFER          );
            writer.Write(ONE_TOUCH_MOVEMENT        );
            writer.Write(TWO_TOUCH_MOVEMENT        );
            writer.Write(MOUSE_RELATIVE_ONE        );
            writer.Write(MOUSE_RELATIVE_TWO        );
            writer.Write(ONE_DRAG_START            );
            writer.Write(ONE_DRAG_END              );
            writer.Write(ONE_TAP                   );
            writer.Write(TWO_DRAG_START            );
            writer.Write(TWO_DRAG_END              );
            writer.Write(TWO_TAP                   );

            writer.Flush();
            writer.Close();
        }

        public static void Load(string szFileName)
        {
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string fullFilePath = appdata + "\\" + szSubFolder + "\\" + szFileName;

            try
            {
                BinaryReader reader = new BinaryReader(File.Open(fullFilePath, FileMode.Open));

                UP                        = (uint)reader.ReadInt32();
                DOWN                      = (uint)reader.ReadInt32();
                LEFT                      = (uint)reader.ReadInt32();
                RIGHT                     = (uint)reader.ReadInt32();
                SQUARE                    = (uint)reader.ReadInt32();
                CROSS                     = (uint)reader.ReadInt32();
                CIRCLE                    = (uint)reader.ReadInt32();
                TRIANGLE                  = (uint)reader.ReadInt32();
                LTRIGGER                  = (uint)reader.ReadInt32();
                RTRIGGER                  = (uint)reader.ReadInt32();
                SELECT                    = (uint)reader.ReadInt32();
                START                     = (uint)reader.ReadInt32();
                LEFT_MOUSE                = reader.ReadBoolean();
                LEFT_UP                   = (uint)reader.ReadInt32();
                LEFT_DOWN                 = (uint)reader.ReadInt32();
                LEFT_LEFT                 = (uint)reader.ReadInt32();
                LEFT_RIGHT                = (uint)reader.ReadInt32();
                RIGHT_MOUSE               = reader.ReadBoolean();
                RIGHT_UP                  = (uint)reader.ReadInt32();
                RIGHT_DOWN                = (uint)reader.ReadInt32();
                RIGHT_LEFT                = (uint)reader.ReadInt32();
                RIGHT_RIGHT               = (uint)reader.ReadInt32();
                TRIGGERS_AS_MOUSE_CLICKS  = reader.ReadBoolean();
                MOUSE_AUTO_LEFT           = reader.ReadBoolean();
                MOUSE_AUTO_RIGHT          = reader.ReadBoolean();
                ONE_TOUCH                 = reader.ReadBoolean();
                TWO_TOUCH                 = reader.ReadBoolean();
                ONE_TOUCH_BUFFER          = reader.ReadBoolean();
                TWO_TOUCH_BUFFER          = reader.ReadBoolean();
                ONE_TOUCH_MOVEMENT        = (uint)reader.ReadInt32();
                TWO_TOUCH_MOVEMENT        = (uint)reader.ReadInt32();
                MOUSE_RELATIVE_ONE        = reader.ReadBoolean();
                MOUSE_RELATIVE_TWO        = reader.ReadBoolean();
                ONE_DRAG_START            = (uint)reader.ReadInt32();
                ONE_DRAG_END              = (uint)reader.ReadInt32();
                ONE_TAP                   = (uint)reader.ReadInt32();
                TWO_DRAG_START            = (uint)reader.ReadInt32();
                TWO_DRAG_END              = (uint)reader.ReadInt32();
                TWO_TAP                   = (uint)reader.ReadInt32();

                reader.Close();
            }
            catch
            {
                Console.WriteLine("Couldnt Open File");
            }
        }
 
    }
}
