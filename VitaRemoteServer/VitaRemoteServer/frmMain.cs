using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using VitaRemoteServer.Input;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System.Threading;
using System.Runtime.InteropServices;
using System.Net;

namespace VitaRemoteServer
{
    

    public partial class frmMain : Form
    {
        private NotifyIcon trayIcon;
        private ContextMenu trayMenu;
        private RunningClass running;
        private Thread ClassThread;

        private string windowText;
        private bool windowTextUpdate;

        private string curProfile;

        public frmMain()
        {
            InitializeComponent();
            running = new RunningClass();
            running.MainFrame = this;
            windowTextUpdate = false;
        }

        private void RunningThread( Object obj )
        {
            RunningClass run = (RunningClass)obj;
            run.Init();

            while (run.IsRunning)
            {
                run.Update();
                //run.Draw();
            }

            run.Close();
        }

        private void OnExit(object sender, EventArgs e)
        {
            running.IsRunning = false;
            Application.Exit();
        }

        public void frmMain_Load(object sender, EventArgs e)
        {

            ParameterizedThreadStart threadStart = new ParameterizedThreadStart(RunningThread);
            ClassThread = new Thread(threadStart);
            ClassThread.Start(running);


            trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add("Exit", OnExit);
            trayIcon = new NotifyIcon();
            trayIcon.Text = "PSV REMOTE SERVER";
            trayIcon.Icon = new Icon(SystemIcons.Application, 40, 40);
            trayIcon.ContextMenu = trayMenu;
            trayIcon.Visible = true;

            // load all the profile names
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string fullFilePath = appdata + "\\" + gamePadInput.szSubFolder;
            string[] profiles = Directory.GetFiles(fullFilePath).ToArray();
            bool createdDefault = false;

            // make sure to create the default
            if (profiles.Length == 0 || profiles.Contains(fullFilePath + "\\" + "default.dat") == false)
            {
                gamePadInput.Save("default.dat");
                curProfile = "default.dat";

                // push onto the list
                profiles = new string[profiles.Length + 1];
                profiles[profiles.Length - 1] = curProfile;

                createdDefault = true;
            }

            // crop each string to only be the filename
            foreach (string fileName in profiles)
            {
                int goodbyAmt = fileName.LastIndexOf('\\');
                string shortName = fileName.Remove(0, goodbyAmt + 1);

                if (shortName != "GlobalSettings.dat")
                {
                    profilesBox.Items.Add(shortName);
                }
            }

            
            // wait for thread to finish init
            while (running.IsRunning == false)
            {
                Thread.Sleep(1);
            }

            SetAreaTable(running.ResCount);
            SetKeyData();


            // open the last used profile 
            if (createdDefault || true)
            {
                try
                {
                    BinaryReader globalRead = new BinaryReader(File.Open(fullFilePath + "\\" + "GlobalSettings.dat", FileMode.Open));
                    int readAmt = globalRead.ReadInt32();
                    curProfile = new string(globalRead.ReadChars(readAmt));

                    tbResolution.Value = globalRead.ReadInt32();
                    tbArea.Value = globalRead.ReadInt32();
                    tbQuality.Value = globalRead.ReadInt32();
                    tbMouseSpeed.Value = globalRead.ReadInt32();

                    globalRead.Close();
                }
                catch
                {
                    Console.WriteLine("Couldnt open global settings");
                }
            }

            // select in the box the profile(if found)
            if (curProfile == null)
            {
                curProfile = "default.dat";
            }
            int profileIndex = profilesBox.Items.IndexOf(curProfile);
            if (profileIndex < 0)
            {
                profileIndex = profilesBox.Items.IndexOf("default.dat");
            }

            profilesBox.SelectedIndex = profileIndex;
            gamePadInput.Load(curProfile);

        }

        private void SaveGlobal()
        {
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string fullFilePath = appdata + "\\" + gamePadInput.szSubFolder;

            // write it
            BinaryWriter globalWrite = new BinaryWriter(File.Open(fullFilePath + "\\" + "GlobalSettings.dat", FileMode.Create));
            globalWrite.Write(curProfile.Length);
            globalWrite.Write(curProfile.ToCharArray());
            globalWrite.Write(tbResolution.Value);
            globalWrite.Write(tbArea.Value);
            globalWrite.Write(tbQuality.Value);
            globalWrite.Write(tbMouseSpeed.Value);

            globalWrite.Flush();
            globalWrite.Close();
        }

