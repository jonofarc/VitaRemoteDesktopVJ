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

    // this class is going to handle all the data that the main form changes
    class RunningClass
    {
        #region DEVMODE_STUFF

        [DllImport("user32.dll")]
        public static extern bool EnumDisplaySettings(
              string deviceName, int modeNum, ref DEVMODE devMode);
        const int ENUM_CURRENT_SETTINGS = -1;

        const int ENUM_REGISTRY_SETTINGS = -2;

        [StructLayout(LayoutKind.Sequential)]
        public struct DEVMODE
        {

            private const int CCHDEVICENAME = 0x20;
            private const int CCHFORMNAME = 0x20;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public int dmPositionX;
            public int dmPositionY;
            public ScreenOrientation dmDisplayOrientation;
            public int dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmFormName;
            public short dmLogPixels;
            public int dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
            public int dmICMMethod;
            public int dmICMIntent;
            public int dmMediaType;
            public int dmDitherType;
            public int dmReserved1;
            public int dmReserved2;
            public int dmPanningWidth;
            public int dmPanningHeight;

        };

        #endregion

        

        private List<Resolutions> mResolutions;

        private TCPConnection server;

        private static double mFps;
        private static long mFrame = 0;
        private static long mT = 0;
        private static long mT0 = 0;
        private static byte[] boundary;
        private static Stopwatch stopWatch = new Stopwatch();
        private Thread mouse;
        
        private static string serverPort;
        private volatile bool running;
        private volatile bool initialized;

        private frmMain mainFrame;

        #region Properties

        public int ResCount
        {
            get { return mResolutions.Count; }
        }

        public frmMain MainFrame
        {
            get { return mainFrame; }
            set { lock (this) { mainFrame = value; } }
        }
        public TCPConnection Server
        {
            get { return server; }
        }
        public bool IsRunning
        {
            get { return running; }
            set { running = value; }
        }

        #endregion
        public Resolutions GetRes(int index)
        {
            while (!initialized)
            {
                Thread.Sleep(1);
            }
            Resolutions res;
            lock (mResolutions)
            {
                res = mResolutions[index];
            }
            return res;
        }

        // constructor
        public RunningClass()
        {
            initialized = false;
        }

        // call to setup everything
        public void Init()
        {
            screenCapture.Area = 2;
            server = new TCPConnection();

            server.localPort = 8080;
            server.listen();
            screenCapture.init();

            screenCapture.MainFrame = mainFrame;
            screenCapture.Resolution = 50;
            screenCapture.Quality = 50;
            screenCapture.Area = 20;
            screenCapture.X = 0;
            screenCapture.Y = 0;

            // show the IP and port
            string sHostName = Dns.GetHostName();
            IPHostEntry ipEntry = Dns.GetHostEntry(sHostName);
            IPAddress[] IpAddr = ipEntry.AddressList;

            int smallestIndex = 0;
            for (int ipIndex = 1; ipIndex < IpAddr.Length; ++ipIndex)
            {
                if (IpAddr[smallestIndex].ToString().Length > IpAddr[ipIndex].ToString().Length)
                {
                    smallestIndex = ipIndex;
                }
            }

            serverPort = IpAddr[smallestIndex] + ":" + server.localPort;

            stopWatch.Reset();
            stopWatch.Start();

            boundary = System.Text.ASCIIEncoding.ASCII.GetBytes("--boundary");
            // init all my variables and data structures
            mResolutions = new List<Resolutions>();

            // get all available resolutions from the display
            DEVMODE vDev = new DEVMODE();
            int resCount = 0;
            while (EnumDisplaySettings(null, resCount, ref vDev))
            {

                Resolutions newRez = new Resolutions();
                newRez.res_x = vDev.dmPelsWidth;
                newRez.res_y = vDev.dmPelsHeight;

                bool bDupe = false;
                foreach (Resolutions oldRez in mResolutions)
                {
                    if (oldRez == newRez)
                    {
                        bDupe = true;
                        break;
                    }
                }

                if (bDupe == false)
                {
                    mResolutions.Add(newRez);
                }

                resCount++;
            }

            // set the form data
            resCount = mResolutions.Count;
           // mainFrame.SetAreaTable(resCount);
           // mainFrame.SetKeyData();          

            // start the mousey thread
            //mouse = new Thread(updateMouse);
            //mouse.Start();

            running = true;
            initialized = true;
        }

        // call to close everything
        public void Close()
        {
            server.Disconnect();
        }

        // call once per frame to process all the data
            static int count = 0;
        public void Update()
        {
            if (server.isConnected)
            {
                UpdateImage();
                ReceivePacket.updateMouse();
            }
        }

        // call to render stuff
        public void Draw()
        {
            throw new NotImplementedException();
        }


        // other thread(if needed)
        private void updateMouse()
        //private void OnThread()
        {            
            while (running)
            {
                ReceivePacket.updateMouse();
                Thread.Sleep(1);
                //Debug.WriteLine(mouse.ThreadState.ToString());
            }
        }

        #region specific_functions

        
        void server_Connected(object sender, EventArgs e)
        {
            MessageBox.Show("Connected");
            throw new NotImplementedException();
        }

        private void sendImage()
        {
            if (server != null && server.isConnected && server.Ready)
            {
                MemoryStream ms1 = new MemoryStream();
                MemoryStream ms2 = new MemoryStream();
                MemoryStream ms3 = new MemoryStream();
                int i = screenCapture.ScreenShot(ref ms1, ref ms2);

                ms3.Write(ms1.ToArray(), 0, (int)ms1.Length);
                ms3.Write(boundary, 0, 10);
                ms3.Write(ms2.ToArray(), 0, (int)ms2.Length);

                SendPacket.sendImage(server, ms3.ToArray());
                //SendPacket.sendImage2(server, ms2.ToArray());

                //server.Ready = false;
                ms1.Dispose();
                ms2.Dispose();

                //ReceivePacket.updateMouse();
                //Console.WriteLine(count.ToString());
                count++;
            }
        }

        public void UpdateImage()
        {

            sendImage();

            mFrame++;
            double dbTime = (double)stopWatch.ElapsedMilliseconds / 1000.0;
            if (dbTime > 1.0)
            {
                mFps = mFrame / dbTime;                
                mainFrame.SetWindowText("FPS: " + mFps.ToString() + " - " + serverPort);

                stopWatch.Reset();
                stopWatch.Start();
                mFrame = 0;
            }

            //label2.Text = screenCapture.ScaleWidth.ToString() + "  " + screenCapture.ScaleHeight.ToString();
            //label3.Text = Input.gamePadInput.PSV_RIGHT_ANALOGY.ToString();
        }


        public static Keys GetKey(string szEnum)
        {
            // check if a number
            if (szEnum == "0" || szEnum == "1" || szEnum == "2" || szEnum == "3" || szEnum == "4" ||
                szEnum == "5" || szEnum == "6" || szEnum == "7" || szEnum == "8" || szEnum == "9")
            {
                szEnum = "D" + szEnum;
            }

            // check for "pg--"
            if (szEnum == "PgUp")
            {
                szEnum = "PageUp";
            }
            if (szEnum == "PgDn")
            {
                szEnum = "PageDown";
            }

            // delete
            if (szEnum == "Del")
            {
                szEnum = "Delete";
            }

            // insert
            if (szEnum == "Ins")
            {
                szEnum = "Insert";
            }

            // odd ones
            if (szEnum == "Shift+None")
            {
                szEnum = "Shift";
            }
            if (szEnum == "Ctrl+None")
            {
                szEnum = "Control";
            }
            if (szEnum == "Alt+None")
            {
                szEnum = "Alt";
            }
            if (szEnum == "Ctrl+Alt+Shift+None")
            {
                szEnum = "Snapshot";
            }

            return (Keys)Enum.Parse(typeof(Keys), szEnum.ToString());
        }
        public static MouseButtons GetButt(string szEnum)
        {
            return (MouseButtons)Enum.Parse(typeof(MouseButtons), szEnum.ToString());
        }

        public MemoryStream CreateToMemoryStream(MemoryStream memStreamIn, string zipEntryName)
        {

            MemoryStream outputMemStream = new MemoryStream();
            ZipOutputStream zipStream = new ZipOutputStream(outputMemStream);

            zipStream.SetLevel(9); //0-9, 9 being the highest level of compression

            ZipEntry newEntry = new ZipEntry(zipEntryName);
            newEntry.DateTime = DateTime.Now;

            zipStream.PutNextEntry(newEntry);

            zipStream.Write(memStreamIn.ToArray(), 0, (int)memStreamIn.Length);
            zipStream.CloseEntry();

            zipStream.IsStreamOwner = false;	// False stops the Close also Closing the underlying stream.
            zipStream.Close();			// Must finish the ZipOutputStream before using outputMemStream.

            outputMemStream.Position = 0;
            return outputMemStream;
        }

        public void sendRelativeDragOne(bool bRelative)
        {
            SendPacket.sendRelativeDragOne(Server, bRelative);
        }
        public void sendRelativeDragTwo(bool bRelative)
        {
            SendPacket.sendRelativeDragTwo(Server, bRelative);
        }


        #endregion
    }
}
