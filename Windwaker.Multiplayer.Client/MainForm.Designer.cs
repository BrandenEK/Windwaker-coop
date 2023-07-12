using System.Drawing;
using System.Windows.Forms;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            mainLbl = new Label();
            connectBtn = new Button();
            serverText = new TextBox();
            serverLbl = new Label();
            debugText = new TextBox();
            SuspendLayout();
            // 
            // mainLbl
            // 
            mainLbl.Dock = DockStyle.Fill;
            mainLbl.Font = new Font("Segoe UI", 24F, FontStyle.Regular, GraphicsUnit.Point);
            mainLbl.Location = new Point(0, 0);
            mainLbl.Name = "mainLbl";
            mainLbl.Size = new Size(984, 561);
            mainLbl.TabIndex = 0;
            mainLbl.Text = "Windwaker-coop";
            mainLbl.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // connectBtn
            // 
            connectBtn.Location = new Point(622, 74);
            connectBtn.Name = "connectBtn";
            connectBtn.Size = new Size(120, 24);
            connectBtn.TabIndex = 1;
            connectBtn.Text = "Connect";
            connectBtn.UseVisualStyleBackColor = true;
            connectBtn.Click += OnClickConnect;
            // 
            // serverText
            // 
            serverText.Location = new Point(604, 45);
            serverText.Name = "serverText";
            serverText.Size = new Size(154, 23);
            serverText.TabIndex = 2;
            serverText.Text = "192.168.1.166:8989";
            // 
            // serverLbl
            // 
            serverLbl.AutoSize = true;
            serverLbl.Location = new Point(604, 27);
            serverLbl.Name = "serverLbl";
            serverLbl.Size = new Size(77, 15);
            serverLbl.TabIndex = 3;
            serverLbl.Text = "Server IpPort:";
            // 
            // debugText
            // 
            debugText.BackColor = SystemColors.ControlLight;
            debugText.Location = new Point(12, 300);
            debugText.Multiline = true;
            debugText.Name = "debugText";
            debugText.ReadOnly = true;
            debugText.Size = new Size(300, 250);
            debugText.TabIndex = 4;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(984, 561);
            Controls.Add(debugText);
            Controls.Add(serverLbl);
            Controls.Add(serverText);
            Controls.Add(connectBtn);
            Controls.Add(mainLbl);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MainForm";
            Text = "Windwaker Multiplayer Client";
            FormClosing += OnFormClose;
            Load += OnFormOpen;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label mainLbl;
        private Button connectBtn;
        private TextBox serverText;
        private Label serverLbl;
        private TextBox debugText;
    }
}