        private void OnExit()
        {
            SaveGlobal();
            gamePadInput.Save(curProfile);
            running.IsRunning = false;
            trayIcon.Dispose();
            trayMenu.Dispose();
        }

        private void tbResolution_Scroll(object sender, EventArgs e)
        {
            int quality = tbResolution.Maximum - (tbResolution.Value - 1);
            int baseX = 940 / quality;
            int baseY = 544 / quality;

            screenCapture.SetResolution(baseX, baseY, tbResolution.Value);
            lblRes.Text = "Resolution: " + baseX + " x " + baseY;

            /*
            screenCapture.Resolution = tbResolution.Value;
            if (tbResolution.Value == 2)
                lblRes.Text = "Resolution: 480 x 270";
            else
                lblRes.Text = "Resolution: 400 x 200";
             * */
            
        }

        private void tbArea_Scroll(object sender, EventArgs e)
        {
            Resolutions res = running.GetRes(tbArea.Value);
            screenCapture.SetArea(res.res_x, res.res_y, tbArea.Value);

            /*
            if (tbArea.Value == 1)
                lblScale.Text = "Capture Area: 400 x 200";
            if (tbArea.Value == 2)
                lblScale.Text = "Capture Area: 480 x 270";
            if (tbArea.Value == 3)
                lblScale.Text = "Capture Area: 640 x 480";
            if (tbArea.Value == 4
                lblScale.Text = "Capture Area: 720 x 576";
            if (tbArea.Value == 5)
                lblScale.Text = "Capture Area: 800 x 600";
             */
            lblScale.Text = res.ToString();
        }

        private void tbQuality_Scroll(object sender, EventArgs e)
        {
            screenCapture.Quality = tbQuality.Value;
            label1.Text = "Image Quality: " + tbQuality.Value + "%";
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            OnExit();
        }

        private void box_Up_SelectedIndexChanged(object sender, EventArgs e)
        {
            Keys key = RunningClass.GetKey(((ComboBox)sender).Text.ToString());           
            gamePadInput.UP = (uint)key;

        }

        private void box_Down_SelectedIndexChanged(object sender, EventArgs e)
        {
            Keys key = RunningClass.GetKey(((ComboBox)sender).Text.ToString());               
            gamePadInput.DOWN = (uint)key;
        }

        private void box_Left_SelectedIndexChanged(object sender, EventArgs e)
        {
            Keys key = RunningClass.GetKey(((ComboBox)sender).Text.ToString());               
            gamePadInput.LEFT = (uint)key;
        }

        private void box_Right_SelectedIndexChanged(object sender, EventArgs e)
        {
            Keys key = RunningClass.GetKey(((ComboBox)sender).Text.ToString());               
            gamePadInput.RIGHT = (uint)key;
        }

        private void box_Triangle_SelectedIndexChanged(object sender, EventArgs e)
        {
            Keys key = RunningClass.GetKey(((ComboBox)sender).Text.ToString());               
            gamePadInput.TRIANGLE = (uint)key;
        }

        private void box_Cross_SelectedIndexChanged(object sender, EventArgs e)
        {
            Keys key = RunningClass.GetKey(((ComboBox)sender).Text.ToString());               
            gamePadInput.CROSS = (uint)key;
        }

        private void box_Square_SelectedIndexChanged(object sender, EventArgs e)
        {
            Keys key = RunningClass.GetKey(((ComboBox)sender).Text.ToString());               
            gamePadInput.SQUARE = (uint)key;
        }

        private void box_Circle_SelectedIndexChanged(object sender, EventArgs e)
        {
            Keys key = RunningClass.GetKey(((ComboBox)sender).Text.ToString());               
            gamePadInput.CIRCLE = (uint)key;
        }

        private void box_L_SelectedIndexChanged(object sender, EventArgs e)
        {
            Keys key = RunningClass.GetKey(((ComboBox)sender).Text.ToString());               
            gamePadInput.LTRIGGER = (uint)key;
        }

        private void box_R_SelectedIndexChanged(object sender, EventArgs e)
        {
            Keys key = RunningClass.GetKey(((ComboBox)sender).Text.ToString());               
            gamePadInput.RTRIGGER = (uint)key;
        }

        private void box_Select_SelectedIndexChanged(object sender, EventArgs e)
        {
            Keys key = RunningClass.GetKey(((ComboBox)sender).Text.ToString());    
           gamePadInput.SELECT = (uint)key;
        }

