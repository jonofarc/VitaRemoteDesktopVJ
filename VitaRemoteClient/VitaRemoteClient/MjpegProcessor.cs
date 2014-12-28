using System;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Graphics;

namespace VitaRemoteClient
{
	public class FrameReadyEventArgs : EventArgs 
	{
		public byte[] FrameBuffer;
	}
	
	public class MjpegProcessor
	{
		public event EventHandler<FrameReadyEventArgs> FrameReady;
		
		public void ProcessFrame(byte[] pixelArray)
		{
			if(pixelArray!= null)
			{
				if(FrameReady!= null)
				{
					var img = new Image(pixelArray);
					img.Decode();
					FrameReady(this, new FrameReadyEventArgs{ FrameBuffer = img.ToBuffer()});
					}
			}
		}
	}
}

