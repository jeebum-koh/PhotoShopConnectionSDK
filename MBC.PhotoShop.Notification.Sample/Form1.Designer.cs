namespace MBC.PhotoShop.Notification.Sample
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.txtIPAddress = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.pbForegroundColor = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.pbBackgroundColor = new System.Windows.Forms.PictureBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtActiveToolName = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbForegroundColor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbBackgroundColor)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP Address";
            // 
            // txtIPAddress
            // 
            this.txtIPAddress.Location = new System.Drawing.Point(88, 23);
            this.txtIPAddress.Name = "txtIPAddress";
            this.txtIPAddress.Size = new System.Drawing.Size(183, 21);
            this.txtIPAddress.TabIndex = 1;
            this.txtIPAddress.Text = "127.0.0.1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "Password";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(88, 53);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(183, 21);
            this.txtPassword.TabIndex = 1;
            this.txtPassword.Text = "kohjeebum";
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(14, 80);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(257, 23);
            this.btnStart.TabIndex = 2;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(314, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(103, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "Foreground Color";
            // 
            // pbForegroundColor
            // 
            this.pbForegroundColor.Location = new System.Drawing.Point(316, 24);
            this.pbForegroundColor.Name = "pbForegroundColor";
            this.pbForegroundColor.Size = new System.Drawing.Size(101, 50);
            this.pbForegroundColor.TabIndex = 3;
            this.pbForegroundColor.TabStop = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(423, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(106, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "Background Color";
            // 
            // pbBackgroundColor
            // 
            this.pbBackgroundColor.Location = new System.Drawing.Point(425, 24);
            this.pbBackgroundColor.Name = "pbBackgroundColor";
            this.pbBackgroundColor.Size = new System.Drawing.Size(101, 50);
            this.pbBackgroundColor.TabIndex = 3;
            this.pbBackgroundColor.TabStop = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(314, 85);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "Active Tool";
            // 
            // txtActiveToolName
            // 
            this.txtActiveToolName.Location = new System.Drawing.Point(316, 100);
            this.txtActiveToolName.Name = "txtActiveToolName";
            this.txtActiveToolName.ReadOnly = true;
            this.txtActiveToolName.Size = new System.Drawing.Size(210, 21);
            this.txtActiveToolName.TabIndex = 4;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(544, 133);
            this.Controls.Add(this.txtActiveToolName);
            this.Controls.Add(this.pbBackgroundColor);
            this.Controls.Add(this.pbForegroundColor);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.txtIPAddress);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PhotoShop Notification Handling Sample";
            ((System.ComponentModel.ISupportInitialize)(this.pbForegroundColor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbBackgroundColor)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtIPAddress;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PictureBox pbForegroundColor;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox pbBackgroundColor;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtActiveToolName;
    }
}

