// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

//namespace MsgDialog
//{
    public class MessageBox : Dialog
    {
	private Label lblTitle = new Label();
	
	public static void init()
	{
		M
	}
	public static int show(string title, string message, MessageDialogStyle style)
	{
		MsgBox.Show();
		return 0;
	}
	
	public static void hide()
	{
		MsgBox.Hide();
	}
    }
//}
