namespace TDFDow30
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.lblHostName = new System.Windows.Forms.Label();
            this.lblIpAddress = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblLogResp = new System.Windows.Forms.Label();
            this.ConnectButton = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.PWlabel = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.PWTextBox = new System.Windows.Forms.TextBox();
            this.Userlabel = new System.Windows.Forms.Label();
            this.UserTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.PortTextBox = new System.Windows.Forms.TextBox();
            this.IPlabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.IPTextBox = new System.Windows.Forms.TextBox();
            this.gbTimeOfLastDelete = new System.Windows.Forms.GroupBox();
            this.lblReceivedTime = new System.Windows.Forms.Label();
            this.gbTime = new System.Windows.Forms.GroupBox();
            this.timeOfDayLabel = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.button4 = new System.Windows.Forms.Button();
            this.TODTimer = new System.Windows.Forms.Timer(this.components);
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.groupBox5.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.gbTimeOfLastDelete.SuspendLayout();
            this.gbTime.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.lblHostName);
            this.groupBox5.Controls.Add(this.lblIpAddress);
            this.groupBox5.Location = new System.Drawing.Point(561, 12);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(423, 38);
            this.groupBox5.TabIndex = 172;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Host PC Info";
            // 
            // lblHostName
            // 
            this.lblHostName.AutoSize = true;
            this.lblHostName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHostName.Location = new System.Drawing.Point(111, 16);
            this.lblHostName.Name = "lblHostName";
            this.lblHostName.Size = new System.Drawing.Size(34, 16);
            this.lblHostName.TabIndex = 122;
            this.lblHostName.Text = "N/A";
            // 
            // lblIpAddress
            // 
            this.lblIpAddress.AutoSize = true;
            this.lblIpAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIpAddress.Location = new System.Drawing.Point(6, 16);
            this.lblIpAddress.Name = "lblIpAddress";
            this.lblIpAddress.Size = new System.Drawing.Size(34, 16);
            this.lblIpAddress.TabIndex = 121;
            this.lblIpAddress.Text = "N/A";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pictureBox2);
            this.groupBox1.Controls.Add(this.pictureBox1);
            this.groupBox1.Controls.Add(this.lblLogResp);
            this.groupBox1.Controls.Add(this.ConnectButton);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.PWlabel);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.PWTextBox);
            this.groupBox1.Controls.Add(this.Userlabel);
            this.groupBox1.Controls.Add(this.UserTextBox);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.PortTextBox);
            this.groupBox1.Controls.Add(this.IPlabel);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.IPTextBox);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(6, 56);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1110, 85);
            this.groupBox1.TabIndex = 171;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Login Info";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(702, 40);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(16, 16);
            this.pictureBox2.TabIndex = 164;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Visible = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(702, 40);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(16, 16);
            this.pictureBox1.TabIndex = 163;
            this.pictureBox1.TabStop = false;
            // 
            // lblLogResp
            // 
            this.lblLogResp.AutoSize = true;
            this.lblLogResp.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLogResp.Location = new System.Drawing.Point(7, 62);
            this.lblLogResp.Name = "lblLogResp";
            this.lblLogResp.Size = new System.Drawing.Size(130, 16);
            this.lblLogResp.TabIndex = 160;
            this.lblLogResp.Text = "Logon Response:";
            // 
            // ConnectButton
            // 
            this.ConnectButton.Location = new System.Drawing.Point(582, 27);
            this.ConnectButton.Name = "ConnectButton";
            this.ConnectButton.Size = new System.Drawing.Size(105, 39);
            this.ConnectButton.TabIndex = 159;
            this.ConnectButton.Text = "Connect";
            this.ConnectButton.UseVisualStyleBackColor = true;
            this.ConnectButton.Click += new System.EventHandler(this.ConnectButton_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(906, 47);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(113, 16);
            this.label7.TabIndex = 167;
            this.label7.Text = "Num Catalogs: ";
            // 
            // PWlabel
            // 
            this.PWlabel.AutoSize = true;
            this.PWlabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PWlabel.Location = new System.Drawing.Point(437, 17);
            this.PWlabel.Name = "PWlabel";
            this.PWlabel.Size = new System.Drawing.Size(76, 16);
            this.PWlabel.TabIndex = 158;
            this.PWlabel.Text = "Password";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(906, 63);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(94, 16);
            this.label6.TabIndex = 166;
            this.label6.Text = "Num Fields: ";
            // 
            // PWTextBox
            // 
            this.PWTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PWTextBox.Location = new System.Drawing.Point(440, 35);
            this.PWTextBox.Name = "PWTextBox";
            this.PWTextBox.Size = new System.Drawing.Size(126, 23);
            this.PWTextBox.TabIndex = 157;
            // 
            // Userlabel
            // 
            this.Userlabel.AutoSize = true;
            this.Userlabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Userlabel.Location = new System.Drawing.Point(295, 17);
            this.Userlabel.Name = "Userlabel";
            this.Userlabel.Size = new System.Drawing.Size(86, 16);
            this.Userlabel.TabIndex = 156;
            this.Userlabel.Text = "User Name";
            // 
            // UserTextBox
            // 
            this.UserTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UserTextBox.Location = new System.Drawing.Point(298, 35);
            this.UserTextBox.Name = "UserTextBox";
            this.UserTextBox.Size = new System.Drawing.Size(126, 23);
            this.UserTextBox.TabIndex = 155;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(906, 31);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(50, 16);
            this.label5.TabIndex = 162;
            this.label5.Text = "bytes:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(147, 17);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(36, 16);
            this.label4.TabIndex = 154;
            this.label4.Text = "Port";
            // 
            // PortTextBox
            // 
            this.PortTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PortTextBox.Location = new System.Drawing.Point(150, 35);
            this.PortTextBox.Name = "PortTextBox";
            this.PortTextBox.Size = new System.Drawing.Size(126, 23);
            this.PortTextBox.TabIndex = 153;
            // 
            // IPlabel
            // 
            this.IPlabel.AutoSize = true;
            this.IPlabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IPlabel.Location = new System.Drawing.Point(10, 17);
            this.IPlabel.Name = "IPlabel";
            this.IPlabel.Size = new System.Drawing.Size(84, 16);
            this.IPlabel.TabIndex = 152;
            this.IPlabel.Text = "IP Address";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(906, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 16);
            this.label2.TabIndex = 161;
            this.label2.Text = "rCnt:";
            // 
            // IPTextBox
            // 
            this.IPTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IPTextBox.Location = new System.Drawing.Point(8, 35);
            this.IPTextBox.Name = "IPTextBox";
            this.IPTextBox.Size = new System.Drawing.Size(126, 23);
            this.IPTextBox.TabIndex = 151;
            // 
            // gbTimeOfLastDelete
            // 
            this.gbTimeOfLastDelete.Controls.Add(this.lblReceivedTime);
            this.gbTimeOfLastDelete.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.gbTimeOfLastDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbTimeOfLastDelete.Location = new System.Drawing.Point(295, 8);
            this.gbTimeOfLastDelete.Name = "gbTimeOfLastDelete";
            this.gbTimeOfLastDelete.Size = new System.Drawing.Size(260, 42);
            this.gbTimeOfLastDelete.TabIndex = 170;
            this.gbTimeOfLastDelete.TabStop = false;
            this.gbTimeOfLastDelete.Text = "Last Data Received";
            // 
            // lblReceivedTime
            // 
            this.lblReceivedTime.BackColor = System.Drawing.Color.Black;
            this.lblReceivedTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblReceivedTime.ForeColor = System.Drawing.Color.Red;
            this.lblReceivedTime.Location = new System.Drawing.Point(6, 17);
            this.lblReceivedTime.Name = "lblReceivedTime";
            this.lblReceivedTime.Size = new System.Drawing.Size(248, 20);
            this.lblReceivedTime.TabIndex = 0;
            this.lblReceivedTime.Text = "--";
            this.lblReceivedTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // gbTime
            // 
            this.gbTime.Controls.Add(this.timeOfDayLabel);
            this.gbTime.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.gbTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbTime.Location = new System.Drawing.Point(7, 8);
            this.gbTime.Name = "gbTime";
            this.gbTime.Size = new System.Drawing.Size(264, 42);
            this.gbTime.TabIndex = 169;
            this.gbTime.TabStop = false;
            this.gbTime.Text = "Current Time";
            this.gbTime.Enter += new System.EventHandler(this.gbTime_Enter);
            // 
            // timeOfDayLabel
            // 
            this.timeOfDayLabel.BackColor = System.Drawing.Color.Black;
            this.timeOfDayLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.timeOfDayLabel.ForeColor = System.Drawing.Color.Red;
            this.timeOfDayLabel.Location = new System.Drawing.Point(6, 17);
            this.timeOfDayLabel.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.timeOfDayLabel.Name = "timeOfDayLabel";
            this.timeOfDayLabel.Size = new System.Drawing.Size(255, 20);
            this.timeOfDayLabel.TabIndex = 0;
            this.timeOfDayLabel.Text = "--";
            this.timeOfDayLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(36, 147);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(915, 725);
            this.dataGridView1.TabIndex = 173;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(36, 878);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(171, 70);
            this.button1.TabIndex = 174;
            this.button1.Text = "Get DB Table";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(241, 881);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(171, 65);
            this.button2.TabIndex = 175;
            this.button2.Text = "Set Symbol List";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(660, 885);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 17);
            this.label1.TabIndex = 176;
            this.label1.Text = "label1";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(459, 885);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(171, 61);
            this.button3.TabIndex = 177;
            this.button3.Text = "Get Dow 30 Data";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.HorizontalScrollbar = true;
            this.listBox1.Location = new System.Drawing.Point(973, 153);
            this.listBox1.Name = "listBox1";
            this.listBox1.ScrollAlwaysVisible = true;
            this.listBox1.Size = new System.Drawing.Size(216, 719);
            this.listBox1.TabIndex = 178;
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(663, 905);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(99, 36);
            this.button4.TabIndex = 179;
            this.button4.Text = "Test Email";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // TODTimer
            // 
            this.TODTimer.Enabled = true;
            this.TODTimer.Interval = 1000;
            this.TODTimer.Tick += new System.EventHandler(this.TODTimer_Tick);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(1050, 907);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(123, 41);
            this.button5.TabIndex = 180;
            this.button5.Text = "Disconnect";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(828, 897);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(109, 44);
            this.button6.TabIndex = 181;
            this.button6.Text = "Pause";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1223, 967);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gbTimeOfLastDelete);
            this.Controls.Add(this.gbTime);
            this.Name = "frmMain";
            this.Text = "TDF Dow 30";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.gbTimeOfLastDelete.ResumeLayout(false);
            this.gbTime.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label lblHostName;
        private System.Windows.Forms.Label lblIpAddress;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblLogResp;
        private System.Windows.Forms.Button ConnectButton;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label PWlabel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox PWTextBox;
        private System.Windows.Forms.Label Userlabel;
        private System.Windows.Forms.TextBox UserTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox PortTextBox;
        private System.Windows.Forms.Label IPlabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox IPTextBox;
        private System.Windows.Forms.GroupBox gbTimeOfLastDelete;
        private System.Windows.Forms.Label lblReceivedTime;
        private System.Windows.Forms.GroupBox gbTime;
        private System.Windows.Forms.Label timeOfDayLabel;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Timer TODTimer;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
    }
}

