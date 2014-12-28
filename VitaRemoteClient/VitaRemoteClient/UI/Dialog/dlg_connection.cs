using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;
using System.Threading;
using System.IO;

namespace dialog
{
    public partial class DLG_CONNECTION : Dialog
    {
        public DLG_CONNECTION()
            : base(null, null)
        {
            InitializeWidget();
        }
		
		private void  Connect (object sender, TouchEventArgs e)
        {
			SaveAddress(txtIp.Text, txtPort.Text);
			
			VitaRemoteClient.AppMain.client.Port = Convert.ToUInt16(txtPort.Text.ToString());
			VitaRemoteClient.AppMain.client.RemoteHost = txtIp.Text;
			VitaRemoteClient.AppMain.client.Connect();
			this.Hide();
			this.Visible =false;
        }
		
		private void OpenSavedAddress(out string szIP, out string szPort)
		{
			try
			{
				BinaryReader read = new BinaryReader(File.Open("/Documents/settings.dat", FileMode.Open));
					
				int numIPChars = (int)read.ReadChar();
				int numPortChars = (int)read.ReadChar();
				
				szIP = new string(read.ReadChars(numIPChars));
				szPort = new string(read.ReadChars(numPortChars));
				
				read.Close();
			}
			catch
			{
				szIP = "127.0.0.1";
				szPort = "8080";
			}
		}
		private void SaveAddress(string szIP, string szPort)
		{
			BinaryWriter write = new BinaryWriter(File.Open("/Documents/settings.dat", FileMode.CreateNew, FileAccess.Write));
				
			char numIPChars = (char)szIP.ToCharArray().Length;
			char numPortChars = (char)szPort.ToCharArray().Length;
			
			write.Write(numIPChars);
			write.Write(numPortChars);
			write.Write(szIP.ToCharArray());
			write.Write(szPort.ToCharArray());
		
			write.Close();
		}
    }
}
