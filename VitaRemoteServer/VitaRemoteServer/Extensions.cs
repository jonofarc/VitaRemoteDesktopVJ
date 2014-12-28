using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace VitaRemoteServer
{
    public static class Extensions
    {
        public static int Find(this byte[] buff, byte[] search)
        {
            int result;
            for (int i = 0; i < buff.Length - search.Length; i++)
            {
                if (buff[i] == search[0])
                {
                    int j;
                    for (j = 1; j < search.Length; j++)
                    {
                        if (buff[i + j] != search[j])
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

        public static Point vitaCoordsToDesktopCoords(byte[] CoordsData)
        {
            int x = BitConverter.ToInt16(CoordsData, 0);
            int y = BitConverter.ToInt16(CoordsData, 2);

            Point pt = new Point();
            int _x = Convert.ToInt16(((float)x / 960f) * 100);
            int _y = Convert.ToInt16(((float)y / 544f) * 100);

            pt.X = ((screenCapture.AreaWidth * _x) / 100) + screenCapture.X;
            pt.Y = ((screenCapture.AreaHeight * _y) / 100) + screenCapture.Y;

            return pt;
        }

        public static Point getVitaCoords(byte[] CoordsData)
        {
        
            Point pt = new Point();
            pt.X  = BitConverter.ToInt16(CoordsData, 0);
            pt.Y = BitConverter.ToInt16(CoordsData, 2);

            return pt;
        }
    }
}
