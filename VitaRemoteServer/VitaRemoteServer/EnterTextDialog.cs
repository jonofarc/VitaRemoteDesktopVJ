using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VitaRemoteServer
{
    public partial class EnterTextDialog : Form
    {
        bool result;
        string profileName;

        public bool OK
        {
            get { return result; }
        }

        public string Profile
        {
            get { return profileName; }
        }

        public EnterTextDialog()
        {
            InitializeComponent();
            result = false;
            profileName = null;
        }
        
        // OK
        private void button1_Click(object sender, EventArgs e)
        {
            result = true;
            profileName = textBox1.Text + ".dat";
            Close();
        }

        // CANCEL
        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