        private void box_Start_SelectedIndexChanged(object sender, EventArgs e)
        {
            Keys key = RunningClass.GetKey(((ComboBox)sender).Text.ToString());               
            gamePadInput.START = (uint)key;
        }

        private void box_Analog_L_Up_SelectedIndexChanged(object sender, EventArgs e)
        {
            Keys key = RunningClass.GetKey(((ComboBox)sender).Text.ToString());
            gamePadInput.LEFT_UP = (uint)key;
        }

        private void box_Analog_L_Down_SelectedIndexChanged(object sender, EventArgs e)
        {
            Keys key = RunningClass.GetKey(((ComboBox)sender).Text.ToString());
            gamePadInput.LEFT_DOWN = (uint)key;
        }

        private void box_Analog_L_Left_SelectedIndexChanged(object sender, EventArgs e)
        {
            Keys key = RunningClass.GetKey(((ComboBox)sender).Text.ToString());
            gamePadInput.LEFT_LEFT = (uint)key;
        }

        private void box_Analog_L_Right_SelectedIndexChanged(object sender, EventArgs e)
        {
            Keys key = RunningClass.GetKey(((ComboBox)sender).Text.ToString());
            gamePadInput.LEFT_RIGHT = (uint)key;
        }

        private void box_Analog_R_Up_SelectedIndexChanged(object sender, EventArgs e)
        {
            Keys key = RunningClass.GetKey(((ComboBox)sender).Text.ToString());
            gamePadInput.RIGHT_UP = (uint)key;
        }

        private void box_Analog_R_Down_SelectedIndexChanged(object sender, EventArgs e)
        {
            Keys key = RunningClass.GetKey(((ComboBox)sender).Text.ToString());
            gamePadInput.RIGHT_DOWN = (uint)key;
        }

        private void box_Analog_R_Left_SelectedIndexChanged(object sender, EventArgs e)
        {
            Keys key = RunningClass.GetKey(((ComboBox)sender).Text.ToString());
            gamePadInput.RIGHT_LEFT = (uint)key;
        }

        private void box_Analog_R_Right_SelectedIndexChanged(object sender, EventArgs e)
        {
            Keys key = RunningClass.GetKey(((ComboBox)sender).Text.ToString());
            gamePadInput.RIGHT_RIGHT = (uint)key;
        }
        
        private void tabPage1_Enter(object sender, EventArgs e)
        {
            // mouse
            Check_Mouse_Triggers.Checked = gamePadInput.TRIGGERS_AS_MOUSE_CLICKS;
            check_Auto_Left.Checked = gamePadInput.MOUSE_AUTO_LEFT;
            check_Auto_Right.Checked = gamePadInput.MOUSE_AUTO_RIGHT;

            // singles checks and radios
            radMouseOne.Checked = gamePadInput.ONE_TOUCH_MOVEMENT == 1;
            radViewOne.Checked = gamePadInput.ONE_TOUCH_MOVEMENT == 2;
            Check_Mouse_Relative_Single.Enabled = radMouseOne.Checked;
            Check_Mouse_Relative_Single.Checked = gamePadInput.MOUSE_RELATIVE_ONE;

            // doubles checks and radios
            radMouseTwo.Checked = gamePadInput.TWO_TOUCH_MOVEMENT == 1;
            radViewTwo.Checked = gamePadInput.TWO_TOUCH_MOVEMENT == 2;
            Check_Mouse_Relative_Double.Enabled = radMouseTwo.Checked;
            Check_Mouse_Relative_Double.Checked = gamePadInput.MOUSE_RELATIVE_TWO;

            // the combos
            uint temp_Single_Start = gamePadInput.ONE_DRAG_START;
            uint temp_Single_End = gamePadInput.ONE_DRAG_END;
            uint temp_Single_Tap = gamePadInput.ONE_TAP;
            uint temp_Double_Start = gamePadInput.TWO_DRAG_START;
            uint temp_Double_End = gamePadInput.TWO_DRAG_END;
            uint temp_Double_Tap = gamePadInput.TWO_TAP;


            box_DragStart_One.SelectedItem = Enum.ToObject(typeof(MouseButtons), temp_Single_Start);
            box_DragEnd_One.SelectedItem = Enum.ToObject(typeof(MouseButtons), temp_Single_End);
            box_Tap_One.SelectedItem = Enum.ToObject(typeof(MouseButtons), temp_Single_Tap);
            box_Tap_Two.SelectedItem = Enum.ToObject(typeof(MouseButtons), temp_Double_Tap);

            tbResolution_Scroll(null, null);
            tbArea_Scroll(null, null);
            tbQuality_Scroll(null, null);

        }

