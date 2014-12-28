using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VitaRemoteServer
{
    public static class Settings
    {
       
        public static int ScreenWidth
        {
            get { return Screen.PrimaryScreen.Bounds.Width; }
        }

        public static int ScreenHeight
        {
            get { return Screen.PrimaryScreen.Bounds.Height; }
        }

    }
}