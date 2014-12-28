using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Imaging;

using System.Net;
using System.Net.Sockets;

namespace VitaRemoteClient
{
	
	public enum Status
	{
		kNone,
		kConnecting,
		kConnected,	
		kDisconnected,
		kConnectionFailed,
		kUnknown
	}
	
	public struct imgData
	{
		public int ID;
		public byte[] data;
	}
	
	interface ISocketListener
	{
		void OnConnect(IAsyncResult AsyncResult);
		//void OnReceive(IAsyncResult AsyncResult);
		void OnSend(IAsyncResult AsyncResult);
	}

	/**
	 * SocketEventCallback
	 */
	class SocketEventCallback
	{
		public static void ConnectCallback(IAsyncResult AsyncResult)
		{
			TCPConnection Client = (TCPConnection)AsyncResult.AsyncState;
			Client.OnConnect(AsyncResult);
		}
//		public static void ReceiveCallback(IAsyncResult AsyncResult)
//		{
//			TCPConnection TCPs = (TCPConnection)AsyncResult.AsyncState;
//			TCPs.OnReceive(AsyncResult);
//		}
		public static void SendCallback(IAsyncResult AsyncResult)
		{
			TCPConnection TCPs = (TCPConnection)AsyncResult.AsyncState;
			TCPs.OnSend(AsyncResult);
		}
	}
	
	/**
	 * Class for SocketTCP local connection
	 */
	public class TCPConnection : ISocketListener
	{
		#region "Properties and variables"
		private byte[] recvBuffer = new byte[bufferSize];
		private const int bufferSize = 1024 * 1024;
		private MemoryStream clientBuffer = new MemoryStream();
		private const int packetHeaderSize = 8;
		private byte[] headerStart = new byte[] { 82, 68};
		private byte[] boundary;
		
        /**
         * Object for exclusive  socket access
         */
        private object syncObject = new object();
		/**
		 * Enter critical section
		 */
		private void enterCriticalSection() 
		{
			Monitor.Enter(syncObject);
		}
		/**
		 * Leave critical section
		 */
		private void leaveCriticalSection() 
		{
			Monitor.Exit(syncObject);
		}

		/**
		 * Get status
		 * 
		 * @return Status
		 */
		private Status _status;
		public Status StatusType
		{
			get{return this._status;}
			private set{ this._status = value;}
		}

		
		/**
		 * Are we connected
		 */
		private bool _isConnected = false;
		public bool isConnected
		{
					get	{	return _isConnected; }
			private set	{	this._isConnected = value;	}
		}

        private Socket socket;
		public  Socket Socket 
		{
			get	{	return socket;	}
		}

		/**
		 * Port number
		 */
		private UInt16 port;
		public UInt16 Port
		{
			get	{	return port;}
			set {this.port = value;}
		}
		
		private string _remoteHost;
		public string RemoteHost
		{
			get { return _remoteHost;}
			set { this._remoteHost = value;}
		}

		/**
		 * Constructor
		 */
		public TCPConnection(string remoteHost, UInt16 Port)
		{
			_remoteHost = remoteHost;
			port      = Port;
			boundary = System.Text.ASCIIEncoding.ASCII.GetBytes("--boundary");
			this.StatusType = Status.kNone;
		}
		
		
		#endregion 
		
		#region "Functions"
        /**
         * Connect to the local host server
         * 
         * Can only be executed when client
         */
        public bool Connect() 
		{
			try
			{
				enterCriticalSection();
				socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);	
				//socket.Blocking = false;
				
				
/*				
				IPAddress LocalIP = null;
				IPHostEntry IPInfo = Dns.GetHostEntry("localhost");
				foreach (IPAddress Info in IPInfo.AddressList)
				{
					if (Info.AddressFamily == AddressFamily.InterNetwork){
						LocalIP = Info;
						break;
					}
				}
				IPEndPoint EP = new IPEndPoint(LocalIP, port);
*/				
				IPEndPoint EP = new IPEndPoint(IPAddress.Parse(_remoteHost), port);
				socket.BeginConnect(EP, new AsyncCallback(SocketEventCallback.ConnectCallback), this);
				this.StatusType = Status.kConnecting;
			}
			finally
			{
				leaveCriticalSection();
			}
			return true;
		}