        private void tabPage2_Enter(object sender, EventArgs e)
        {
            uint temp_UP = gamePadInput.UP;
            uint temp_DOWN = gamePadInput.DOWN;
            uint temp_LEFT = gamePadInput.LEFT;
            uint temp_RIGHT = gamePadInput.RIGHT;
            uint temp_SQUARE = gamePadInput.SQUARE;
            uint temp_CROSS = gamePadInput.CROSS;
            uint temp_CIRCLE = gamePadInput.CIRCLE;
            uint temp_TRIANGLE = gamePadInput.TRIANGLE;
            uint temp_LTRIGGER = gamePadInput.LTRIGGER;
            uint temp_RTRIGGER = gamePadInput.RTRIGGER;
            uint temp_SELECT = gamePadInput.SELECT;
            uint temp_START = gamePadInput.START;
            
            box_Up.SelectedItem =  Enum.ToObject(typeof(Keys), temp_UP);
            box_Down.SelectedItem = Enum.ToObject(typeof(Keys), temp_DOWN);
            box_Left.SelectedItem = Enum.ToObject(typeof(Keys), temp_LEFT);
            box_Right.SelectedItem = Enum.ToObject(typeof(Keys), temp_RIGHT);
            box_Square.SelectedItem = Enum.ToObject(typeof(Keys), temp_SQUARE);
            box_Cross.SelectedItem = Enum.ToObject(typeof(Keys), temp_CROSS);
            box_Circle.SelectedItem = Enum.ToObject(typeof(Keys), temp_CIRCLE);
            box_Triangle.SelectedItem = Enum.ToObject(typeof(Keys), temp_TRIANGLE);
            box_L.SelectedItem = Enum.ToObject(typeof(Keys), temp_LTRIGGER);
            box_R.SelectedItem = Enum.ToObject(typeof(Keys), temp_RTRIGGER);
            box_Select.SelectedItem = Enum.ToObject(typeof(Keys), temp_SELECT);
            box_Start.SelectedItem = Enum.ToObject(typeof(Keys), temp_START);
        }

        private void tabPage3_Enter(object sender, EventArgs e)
        {
            uint temp_LEFT_UP = gamePadInput.LEFT_UP;
            uint temp_LEFT_DOWN = gamePadInput.LEFT_DOWN;
            uint temp_LEFT_LEFT = gamePadInput.LEFT_LEFT;
            uint temp_LEFT_RIGHT = gamePadInput.LEFT_RIGHT;
            uint temp_RIGHT_UP = gamePadInput.RIGHT_UP;
            uint temp_RIGHT_DOWN = gamePadInput.RIGHT_DOWN;
            uint temp_RIGHT_LEFT = gamePadInput.RIGHT_LEFT;
            uint temp_RIGHT_RIGHT = gamePadInput.RIGHT_RIGHT;


            box_Analog_L_Up.SelectedItem = Enum.ToObject(typeof(Keys), temp_LEFT_UP);
            box_Analog_L_Down.SelectedItem = Enum.ToObject(typeof(Keys), temp_LEFT_DOWN);
            box_Analog_L_Left.SelectedItem = Enum.ToObject(typeof(Keys), temp_LEFT_LEFT);
            box_Analog_L_Right.SelectedItem = Enum.ToObject(typeof(Keys), temp_LEFT_RIGHT);
            box_Analog_R_Up.SelectedItem = Enum.ToObject(typeof(Keys), temp_RIGHT_UP);
            box_Analog_R_Down.SelectedItem = Enum.ToObject(typeof(Keys), temp_RIGHT_DOWN);
            box_Analog_R_Left.SelectedItem = Enum.ToObject(typeof(Keys), temp_RIGHT_LEFT);
            box_Analog_R_Right.SelectedItem = Enum.ToObject(typeof(Keys), temp_RIGHT_RIGHT);
            
            LeftMouseCheck.Checked = gamePadInput.LEFT_MOUSE;
            RightMouseCheck.Checked = gamePadInput.RIGHT_MOUSE;
        }

