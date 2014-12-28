// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace dialog
{
    partial class DLG_CONNECTION
    {
        Label Label_1;
        EditableText txtIp;
        EditableText txtPort;
        Label Label_2;
        Button btn_Connect;
        Button btn_Cancel;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            Label_1 = new Label();
            Label_1.Name = "Label_1";
            txtIp = new EditableText();
            txtIp.Name = "txtIp";
            txtPort = new EditableText();
            txtPort.Name = "txtPort";
            Label_2 = new Label();
            Label_2.Name = "Label_2";
            btn_Connect = new Button();
            btn_Connect.Name = "btn_Connect";
            btn_Cancel = new Button();
            btn_Cancel.Name = "btn_Cancel";

            // Label_1
            Label_1.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            Label_1.Font = new Font( FontAlias.System, 20, FontStyle.Regular);
            Label_1.LineBreak = LineBreak.Character;

            // txtIp
            txtIp.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            txtIp.Font = new Font( FontAlias.System, 25, FontStyle.Regular);
            txtIp.LineBreak = LineBreak.Character;

            // txtPort
            txtPort.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            txtPort.Font = new Font( FontAlias.System, 25, FontStyle.Regular);
            txtPort.LineBreak = LineBreak.Character;

            // Label_2
            Label_2.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            Label_2.Font = new Font( FontAlias.System, 20, FontStyle.Regular);
            Label_2.LineBreak = LineBreak.Character;

            // btn_Connect
            btn_Connect.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            btn_Connect.TextFont = new Font( FontAlias.System, 25, FontStyle.Regular);
			btn_Connect.ButtonAction +=  Connect;

            // btn_Cancel
            btn_Cancel.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            btn_Cancel.TextFont = new Font( FontAlias.System, 25, FontStyle.Regular);

            // DLG_CONNECTION
			this.Visible = false;
            // Dialog
            this.AddChildLast(Label_1);
            this.AddChildLast(txtIp);
            this.AddChildLast(txtPort);
            this.AddChildLast(Label_2);
            this.AddChildLast(btn_Connect);
            this.AddChildLast(btn_Cancel);
            this.ShowEffect = new BunjeeJumpEffect()
            {
            };

            SetWidgetLayout(orientation);

            UpdateLanguage();
        }

        

        private LayoutOrientation _currentLayoutOrientation;
        public void SetWidgetLayout(LayoutOrientation orientation)
        {
            switch (orientation)
            {
            case LayoutOrientation.Vertical:
                this.SetPosition(0, 0);
                this.SetSize(544, 960);
                this.Anchors = Anchors.None;

                Label_1.SetPosition(139, 67);
                Label_1.SetSize(214, 36);
                Label_1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                Label_1.Visible = true;

                txtIp.SetPosition(128, 119);
                txtIp.SetSize(360, 56);
                txtIp.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                txtIp.Visible = true;

                txtPort.SetPosition(128, 119);
                txtPort.SetSize(360, 56);
                txtPort.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                txtPort.Visible = true;

                Label_2.SetPosition(139, 67);
                Label_2.SetSize(214, 36);
                Label_2.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                Label_2.Visible = true;

                btn_Connect.SetPosition(31, 180);
                btn_Connect.SetSize(214, 56);
                btn_Connect.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                btn_Connect.Visible = true;

                btn_Cancel.SetPosition(31, 180);
                btn_Cancel.SetSize(214, 56);
                btn_Cancel.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                btn_Cancel.Visible = true;

                break;

            default:
                this.SetPosition(0, 0);
                this.SetSize(500, 240);
                this.Anchors = Anchors.None;

                Label_1.SetPosition(31, 35);
                Label_1.SetSize(333, 36);
                Label_1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                Label_1.Visible = true;

                txtIp.SetPosition(31, 81);
                txtIp.SetSize(333, 56);
                txtIp.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                txtIp.Visible = true;

                txtPort.SetPosition(377, 81);
                txtPort.SetSize(95, 56);
                txtPort.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                txtPort.Visible = true;

                Label_2.SetPosition(377, 35);
                Label_2.SetSize(95, 36);
                Label_2.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                Label_2.Visible = true;

                btn_Connect.SetPosition(56, 149);
                btn_Connect.SetSize(180, 56);
                btn_Connect.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                btn_Connect.Visible = true;

                btn_Cancel.SetPosition(258, 149);
                btn_Cancel.SetSize(180, 56);
                btn_Cancel.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                btn_Cancel.Visible = true;

                break;
            }
            _currentLayoutOrientation = orientation;
        }
        public void UpdateLanguage()
        {
            Label_1.Text = "IP Adress:";
            Label_2.Text = "Port:";
            btn_Connect.Text = "Connect";
            btn_Cancel.Text = "Cancel";
			
			string ip, port;
			OpenSavedAddress(out ip, out port);
            txtIp.Text = ip;
            txtPort.Text = port;
        }
		
        private void onShowing(object sender, EventArgs e)
        {
            switch (_currentLayoutOrientation)
            {
                case LayoutOrientation.Vertical:
                {
                }
                break;

                default:
                {
                }
                break;
            }
        }
        private void onShown(object sender, EventArgs e)
        {
            switch (_currentLayoutOrientation)
            {
                case LayoutOrientation.Vertical:
                {
                }
                break;

                default:
                {
                }
                break;
            }
        }
    }
}
