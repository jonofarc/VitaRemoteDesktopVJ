using System;

namespace VitaRemoteClient
{
	public static class Extensions 
	{
		//Find Function
		//Search an array patterns from another array, returns -1 if pattern not found
		//returns the array pos when found
		public static int Find (this byte[] buff, byte[] search) 
		{
			int result;
			for (int i = 0; i < (buff.Length - search.Length); i++)
			{
				if (buff [i] == search [0])
				{
					int j;
					for (j = 1; j < search.Length; j++)
					{
						if (buff [i + j] != search [j])
						{
							break;
						}
					}
					if (j == search.Length)
					{
						result = i;
						return result;
					}
				}
			}
			result = -1;
			return result;
		}
		
		public static bool CompareBytes(this byte[] buff, byte[] buff2)
		{
			for(int i = 0; i < buff.Length; i++)
			{
				if(buff[i] != buff2[i])
					return true;
			}
			
			return false;
		}
    }
	
	
}