        private void leftMouseCheck_CheckedChanged(object sender, EventArgs e)
        {
            gamePadInput.LEFT_MOUSE = LeftMouseCheck.Checked;

            // make sure other mouse isnt checked if we just checked
            if (gamePadInput.LEFT_MOUSE)
            {
                gamePadInput.RIGHT_MOUSE = RightMouseCheck.Checked = false;
            }
        }

        private void RightMouseCheck_CheckedChanged(object sender, EventArgs e)
        {
            gamePadInput.RIGHT_MOUSE = RightMouseCheck.Checked;

            // make sure other mouse isnt checked if we just checked
            if (gamePadInput.RIGHT_MOUSE)
            {
                gamePadInput.LEFT_MOUSE = LeftMouseCheck.Checked = false;
            }
        }

        private void Check_Mouse_Relative_Single_CheckedChanged(object sender, EventArgs e)
        {
            gamePadInput.MOUSE_RELATIVE_ONE = Check_Mouse_Relative_Single.Checked;
            running.sendRelativeDragOne(Check_Mouse_Relative_Single.Checked);
        }

        private void Check_Mouse_Relative_Double_CheckedChanged(object sender, EventArgs e)
        {
            gamePadInput.MOUSE_RELATIVE_TWO = Check_Mouse_Relative_Double.Checked;
            running.sendRelativeDragTwo(Check_Mouse_Relative_Double.Checked);
        }

        private void check_Auto_Left_CheckedChanged(object sender, EventArgs e)
        {
            gamePadInput.MOUSE_AUTO_LEFT = check_Auto_Left.Checked;

            // make sure other mouse isnt checked if we just checked
            if (gamePadInput.MOUSE_AUTO_LEFT)
            {
                gamePadInput.MOUSE_AUTO_RIGHT = check_Auto_Right.Checked = false;
            }
        }

        private void check_Auto_Right_CheckedChanged(object sender, EventArgs e)
        {
            gamePadInput.MOUSE_AUTO_RIGHT = check_Auto_Right.Checked;

            // make sure other mouse isnt checked if we just checked
            if (gamePadInput.MOUSE_AUTO_RIGHT)
            {
                gamePadInput.MOUSE_AUTO_LEFT = check_Auto_Left.Checked = false;
            }
        }

        private void RadioOne_CheckedChanged(object sender, EventArgs e)
        {
            if(radMouseOne.Checked)
            {
                gamePadInput.ONE_TOUCH_MOVEMENT = 1;
            }
            else if (radViewOne.Checked)
            {
                gamePadInput.ONE_TOUCH_MOVEMENT = 2;
            }
            else
            {
                // somethign bad happened to get here
                //throw new NoNullAllowedException();
            }
            Check_Mouse_Relative_Single.Checked = gamePadInput.MOUSE_RELATIVE_ONE = radViewOne.Checked;
            Check_Mouse_Relative_Single.Enabled = radMouseOne.Checked;
            Check_Mouse_Relative_Single_CheckedChanged(sender, e);

        }

        private void RadioTwo_CheckedChanged(object sender, EventArgs e)
        {
            if (radMouseTwo.Checked)
            {
                gamePadInput.TWO_TOUCH_MOVEMENT = 1;
            }
            else if (radViewTwo.Checked)
            {
                gamePadInput.TWO_TOUCH_MOVEMENT = 2;
            }
            else
            {
                // somethign bad happened to get here
                //throw new NoNullAllowedException();
            }
            Check_Mouse_Relative_Double.Checked = gamePadInput.MOUSE_RELATIVE_TWO = radViewTwo.Checked;
            Check_Mouse_Relative_Double.Enabled = radMouseTwo.Checked;
            Check_Mouse_Relative_Double_CheckedChanged(sender, e);
        }

        private void box_DragStart_One_SelectedIndexChanged(object sender, EventArgs e)
        {
            MouseButtons butt = RunningClass.GetButt(((ComboBox)sender).Text.ToString());
            gamePadInput.ONE_DRAG_START = (uint)butt;
        }

        private void box_DragEnd_One_SelectedIndexChanged(object sender, EventArgs e)
        {
            MouseButtons butt = RunningClass.GetButt(((ComboBox)sender).Text.ToString());
            gamePadInput.ONE_DRAG_END = (uint)butt;
        }

