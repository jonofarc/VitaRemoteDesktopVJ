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

namespace VitaRemoteServer
{
    public partial class frmMain : Form
    {
        
        private TCPConnection server;

        private NotifyIcon trayIcon;
        private ContextMenu trayMenu;
        private static double mFps;
        private static long mFrame = 0;
        private static long mT = 0;
        private static long mT0 = 0;
        private static byte[] boundary;
        private static Stopwatch stopWatch = new Stopwatch();
        private Thread mouse;

        private void updateMouse()
        {
            while (true)
            {
                ReceivePacket.updateMouse();
                Thread.Sleep(20);
                //Debug.WriteLine(mouse.ThreadState.ToString());
            }
        }

        public frmMain()
        {
            InitializeComponent();

            trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add("Exit", OnExit);
            trayIcon = new NotifyIcon();
            trayIcon.Text = "PSV REMOTE SERVER";
            trayIcon.Icon = new Icon(SystemIcons.Application, 40, 40);
            trayIcon.ContextMenu = trayMenu;
            trayIcon.Visible = true;

            mouse = new Thread(updateMouse);
            mouse.Start();
            
        }
        private void OnExit(object sender, EventArgs e)
        {
            trayIcon.Dispose();
            trayMenu.Dispose();
            Application.Exit();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {

            screenCapture.Area = 2;
            server = new TCPConnection();

            server.localPort = 8080;
            server.listen();
            screenCapture.init();

            screenCapture.Resolution = 50;
            screenCapture.Quality = 50;
            screenCapture.Area = 20;
            screenCapture.X = 0;
            screenCapture.Y = 0;
            stopWatch.Start();

            boundary = System.Text.ASCIIEncoding.ASCII.GetBytes("--boundary");
            //deskTopHwnd = screenCapture.GetDesktopWindow();
        }

        void server_Connected(object sender, EventArgs e)
        {
            MessageBox.Show("Connected");
            throw new NotImplementedException();
        }

        private void sendImage()
        {
            if (server.isConnected && server.Ready)
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
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            sendImage();

            mFrame++;
            mT = stopWatch.ElapsedMilliseconds;
            if (mT - mT0 >= 1000)
            {
                float seconds = (mT - mT0) / 1000;
                mFps = mFrame / seconds;
                mT0 = mT;
                mFrame = 0;
            }

            this.Text = "FPS: " + mFps.ToString();
            //label2.Text = screenCapture.ScaleWidth.ToString() + "  " + screenCapture.ScaleHeight.ToString();
            //label3.Text = Input.gamePadInput.PSV_RIGHT_ANALOGY.ToString();
        }

        private void tbResolution_Scroll(object sender, EventArgs e)
        {
            screenCapture.Resolution = tbResolution.Value;
            if (tbResolution.Value == 2)
                lblRes.Text = "Resolution: 480 x 270";
            else
                lblRes.Text = "Resolution: 400 x 200";
            
        }

        private void tbArea_Scroll(object sender, EventArgs e)
        {
            screenCapture.Area = tbArea.Value;
            if (tbArea.Value == 1)
                lblScale.Text = "Capture Area: 400 x 200";
            if (tbArea.Value == 2)
                lblScale.Text = "Capture Area: 480 x 270";
            if (tbArea.Value == 3)
                lblScale.Text = "Capture Area: 640 x 480";
            if (tbArea.Value == 4)
                lblScale.Text = "Capture Area: 720 x 576";
            if (tbArea.Value == 5)
                lblScale.Text = "Capture Area: 800 x 600";
        }

        private void tbQuality_Scroll(object sender, EventArgs e)
        {
            screenCapture.Quality = tbQuality.Value;
            label1.Text = "Image Quality: " + tbQuality.Value + "%";
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

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            trayIcon.Dispose();
            trayMenu.Dispose();
            server.Disconnect();
            mouse.Abort();
        }

    }
}

