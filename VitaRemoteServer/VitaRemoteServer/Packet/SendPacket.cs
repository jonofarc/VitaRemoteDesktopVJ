using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VitaRemoteServer
{
    class SendPacket 
    {
        public static void sendMessage(TCPConnection socket, string Msg)
        {
			
            PacketData msg = new PacketData();
            msg.sData = Msg;
            msg.ID = 101;
			
            socket.send(msg);
			
        }
        public static void sendImage(TCPConnection socket, byte[] img)
        {
            PacketData msg = new PacketData();
            msg.ID = 102;
		//	Console.WriteLine(img.Length);
            msg.bData = img;
            socket.send(msg);
        }

        public static void sendImage2(TCPConnection socket, byte[] img)
        {
            PacketData msg = new PacketData();
            msg.ID = 103;
            msg.bData = img;
            socket.send(msg);
        }

        public static void sendPing(TCPConnection socket)
        {
            byte[] ping = {255,0,255,0};
            socket.send(ping);
        }
        
    }
}
