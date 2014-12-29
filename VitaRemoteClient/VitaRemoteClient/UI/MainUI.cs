using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;
using Sce.PlayStation.Core.Graphics;

namespace VitaRemoteClient 
{
	public partial class MainUI : Scene
	{
		private bool bDragging;
		public MainUI()
		{
			InitializeWidget();
			bDragging = false;
		}
		
		public void updateLabel(string str)
		{
			if(lblOutput.Text != str)
				lblOutput.Text = str;
		}
		
		public void updateImage(Texture2D txt, Texture2D txt2)
		{
			if(txt != null && txt2 != null)
			{
				imgDesktop.Image = new ImageAsset(txt);
				imgDesktop2.Image = new ImageAsset(txt2);
			}
		}
		
		void  Tap (object sender, TapEventArgs e)
		{
		 	SendPacket.sendTap((int)e.LocalPosition.X, (int)e.LocalPosition.Y);	
			Console.WriteLine("Tap at " + e.LocalPosition.ToString());
			SendPacket.sendLeftMouseClick((int)e.LocalPosition.X,(int)e.LocalPosition.Y);//jonathan send touch cordinates so we can click those cordinates
		
			
			
		}
		
		void DoubleTap (object sender, DoubleTapEventArgs e)
		{
			SendPacket.sendDoubleTap((int)e.LocalPosition.X, (int)e.LocalPosition.Y);	
			Console.WriteLine("DoubleTap at " + e.LocalPosition.ToString());
			SendPacket.sendLeftMouseDoubleClick((int)e.LocalPosition.X,(int)e.LocalPosition.Y);//jonathan send touch cordinates so we can click those cordinates
		}
		
		void  Drag (object sender, DragEventArgs e)
		{
			Vector2 pos = e.LocalPosition;
			
			// 1 finger
			if(Frame.TC == 1)
			{
				if(Frame.RelativeDragOne)
				{
					pos = e.Distance;
				}
				if(e.Distance.X != 0 || e.Distance.Y != 0)
				{
					SendPacket.sendDrag1((int)pos.X,(int)pos.Y);
					Console.WriteLine("Drag1 at " + pos.ToString());
					SendPacket.sendLeftMouseDown((int)e.LocalPosition.X,(int)e.LocalPosition.Y);//jonathan using drag as a mousedown
					Console.WriteLine(" sendLeftMouseDown at" + e.LocalPosition.ToString());
				}
			}
			
			// 2 fingers
			if(Frame.TC == 2)
			{
				if(e.Distance.X != 0 || e.Distance.Y != 0)
				{
					if(Frame.RelativeDragTwo)
					{
						pos = e.Distance;
					}
		 			SendPacket.sendDrag2((int)pos.X,(int)pos.Y);
					Console.WriteLine("Drag2 at " + pos.ToString());
					
				}
			}
		}
		
		 void  LongPress (object sender, LongPressEventArgs e)
		 {
			SendPacket.sendLongPress((int)e.LocalPosition.X,(int)e.LocalPosition.Y);	
			Console.WriteLine("LongPress at " + e.LocalPosition.ToString());
			
			
		
		
		 }
		
		  void  dragStart (object sender, DragEventArgs e)
		 {
			if(Frame.TC == 1 && bDragging == false)
			{
		 		SendPacket.sendDragStart((int)e.LocalPosition.X,(int)e.LocalPosition.Y);
				Console.WriteLine("DragStart1 at " + e.LocalPosition.ToString());
				bDragging = true;
			}
			if(Frame.TC == 2 && bDragging == false)
			{
		 		SendPacket.sendDragStart((int)e.LocalPosition.X,(int)e.LocalPosition.Y);
				Console.WriteLine("DragStart2 at " + e.LocalPosition.ToString());
				bDragging = true;
			}
		 }
		
		 void  dragEnd (object sender, DragEventArgs e)
		 {
			if(Frame.TC == 1 && bDragging)
			{
				SendPacket.sendDragEnd((int)e.LocalPosition.X,(int)e.LocalPosition.Y);
				Console.WriteLine("DragEnd1 at " + e.LocalPosition.ToString());
				bDragging = false;
			}
			if(Frame.TC == 2 && bDragging)
			{
				SendPacket.sendDragEnd((int)e.LocalPosition.X,(int)e.LocalPosition.Y);
				Console.WriteLine("DragEnd2 at " + e.LocalPosition.ToString());
				bDragging = false;
			}
		 }
		 


		
	}
		
}