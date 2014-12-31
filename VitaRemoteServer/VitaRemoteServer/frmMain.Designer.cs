namespace VitaRemoteServer
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tbResolution = new System.Windows.Forms.TrackBar();
            this.tbArea = new System.Windows.Forms.TrackBar();
            this.lblRes = new System.Windows.Forms.Label();
            this.lblScale = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tbQuality = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.tbResolution)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbArea)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbQuality)).BeginInit();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 10;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // tbResolution
            // 
            this.tbResolution.Location = new System.Drawing.Point(12, 25);
            this.tbResolution.Maximum = 2;
            this.tbResolution.Minimum = 1;
            this.tbResolution.Name = "tbResolution";
            this.tbResolution.Size = new System.Drawing.Size(245, 45);
            this.tbResolution.TabIndex = 0;
            this.tbResolution.Value = 2;
            this.tbResolution.Scroll += new System.EventHandler(this.tbResolution_Scroll);
            // 
            // tbArea
            // 
            this.tbArea.Location = new System.Drawing.Point(12, 73);
            this.tbArea.Maximum = 5;
            this.tbArea.Minimum = 1;
            this.tbArea.Name = "tbArea";
            this.tbArea.Size = new System.Drawing.Size(245, 45);
            this.tbArea.TabIndex = 1;
            this.tbArea.Value = 2;
            this.tbArea.Scroll += new System.EventHandler(this.tbArea_Scroll);
            // 
            // lblRes
            // 
            this.lblRes.AutoSize = true;
            this.lblRes.Location = new System.Drawing.Point(12, 9);
            this.lblRes.Name = "lblRes";
            this.lblRes.Size = new System.Drawing.Size(110, 13);
            this.lblRes.TabIndex = 2;
            this.lblRes.Text = "Resolution: 480 x 270";
            // 
            // lblScale
            // 
            this.lblScale.AutoSize = true;
            this.lblScale.Location = new System.Drawing.Point(12, 57);
            this.lblScale.Name = "lblScale";
            this.lblScale.Size = new System.Drawing.Size(122, 13);
            this.lblScale.TabIndex = 3;
            this.lblScale.Text = "Capture Area: 480 x 270";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 105);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Image Quality: 50%";
            // 
            // tbQuality
            // 
            this.tbQuality.Location = new System.Drawing.Point(12, 124);
            this.tbQuality.Maximum = 95;
            this.tbQuality.Minimum = 10;
            this.tbQuality.Name = "tbQuality";
            this.tbQuality.Size = new System.Drawing.Size(245, 45);
            this.tbQuality.TabIndex = 5;
            this.tbQuality.TickFrequency = 5;
            this.tbQuality.Value = 50;
            this.tbQuality.Scroll += new System.EventHandler(this.tbQuality_Scroll);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 194);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "LEFT: Jon3";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(98, 194);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "RIGHT: Jon2";
			// 
            // ip address
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 174);
            this.label4.Name = "IP:";
            this.label4.Size = new System.Drawing.Size(44, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "IP:";
			
			
				
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(279, 229);
			this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbQuality);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblScale);
            this.Controls.Add(this.lblRes);
            this.Controls.Add(this.tbArea);
            this.Controls.Add(this.tbResolution);
            this.Name = "frmMain";
            this.Text = "Vita Remote Sever";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.tbResolution)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbArea)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbQuality)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TrackBar tbResolution;
        private System.Windows.Forms.TrackBar tbArea;
        private System.Windows.Forms.Label lblRes;
        private System.Windows.Forms.Label lblScale;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar tbQuality;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;

    }
}