        private void box_Tap_One_SelectedIndexChanged(object sender, EventArgs e)
        {
            MouseButtons butt = RunningClass.GetButt(((ComboBox)sender).Text.ToString());
            gamePadInput.ONE_TAP = (uint)butt;
        }

        private void box_DragStart_Two_SelectedIndexChanged(object sender, EventArgs e)
        {
            MouseButtons butt = RunningClass.GetButt(((ComboBox)sender).Text.ToString());
            gamePadInput.TWO_DRAG_START = (uint)butt;
        }

        private void box_DragEnd_Two_SelectedIndexChanged(object sender, EventArgs e)
        {
            MouseButtons butt = RunningClass.GetButt(((ComboBox)sender).Text.ToString());
            gamePadInput.TWO_DRAG_END = (uint)butt;
        }

        private void box_Tap_Two_SelectedIndexChanged(object sender, EventArgs e)
        {
            MouseButtons butt = RunningClass.GetButt(((ComboBox)sender).Text.ToString());
            gamePadInput.TWO_TAP = (uint)butt;
        }

        private void Check_Mouse_Triggers_CheckedChanged(object sender, EventArgs e)
        {
            gamePadInput.TRIGGERS_AS_MOUSE_CLICKS = Check_Mouse_Triggers.Checked;
        }


        public void SetAreaTable(int resCount)
        {
            lock (this)
            {
                tbArea.Minimum = 0;
                tbArea.Maximum = resCount - 1;
                tbArea.Value = resCount / 2;
                screenCapture.Area = tbArea.Value;
            }
        }
        public void SetKeyData()
        {
            // save the default values
            uint temp_UP = gamePadInput.UP;
            uint temp_DOWN = gamePadInput.DOWN;
            uint temp_LEFT = gamePadInput.LEFT;
            uint temp_RIGHT = gamePadInput.RIGHT;
            uint temp_SQUARE = gamePadInput.SQUARE;
            uint temp_CROSS = gamePadInput.CROSS;
            uint temp_CIRCLE = gamePadInput.CIRCLE;
            uint temp_TRIANGLE = gamePadInput.TRIANGLE;
            uint temp_LTRIGGER = gamePadInput.LTRIGGER;
            uint temp_RTRIGGER = gamePadInput.RTRIGGER;
            uint temp_SELECT = gamePadInput.SELECT;
            uint temp_START = gamePadInput.START;
            uint temp_LEFT_UP = gamePadInput.LEFT_UP;
            uint temp_LEFT_DOWN = gamePadInput.LEFT_DOWN;
            uint temp_LEFT_LEFT = gamePadInput.LEFT_LEFT;
            uint temp_LEFT_RIGHT = gamePadInput.LEFT_RIGHT;
            uint temp_RIGHT_UP = gamePadInput.RIGHT_UP;
            uint temp_RIGHT_DOWN = gamePadInput.RIGHT_DOWN;
            uint temp_RIGHT_LEFT = gamePadInput.RIGHT_LEFT;
            uint temp_RIGHT_RIGHT = gamePadInput.RIGHT_RIGHT;
            uint temp_Single_Start = gamePadInput.ONE_DRAG_START;
            uint temp_Single_End = gamePadInput.ONE_DRAG_END;
            uint temp_Single_Tap = gamePadInput.ONE_TAP;
            uint temp_Double_Start = gamePadInput.TWO_DRAG_START;
            uint temp_Double_End = gamePadInput.TWO_DRAG_END;
            uint temp_Double_Tap = gamePadInput.TWO_TAP;

            // setup all the input combo boxes
            lock (this)
            {
                box_L.DataSource = System.Enum.GetValues(typeof(Keys));
                box_R.DataSource = System.Enum.GetValues(typeof(Keys));
                box_Up.DataSource = System.Enum.GetValues(typeof(Keys));
                box_Down.DataSource = System.Enum.GetValues(typeof(Keys));
                box_Left.DataSource = System.Enum.GetValues(typeof(Keys));
                box_Right.DataSource = System.Enum.GetValues(typeof(Keys));
                box_Triangle.DataSource = System.Enum.GetValues(typeof(Keys));
                box_Cross.DataSource = System.Enum.GetValues(typeof(Keys));
                box_Square.DataSource = System.Enum.GetValues(typeof(Keys));
                box_Circle.DataSource = System.Enum.GetValues(typeof(Keys));
                box_Select.DataSource = System.Enum.GetValues(typeof(Keys));
                box_Start.DataSource = System.Enum.GetValues(typeof(Keys));
                box_Analog_L_Up.DataSource = System.Enum.GetValues(typeof(Keys));
                box_Analog_L_Down.DataSource = System.Enum.GetValues(typeof(Keys));
                box_Analog_L_Left.DataSource = System.Enum.GetValues(typeof(Keys));
                box_Analog_L_Right.DataSource = System.Enum.GetValues(typeof(Keys));
                box_Analog_R_Up.DataSource = System.Enum.GetValues(typeof(Keys));
                box_Analog_R_Down.DataSource = System.Enum.GetValues(typeof(Keys));
                box_Analog_R_Left.DataSource = System.Enum.GetValues(typeof(Keys));
                box_Analog_R_Right.DataSource = System.Enum.GetValues(typeof(Keys));
                box_DragStart_One.DataSource = System.Enum.GetValues(typeof(MouseButtons));
                box_DragEnd_One.DataSource = System.Enum.GetValues(typeof(MouseButtons));
                box_Tap_One.DataSource = System.Enum.GetValues(typeof(MouseButtons));
                box_Tap_Two.DataSource = System.Enum.GetValues(typeof(MouseButtons));
            }
            gamePadInput.UP = temp_UP;
            gamePadInput.DOWN = temp_DOWN;
            gamePadInput.LEFT = temp_LEFT;
            gamePadInput.RIGHT = temp_RIGHT;
            gamePadInput.SQUARE = temp_SQUARE;
            gamePadInput.CROSS = temp_CROSS;
            gamePadInput.CIRCLE = temp_CIRCLE;
            gamePadInput.TRIANGLE = temp_TRIANGLE;
            gamePadInput.LTRIGGER = temp_LTRIGGER;
            gamePadInput.RTRIGGER = temp_RTRIGGER;
            gamePadInput.SELECT = temp_SELECT;
            gamePadInput.START = temp_START;
            gamePadInput.LEFT_UP = temp_LEFT_UP;
            gamePadInput.LEFT_DOWN = temp_LEFT_DOWN;
            gamePadInput.LEFT_LEFT = temp_LEFT_LEFT;
            gamePadInput.LEFT_RIGHT = temp_LEFT_RIGHT;
            gamePadInput.RIGHT_UP = temp_RIGHT_UP;
            gamePadInput.RIGHT_DOWN = temp_RIGHT_DOWN;
            gamePadInput.RIGHT_LEFT = temp_RIGHT_LEFT;
            gamePadInput.RIGHT_RIGHT = temp_RIGHT_RIGHT;
            gamePadInput.ONE_DRAG_START = temp_Single_Start;
            gamePadInput.ONE_DRAG_END = temp_Single_End;
            gamePadInput.ONE_TAP = temp_Single_Tap;
            gamePadInput.TWO_DRAG_START = temp_Double_Start;
            gamePadInput.TWO_DRAG_END = temp_Double_End;
            gamePadInput.TWO_TAP = temp_Double_Tap;

        }