		/**
		 * Disconnect
		 */
		public void Disconnect() 
		{
			try
			{
				enterCriticalSection();
				if (socket != null){
					
					Console.WriteLine("Disconnect Client");
					//  socket.Shutdown(SocketShutdown.Both);
					socket.Close();
					socket		= null;
					isConnected	= false;
				}
			}
			finally
			{
				this.StatusType = Status.kDisconnected;
				leaveCriticalSection();
			}
		}
		
		public void updateImage()
		{
			try
			{
				
				if(isConnected && socket != null)
				{
					
					int len, pos = 0;
					int packetLen = 0;
					int totalLen = 0;
					//Packet packet = new Packet();
					len = socket.Receive(recvBuffer,recvBuffer.Length,SocketFlags.None);
					totalLen += len;
					//clientBuffer.Write(recvBuffer, 0, len); //save to buffer;
					if(len > packetHeaderSize)
					{
						pos = recvBuffer.Find(headerStart);
						if(pos == 0 )
						{
							//Packet Header found
							packetLen = BitConverter.ToInt32(recvBuffer, pos + headerStart.Length);
							
							while((totalLen - pos) < packetLen)
							{
								len = socket.Receive(recvBuffer,totalLen, 8129, SocketFlags.None);//loop till packet is complete
								totalLen += len;
								//clientBuffer.Write(recvBuffer, 0, len);
							}
							
							Packet packet = unpackPacket(recvBuffer,packetLen);
							
							if(packet != null)
								ProcessPacket(packet);
							
							//clientBuffer.Close();
							//clientBuffer = new MemoryStream();
						}
					}
					else
					{
						//clientBuffer.Close();
						//clientBuffer = new MemoryStream();
					}
				}
			}
			
			catch(System.IO.IOException e)
			{
				Console.WriteLine(e.ToString());
			}
			catch(SocketException e)
			{
				if (e.SocketErrorCode == SocketError.ConnectionReset || e.SocketErrorCode == SocketError.ConnectionAborted){
				Disconnect();
				}
			}
		
		}
		
		/***
		 * Connect
		 */
		public void OnConnect(IAsyncResult AsyncResult) //worker thread for receiving and processing packets
		{
			try
			{
				if(socket != null)
				{
					socket.EndConnect(AsyncResult);
					isConnected = true;
					//Console.WriteLine("Client connected...");
					this.StatusType = Status.kConnected;
					Thread.Sleep(2000);
					SendPacket.sendReady();
					
					//socket.BeginReceive(recvBuffer, 0, recvBuffer.Length, 0, new AsyncCallback(SocketEventCallback.ReceiveCallback), this);

				}
			}
			catch(SocketException e)
			{
				if (e.SocketErrorCode == SocketError.ConnectionReset || e.SocketErrorCode == SocketError.ConnectionAborted){
				Disconnect();
				}
			}
		}
		
//		private void Remove(int len)
//		{
//			using(MemoryStream retVal = new MemoryStream())
//			{
//				int dataLen = (int)clientBuffer.Length;
//				retVal.Write(clientBuffer.ToArray(),len, dataLen - len);
//				clientBuffer.Close();
//				clientBuffer = new MemoryStream();
//				
//				if(retVal.Length > 0)
//				{
//					byte[] data = retVal.ToArray();
//					clientBuffer.Write(data,0,data.Length);
//				}
//			}
//		}
		
