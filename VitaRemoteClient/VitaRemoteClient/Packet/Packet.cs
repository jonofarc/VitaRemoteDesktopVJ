using System;
using System.Text;

namespace VitaRemoteClient
{
	public class Packet
	{
        private const int headerSize = 6;
        private byte[] headerStart = new byte[] {82, 68};
		
		public Packet ()
		{
			_ID = 0;	
		}
		
		private byte[] _Data;
		public byte[] Data 
		{
			get{return _Data;}
			set{this._Data = value;
				_Len = Data.Length + headerSize;
			}
		}
		
		private int _ID;
		public int ID 
		{
			get{return _ID;}
			set{this._ID = value;}
		}
		
		private int _Len;
		public int Len 
		{
			get{return _Len;}
		}
		
		public byte[] toArray()
        {
            byte[] retVal = new byte[_Data.Length + headerSize];
            if (_Data != null)
            {
                Array.Copy(headerStart, 0, retVal, 0, headerStart.Length);
                Array.Copy(BitConverter.GetBytes(_Len), 0, retVal, 2, 2);
                Array.Copy(BitConverter.GetBytes(_ID), 0, retVal, 4, 2);
                Array.Copy(_Data, 0, retVal, headerSize, _Data.Length);
            }
            return retVal;

        }
	}
	
}

