using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

namespace VitaRemoteServer
{
    class SocketEventCallback
    {
        public static void AcceptCallback(IAsyncResult ar)
        {
            TCPConnection Server = (TCPConnection)ar.AsyncState;
            Server.connectionRequest(ar);
        }

        public static void ReceiveCallback(IAsyncResult ar)
        {
            TCPConnection TCPrcv = (TCPConnection)ar.AsyncState;
            TCPrcv.dataArrival(ar);
        }

        public static void SendCallback(IAsyncResult ar)
        {
            TCPConnection TCPsnd = (TCPConnection)ar.AsyncState;
            TCPsnd.onSend(ar);
        }
            

    }
    interface ISocketListener
    {
        void connectionRequest(IAsyncResult ar);
        void dataArrival(IAsyncResult ar);
        void onSend(IAsyncResult ar);
    }
    class TCPConnection : ISocketListener 
    {
        
        private const int bufferSize = 1024; //jonathan
        private byte[] recvBuffer = new byte[bufferSize];
        private MemoryStream serverBuffer = new MemoryStream();

        //public const int packetHeaderSize = 13;
        public static byte[] headerStart = new byte[] { 82, 68};
        private static bool ready = false;

        private Socket socketListener;
        private UInt16 _localPort;
        public UInt16 localPort
        {
            get { return _localPort; }
            set { _localPort = value; }
        }

        public bool Ready
        {
            get { return ready; }
            set { ready = value; }
        }

        private Socket clientSocket;
        public Socket SocketHandle
        {
            get { return clientSocket; }

        }

        private  bool _isConnected = false;
        public bool isConnected
        {
            get { return _isConnected; }
            private set { _isConnected = value; }
        }

        public bool listen()
        {
            socketListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			
			
            socketListener.Bind(new IPEndPoint(IPAddress.Any, _localPort));
			
			 
            System.Diagnostics.Debug.WriteLine(string.Format("Server started on port {0}.", _localPort));
			
            socketListener.Listen(1);
            socketListener.BeginAccept(new AsyncCallback(SocketEventCallback.AcceptCallback),this);
          
            return true;
        }

        public void connectionRequest(IAsyncResult ar)
        {
            if (socketListener != null)
            {
                clientSocket = socketListener.EndAccept(ar);
                isConnected = true;
                clientSocket.BeginReceive(recvBuffer, 0, recvBuffer.Length, 0, new AsyncCallback(SocketEventCallback.ReceiveCallback), this);

                System.Diagnostics.Debug.WriteLine(string.Format("client Connected"));
            }
        }
        public void Disconnect()
        {
            try
            {
                socketListener.BeginAccept(new AsyncCallback(SocketEventCallback.AcceptCallback), this);
                if (clientSocket != null)
                {

                    Console.WriteLine("Disconnect Client");
                    //  socket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                    clientSocket = null;
                    _isConnected = false;
                }
            }
            catch
            { }
        }

        public void send(PacketData packet)
        {
            try
            {

                if (clientSocket != null && packet != null)
                {
                    //NetworkStream ns = new NetworkStream(clientSocket);
                    //ns.Write(packet.toArray(), 0, packet.Size);
                    //clientSocket.BeginSend(packet.toArray(), 0, packet.Size, 0, new AsyncCallback(SocketEventCallback.SendCallback), this);
                    //ns.Flush();
                    clientSocket.Send(packet.toArray(), SocketFlags.None);
                }
            }
            catch
            {
                Disconnect();
            }
        }

        public void send(byte[] data)
        {
            if (clientSocket != null && data != null)
            {
                clientSocket.BeginSend(data , 0, data.Length, 0, new AsyncCallback(SocketEventCallback.SendCallback), this);
            }
        }

        public void onSend(IAsyncResult ar)
        {
            try
            {
                if (clientSocket != null)
                {
                    int len = 0;
                    len = clientSocket.EndSend(ar);

                }
            }
            catch(SocketException e)
            {
                if (e.SocketErrorCode == SocketError.ConnectionReset || e.SocketErrorCode == SocketError.ConnectionAborted)
                {
                    Disconnect();
                }
            }
        }


       
        public void dataArrival(IAsyncResult ar)
        {
			int Len = 0;
			try
		{
				if (clientSocket != null){
                    Len = clientSocket.EndReceive(ar);
						
					if (Len <= 0)
					{
						Disconnect();
					}
					else
					{
						int next = 0;
						Packet packet;
						serverBuffer.Write(recvBuffer,0,Len);
                        while ((packet = unpackPacket(serverBuffer, (int)serverBuffer.Length, ref next)) != null)
						{
							ProcessPacket(packet);
                            Remove(next);
						}
                        clientSocket.BeginReceive(recvBuffer, 0, recvBuffer.Length, 0, new AsyncCallback(SocketEventCallback.ReceiveCallback), this);
					}
				}
			}
			catch (System.Net.Sockets.SocketException e)
			{
				if (e.SocketErrorCode == SocketError.ConnectionReset || e.SocketErrorCode == SocketError.ConnectionAborted){
					Disconnect();
				}
			}
			catch (Exception e)
			{
                Debug.WriteLine(e.ToString());
			}

		}