		public void send(Packet packet)
		{
			try 
			{
				if(socket != null && StatusType == Status.kConnected)
					//socket.BeginSend(packet.toArray(), 0, packet.Len, 0, new AsyncCallback(SocketEventCallback.SendCallback), this);
					socket.Send(packet.toArray(),SocketFlags.None);
			}
			catch(System.Net.Sockets.SocketException e)
			{
				if (e.SocketErrorCode == SocketError.ConnectionReset || e.SocketErrorCode == SocketError.ConnectionAborted){
					Console.WriteLine("DataExchange 切断検出");
					Disconnect();
				}
			}
				
		}		
		
//		private void Remove(int len)
//		{
//			using(MemoryStream retVal = new MemoryStream())
//			{
//				int dataLen = (int)clientBuffer.Length;
//				retVal.Write(clientBuffer.ToArray(),len, dataLen - len);
//				clientBuffer.Close();
//				clientBuffer = new MemoryStream();
//				
//				if(retVal.Length > 0)
//				{
//					byte[] data = retVal.ToArray();
//					clientBuffer.Write(data,0,data.Length);
//				}
//			}
//		}
		
		
		/**
		 * Send
		 */
		public void OnSend(IAsyncResult AsyncResult)
		{
			int Len = 0;
			try
			{
				try
				{
					enterCriticalSection();
					if (Socket != null){
						Len = Socket.EndSend(AsyncResult);}
                    // Disconnection detection should go here...
					if (Len <= 0){
						// send error
					}
				}
				finally
				{
					leaveCriticalSection();
				}
			}
			catch (System.Net.Sockets.SocketException e)
			{
				if (e.SocketErrorCode == SocketError.ConnectionReset || e.SocketErrorCode == SocketError.ConnectionAborted){
					Console.WriteLine("SendCallback 切断検出");
//					Log.Write("SendCallback 切断検出\n");
					Disconnect();
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
		}
		
		private Packet unpackPacket(byte[] data, int len)
		{
			
			try
			{
				Packet packet = new Packet();
	
				packet.ID = BitConverter.ToUInt16(data, 6);
				packet.Data = new byte[len - packetHeaderSize];
				Array.Copy(data ,packetHeaderSize , packet.Data , 0 ,packet.Data.Length); //get the packetDATA
				return packet;
			}
			catch 
			{
				return null;
			}
		}
		
		public static void imgDecoder(object obj)
		{
			imgData imgdata = (imgData)obj;
			var img = new Image(imgdata.data);
			img.Decode();
			
			if(Frame.Width != img.Size.Width || Frame.Height != img.Size.Height)
				Frame.Width = img.Size.Width;Frame.Height = img.Size.Height;
			
			if(imgdata.ID == 1)
				Frame.FrameBuffer1 = img.ToBuffer();
			else
				Frame.FrameBuffer2 = img.ToBuffer();
			
			img.Dispose();
		}
		
		private void ProcessPacket(Packet packet)
		{
			switch(packet.ID)
			{
			case 102:
				if(Frame.FrameReady) 
					break;
				
				try{
					//AppMain.sceneMain.updateLabel((packet.Data.Length / 1024) + "kb");
					//Console.WriteLine((packet.Data.Length / 1024) + "kb");
					//var img = new Image(packet.Data);
					//img.Decode(); 
					//--boundary
					int pos = packet.Data.Find(boundary);
					if(pos != -1)
					{
						imgData img1 = new imgData();
						imgData img2 = new imgData();
						img1.ID = 1;
						img1.data = new byte[pos - 1];
						Array.Copy(packet.Data,0, img1.data ,0, img1.data.Length);
						
						img2.ID = 2;
						img2.data = new byte[packet.Data.Length - (pos + boundary.Length)];
						Array.Copy(packet.Data, pos + boundary.Length, img2.data,0, img2.data.Length);
						
						ThreadPool.QueueUserWorkItem(new WaitCallback(imgDecoder),img1);
						ThreadPool.QueueUserWorkItem(new WaitCallback(imgDecoder),img2);
					}
					//Frame.FrameBuffer = img.ToBuffer(); //copy to image buffer
					//img.Dispose();
					//GC.Collect();
				}
				catch {
				}
					break;
			default:
				break;
			
			case 103:
				
				
				break;
			case 501: // recieve if we are moving touch relative or not
			case 502:
			{
				Console.WriteLine(packet.ID + " recieved: " + (packet.Data.Length / 1024) + "kb");
				char[] fakeBool = new char[1];
				Array.Copy(packet.Data, 0, fakeBool, 0, 1);
				
				Console.WriteLine((int)fakeBool[0]);
				
				if(packet.ID == 501)
				{
					Frame.RelativeDragOne = ((int)fakeBool[0] == 1);
				}
				else
				{
					Frame.RelativeDragTwo = ((int)fakeBool[0] == 1);
				}
				
			} break;
		}
		#endregion 
	}
	}
}
	

