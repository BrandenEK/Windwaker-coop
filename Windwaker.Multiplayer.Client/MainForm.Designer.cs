namespace Windwaker.Multiplayer.Client
{
    partial class MainForm
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
            logInner = new System.Windows.Forms.RichTextBox();
            logOuter = new System.Windows.Forms.Panel();
            connectBtn = new System.Windows.Forms.Button();
            logOuter.SuspendLayout();
            SuspendLayout();
            // 
            // logInner
            // 
            logInner.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            logInner.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            logInner.BorderStyle = System.Windows.Forms.BorderStyle.None;
            logInner.Location = new System.Drawing.Point(2, 2);
            logInner.Name = "logInner";
            logInner.ReadOnly = true;
            logInner.Size = new System.Drawing.Size(296, 396);
            logInner.TabIndex = 0;
            logInner.Text = "";
            // 
            // logOuter
            // 
            logOuter.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            logOuter.BackColor = System.Drawing.Color.Black;
            logOuter.Controls.Add(logInner);
            logOuter.Location = new System.Drawing.Point(670, 150);
            logOuter.Name = "logOuter";
            logOuter.Size = new System.Drawing.Size(300, 400);
            logOuter.TabIndex = 1;
            // 
            // connectBtn
            // 
            connectBtn.Location = new System.Drawing.Point(55, 30);
            connectBtn.Name = "connectBtn";
            connectBtn.Size = new System.Drawing.Size(100, 25);
            connectBtn.TabIndex = 2;
            connectBtn.Text = "Connect";
            connectBtn.UseVisualStyleBackColor = true;
            connectBtn.Click += OnConnectBtnClicked;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(984, 561);
            Controls.Add(connectBtn);
            Controls.Add(logOuter);
            MinimumSize = new System.Drawing.Size(1000, 600);
            Name = "MainForm";
            Text = "Windwaker Multiplayer Client";
            logOuter.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.RichTextBox logInner;
        private System.Windows.Forms.Panel logOuter;
        private System.Windows.Forms.Button connectBtn;
    }
}