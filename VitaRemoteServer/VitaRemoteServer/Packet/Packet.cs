using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace VitaRemoteServer
{
    
    //server packet header 8 bytes
    //        RD                  0000                      00                  000000000000000000000000000
    //head start 2bytes    packet size 4 bytes      packet id 2 bytes           packet data

    //client packet header 6 bytes
    //        RD                  00                      00                  000000000000000000000000000
    //head start 2bytes    packet size 2 bytes      packet id 2 bytes           packet data
    public class Packet  //packet for receiving
    {
        public Packet()
        {
            _ID = 0;
        }

        private byte[] _Data;
        public byte[] Data
        {
            get { return _Data; }
            set { this._Data = value; }
        }

        private int _ID;
        public int ID
        {
            get { return _ID; }
            set { this._ID = value; }
        }

        private int _Len;
        public int Len
        {
            get { return _Len; }
            set
            {
                this._Len = value;
                _Data = new byte[_Len - 8];
            }
        }
    }
	
    public class PacketData
    {
        private int _ID;
        private const int headerSize = 8;
        private byte[] headerStart;

        public PacketData()
        {
            headerStart = Encoding.ASCII.GetBytes("RD");
        }

        public int ID
        {
            get{return _ID;}
            set { this._ID = value; }
        }

        private string _sData;
        public String sData
        {
            get { return _sData; }
            set {
                this._sData = value;
                _size = _sData.Length + headerSize;
            }
        }

        private byte[] _bData;
        public byte[] bData
        {
            get { return _bData; }
            set
            {
                this._bData = value;
                _size = _bData.Length + headerSize;
            }
        }

        private int _size;
        public int Size
        {
            get { return this._size ; }
        }

        public byte[] toArray()
        {
            byte[] retVal = new byte[_size];
            if (_bData != null)
            {

                Array.Copy(headerStart, 0, retVal, 0, headerStart.Length);
                Array.Copy(BitConverter.GetBytes(_size), 0, retVal, 2, 4);
                Array.Copy(BitConverter.GetBytes(_ID), 0, retVal, 6, 2);
                Array.Copy(_bData, 0, retVal, headerSize, _bData.Length);
                
            }
            else
            {
                byte[] msg = new byte[_size];
                msg = Encoding.ASCII.GetBytes(_sData);
                Array.Copy(headerStart, 0, retVal, 0, headerStart.Length);
                Array.Copy(BitConverter.GetBytes(_size), 0, retVal, 0, 4);
                Array.Copy(BitConverter.GetBytes(_ID), 0, retVal, 4, 2);
                Array.Copy(msg, 0, retVal, headerSize , msg.Length);
                
            }
            return retVal;

        }

    }

    
}
