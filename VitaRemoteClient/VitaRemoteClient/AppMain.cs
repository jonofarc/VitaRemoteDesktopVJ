using System;
using System.Collections.Generic;
using System.Threading;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Input;
using System.IO;
using System.Reflection;
using Sce.PlayStation.HighLevel.UI;
using System.Diagnostics;


namespace VitaRemoteClient
{
	public class AppMain
	{
		
		private static GraphicsContext graphics;
		public static MainUI sceneMain;
		public static TCPConnection client;
		
		private static dialog.DLG_CONNECTION dgl_connection;
		
		private static Stopwatch stopWatch;
		
		private static Texture2D texture1;
		private static Texture2D texture2;
		
		private static double       mFps;
		private static long        mFrame = 0;
		private static long        mT = 0;
		private static long        mT0 = 0;
		
		private static bool loop = true;
		
		public static void Main (string[] args)
		{
			Initialize ();
			stopWatch.Start();
			while (loop) {
				//Thread.Sleep(10);
				
				SystemEvents.CheckEvents ();
				Update ();
				Render ();
				
#region "Calculation for frame rate"
				mFrame++;
			    mT = stopWatch.ElapsedMilliseconds ;
			    if (mT - mT0 >= 1000) {
				 float seconds = (mT - mT0)/1000;
				 mFps = mFrame / seconds; 
				 mT0    = mT;
				 mFrame = 0;
				}
#endregion 
			}
			
			term();
		}
		
		public static void Initialize ()
		{
			// Set up the graphics system
			graphics = new GraphicsContext();
			
			Draw.init(graphics);
			dgl_connection = new dialog.DLG_CONNECTION();
		
			UISystem.Initialize(graphics);
			
			sceneMain = new MainUI();
			UISystem.SetScene(sceneMain, null);
			
			stopWatch = new Stopwatch();
			texture1 = new Texture2D(940, 544, false,PixelFormat.Rgba);
			texture2 = new Texture2D(940, 544, false,PixelFormat.Rgba);
			
			// Set up Client
			client = new TCPConnection("192.168.1.110",8080);
			SendPacket.Init(client);
			
			
		}
		
		public static void Update ()
		{
			// Query gamepad for current state
			//var gamePadData = GamePad.GetData (0);
			
			switch(client.StatusType)
			{
			case Status.kNone:
					if(dgl_connection.Visible != true)
						dgl_connection.Show();
				break;
			case Status.kDisconnected:
				if(dgl_connection.Visible != true)
						dgl_connection.Show();
				break;
			default:
				break;
			}
			
			List<TouchData> touchDataList = Touch.GetData (0);
			Frame.TC = touchDataList.Count;
				
			UISystem.Update(touchDataList);

		}
		
		private static void term()
		{
			graphics.Dispose();
			UISystem.Terminate();
			client.Disconnect();
		}
		public static void Render ()
		{
			
			
			// Clear the screen
			graphics.SetClearColor (0.0f, 0.0f, 0.0f, 0.0f);
			graphics.Clear ();

			Input.UpdateGamepadState();
			client.updateImage();
			
			drawDesktop();
			sceneMain.updateLabel("FPSJON: " + mFps.ToString());

			UISystem.Render();

			// Present the screen
			graphics.SwapBuffers ();
			//SampleDraw.ClearSprite();
		}
		private static void drawDesktop()
		{
			
			try
			{
				if(Frame.FrameReady)
				{
					if(texture1.Width != Frame.Width || texture1.Height != Frame.Height)
					{
						texture1 = new Texture2D(Frame.Width ,Frame.Height,false,PixelFormat.Rgba);
						texture2 = new Texture2D(Frame.Width ,Frame.Height,false,PixelFormat.Rgba);
					}
					
					texture1.SetPixels(0,Frame.FrameBuffer1);
					texture2.SetPixels(0,Frame.FrameBuffer2);
					
					sceneMain.updateImage(texture1,texture2);
					
					//SendPacket.sendReady();
					GC.Collect();
				}
			}
			catch(Exception e)
			{
				Console.WriteLine(e.ToString());
			}
		}
	}
}
