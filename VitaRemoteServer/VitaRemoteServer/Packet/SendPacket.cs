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
            msg.bData = img;
            socket.send(msg);
        }

        public static void sendRelativeDragOne(TCPConnection socket, bool bRelative)
        {
            PacketData msg = new PacketData();
            byte[] fakeBool = new byte[1];

            if (bRelative)
            {
                fakeBool[0] = 1;
            }
            else
            {
                fakeBool[0] = 0;
            }

            msg.ID = 501;
            msg.bData = fakeBool;
            socket.send(msg);
        }
        public static void sendRelativeDragTwo(TCPConnection socket, bool bRelative)
        {
            PacketData msg = new PacketData();
            byte[] fakeBool = new byte[1];

            if (bRelative)
            {
                fakeBool[0] = 1;
            }
            else
            {
                fakeBool[0] = 0;
            }

            msg.ID = 502;
            msg.bData = fakeBool;
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
