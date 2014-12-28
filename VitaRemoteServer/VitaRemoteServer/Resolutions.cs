using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VitaRemoteServer
{
    public class Resolutions
    {
        public int res_x;
        public int res_y;

        public override string ToString()
        {
            return "Capture Area: " + res_x + " X " + res_y;
        }
        public static bool operator ==(Resolutions x, Resolutions y)
        {
            return x.res_y == y.res_y && x.res_x == y.res_x;
        }
        public static bool operator !=(Resolutions x, Resolutions y)
        {
            return x.res_y != y.res_y && x.res_x != y.res_x;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
    };
}
