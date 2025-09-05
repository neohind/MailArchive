namespace MailAchiveUi
{
    partial class FrmMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txtUrl = new TextBox();
            txtId = new TextBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            txtPassword = new TextBox();
            label4 = new Label();
            txtPort = new TextBox();
            chkUseSSL = new CheckBox();
            btnLoad = new Button();
            progressBar1 = new ProgressBar();
            txtLogs = new TextBox();
            SuspendLayout();
            // 
            // txtUrl
            // 
            txtUrl.Location = new Point(81, 24);
            txtUrl.Name = "txtUrl";
            txtUrl.Size = new Size(303, 23);
            txtUrl.TabIndex = 0;
            // 
            // txtId
            // 
            txtId.Location = new Point(81, 107);
            txtId.Name = "txtId";
            txtId.Size = new Size(221, 23);
            txtId.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 27);
            label1.Name = "label1";
            label1.Size = new Size(28, 15);
            label1.TabIndex = 1;
            label1.Text = "URL";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 110);
            label2.Name = "label2";
            label2.Size = new Size(19, 15);
            label2.TabIndex = 1;
            label2.Text = "ID";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 140);
            label3.Name = "label3";
            label3.Size = new Size(57, 15);
            label3.TabIndex = 1;
            label3.Text = "Password";
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(81, 137);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(221, 23);
            txtPassword.TabIndex = 0;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 62);
            label4.Name = "label4";
            label4.Size = new Size(29, 15);
            label4.TabIndex = 1;
            label4.Text = "Port";
            // 
            // txtPort
            // 
            txtPort.Location = new Point(81, 59);
            txtPort.Name = "txtPort";
            txtPort.Size = new Size(84, 23);
            txtPort.TabIndex = 0;
            // 
            // chkUseSSL
            // 
            chkUseSSL.AutoSize = true;
            chkUseSSL.Location = new Point(200, 61);
            chkUseSSL.Name = "chkUseSSL";
            chkUseSSL.Size = new Size(75, 19);
            chkUseSSL.TabIndex = 2;
            chkUseSSL.Text = "Use SSL?";
            chkUseSSL.UseVisualStyleBackColor = true;
            // 
            // btnLoad
            // 
            btnLoad.Location = new Point(13, 219);
            btnLoad.Name = "btnLoad";
            btnLoad.Size = new Size(75, 23);
            btnLoad.TabIndex = 3;
            btnLoad.Text = "Load";
            btnLoad.UseVisualStyleBackColor = true;
            btnLoad.Click += btnLoad_Click;
            // 
            // progressBar1
            // 
            progressBar1.Dock = DockStyle.Bottom;
            progressBar1.Location = new Point(0, 427);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(800, 23);
            progressBar1.TabIndex = 4;
            // 
            // txtLogs
            // 
            txtLogs.Dock = DockStyle.Bottom;
            txtLogs.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtLogs.Location = new Point(0, 248);
            txtLogs.Multiline = true;
            txtLogs.Name = "txtLogs";
            txtLogs.Size = new Size(800, 179);
            txtLogs.TabIndex = 5;
            // 
            // FrmMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(txtLogs);
            Controls.Add(progressBar1);
            Controls.Add(btnLoad);
            Controls.Add(chkUseSSL);
            Controls.Add(label3);
            Controls.Add(label4);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(txtPassword);
            Controls.Add(txtId);
            Controls.Add(txtPort);
            Controls.Add(txtUrl);
            Name = "FrmMain";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtUrl;
        private TextBox txtId;
        private Label label1;
        private Label label2;
        private Label label3;
        private TextBox txtPassword;
        private Label label4;
        private TextBox txtPort;
        private CheckBox chkUseSSL;
        private Button btnLoad;
        private ProgressBar progressBar1;
        private TextBox txtLogs;
    }
}
