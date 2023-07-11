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
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(984, 561);
            Controls.Add(mainLbl);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MainForm";
            Text = "Windwaker Multiplayer Client";
            ResumeLayout(false);
        }

        #endregion

        private Label mainLbl;
    }
}