        private int Remove(int len)
        {
            if ((int)serverBuffer.Length == len)
            {
                serverBuffer.Close();
                serverBuffer = new MemoryStream();
                return 0;
            }

            using (MemoryStream retVal = new MemoryStream())
            {
                int dataLen = (int)serverBuffer.Length;
                retVal.Write(serverBuffer.ToArray(), len, dataLen - len);
                serverBuffer.Close();
                serverBuffer = new MemoryStream();

                if (retVal.Length > 0)
                {
                    byte[] data = retVal.ToArray();
                    serverBuffer.Write(data, 0, data.Length);
                }
            }
            return 0;
        }

        private Packet unpackPacket(MemoryStream ms, int len, ref int next)
        {
            if (len < 6)
                return null;

            int pos = ms.ToArray().Find(headerStart);

            if (pos != 0)
                return null;

            byte[] data = ms.ToArray();

            int packetLen = BitConverter.ToInt16(data, pos + headerStart.Length);

            if ((len - pos) >= packetLen)
            {
                Packet packet = new Packet();
                packet.ID = BitConverter.ToInt16(data, pos + 4);
                packet.Data = new byte[packetLen - 6];
                Array.Copy(data, pos + 6, packet.Data, 0, packet.Data.Length);
                next = packetLen + pos;
                return packet;
            }
            else
            {
                return null;
            }
        }


        private void ProcessPacket(Packet packet)
        {
            
            switch(packet.ID)
            {
                		case 101:
                    Point ptTap = Extensions.vitaCoordsToDesktopCoords(packet.Data);
                    ReceivePacket.Tap(ptTap);
                    break;
                case 102:
                    Point ptDTap = Extensions.vitaCoordsToDesktopCoords(packet.Data);
                    ReceivePacket.DoubleTap(ptDTap);
                    break;
                case 103:
                    Point ptDrag = Extensions.getVitaCoords(packet.Data);
                    ReceivePacket.Drag(ptDrag);
                    break;
                case 104:
                    Point ptMouseMove = Extensions.vitaCoordsToDesktopCoords(packet.Data);
                    ReceivePacket.MouseMove(ptMouseMove);
                    break;
                case 105:
                    Point ptRightClick = Extensions.vitaCoordsToDesktopCoords(packet.Data);
                    ReceivePacket.MouseRightClick(ptRightClick);
                    break;
                case 106:
                    Point ptRightDoubleClick = Extensions.vitaCoordsToDesktopCoords(packet.Data);
                    ReceivePacket.MouseRightDoubleClick(ptRightDoubleClick);
                    break;
                case 107:
                    Point leftMouseDown = Extensions.vitaCoordsToDesktopCoords(packet.Data);
                    ReceivePacket.LeftMouseDown(leftMouseDown);
                    break;
                case 108:
                    Point leftMouseUp = Extensions.vitaCoordsToDesktopCoords(packet.Data);
                    ReceivePacket.LeftMouseUp(leftMouseUp);
                    break;
//jonathan area dded for functions not included in client
				case 109:
				
                    Point ptDrag2 = Extensions.getVitaCoords(packet.Data);
                    ReceivePacket.Drag2(ptDrag2);
                    break;
		 		case 110:
                    Point sendLongPress = Extensions.vitaCoordsToDesktopCoords(packet.Data);
                    ReceivePacket.LongPress(sendLongPress);
                    break;
		 		case 111:
                    Point sendDragStart = Extensions.getVitaCoords(packet.Data);
                    ReceivePacket.DragStart(sendDragStart);
                    break;
				case 112:		
                   Point sendDragEnd = Extensions.getVitaCoords(packet.Data);
                    ReceivePacket.DragEnd(sendDragEnd);
                    break;
// end jonathan
                case 301:
                    ReceivePacket.ReceiveGamePadInput(packet.Data);
                    break;
                case 302:
                    ReceivePacket.MotionData(packet.Data);
                    break;
                case 401:
                    ready = true;
                    break;
                default:
                    break;
            }
        }

    }
}