        public void SetWindowText(string szText)
        {
            lock (this)
            {
                if (windowTextUpdate == false)
                {
                    windowTextUpdate = true;
                    windowText = szText;
                }
            }
        }

        // this function will get called once a 1000 ms
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (windowTextUpdate)
            {
                this.Text = windowText;
                windowTextUpdate = false;
            }
        }

        private void ResetGUI()
        {
            SetKeyData();
            tabPage1_Enter(null, null);
            tabPage2_Enter(null, null);
            tabPage3_Enter(null, null);
        }

        private void profilesBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string szProfile = profilesBox.SelectedItem.ToString();
            gamePadInput.Save(curProfile);
            gamePadInput.Load(szProfile);

            ResetGUI();

            curProfile = szProfile;
        }

        private void newProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EnterTextDialog text = new EnterTextDialog();
            text.ShowDialog(this);

            if (text.OK)
            {
                string szProfile = text.Profile;
                gamePadInput.Save(curProfile);
                gamePadInput.Save(szProfile);

                profilesBox.Items.Add(szProfile);
                
                curProfile = szProfile;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void tbMouseSpeed_Scroll(object sender, EventArgs e)
        {
            label2.Text = "Mouse Speed: " + tbMouseSpeed.Value;
            ReceivePacket.MouseSpeed = tbMouseSpeed.Value;
        }


    }
}

