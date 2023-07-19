using System.Drawing;
using System.Windows.Forms;

namespace Windwaker.Multiplayer.Server
{
    partial class ServerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServerForm));
            debugText = new TextBox();
            maxPlayersLabel = new Label();
            maxPlayersField = new TextBox();
            serverPortField = new TextBox();
            serverPortLabel = new Label();
            gameNameLabel = new Label();
            gameNameField = new TextBox();
            passwordLabel = new Label();
            passwordField = new TextBox();
            startButton = new Button();
            playerGridOuter = new Panel();
            playerGridInner = new Panel();
            playerGridDividerH = new Panel();
            playerGridDividerV = new Panel();
            playerGridHeader = new Panel();
            playerGridHeaderLocation = new Label();
            playerGridHeaderName = new Label();
            connectedPlayersLabel = new Label();
            playerGridOuter.SuspendLayout();
            playerGridInner.SuspendLayout();
            playerGridHeader.SuspendLayout();
            SuspendLayout();
            // 
            // debugText
            // 
            debugText.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            debugText.BackColor = SystemColors.AppWorkspace;
            debugText.Location = new Point(672, 299);
            debugText.Multiline = true;
            debugText.Name = "debugText";
            debugText.ReadOnly = true;
            debugText.Size = new Size(300, 250);
            debugText.TabIndex = 100;
            // 
            // maxPlayersLabel
            // 
            maxPlayersLabel.AutoSize = true;
            maxPlayersLabel.Location = new Point(25, 40);
            maxPlayersLabel.Name = "maxPlayersLabel";
            maxPlayersLabel.Size = new Size(73, 15);
            maxPlayersLabel.TabIndex = 101;
            maxPlayersLabel.Text = "Max players:";
            // 
            // maxPlayersField
            // 
            maxPlayersField.Location = new Point(25, 58);
            maxPlayersField.MaxLength = 2;
            maxPlayersField.Name = "maxPlayersField";
            maxPlayersField.PlaceholderText = "8";
            maxPlayersField.Size = new Size(40, 23);
            maxPlayersField.TabIndex = 102;
            // 
            // serverPortField
            // 
            serverPortField.Location = new Point(25, 222);
            serverPortField.MaxLength = 5;
            serverPortField.Name = "serverPortField";
            serverPortField.PlaceholderText = "8989";
            serverPortField.Size = new Size(40, 23);
            serverPortField.TabIndex = 103;
            // 
            // serverPortLabel
            // 
            serverPortLabel.AutoSize = true;
            serverPortLabel.Location = new Point(25, 204);
            serverPortLabel.Name = "serverPortLabel";
            serverPortLabel.Size = new Size(67, 15);
            serverPortLabel.TabIndex = 104;
            serverPortLabel.Text = "Server port:";
            // 
            // gameNameLabel
            // 
            gameNameLabel.AutoSize = true;
            gameNameLabel.Location = new Point(25, 93);
            gameNameLabel.Name = "gameNameLabel";
            gameNameLabel.Size = new Size(74, 15);
            gameNameLabel.TabIndex = 106;
            gameNameLabel.Text = "Game name:";
            // 
            // gameNameField
            // 
            gameNameField.Location = new Point(25, 111);
            gameNameField.MaxLength = 16;
            gameNameField.Name = "gameNameField";
            gameNameField.PlaceholderText = "Windwaker";
            gameNameField.Size = new Size(136, 23);
            gameNameField.TabIndex = 105;
            // 
            // passwordLabel
            // 
            passwordLabel.AutoSize = true;
            passwordLabel.Location = new Point(25, 147);
            passwordLabel.Name = "passwordLabel";
            passwordLabel.Size = new Size(60, 15);
            passwordLabel.TabIndex = 108;
            passwordLabel.Text = "Password:";
            // 
            // passwordField
            // 
            passwordField.Location = new Point(25, 165);
            passwordField.MaxLength = 16;
            passwordField.Name = "passwordField";
            passwordField.PlaceholderText = "No password";
            passwordField.Size = new Size(136, 23);
            passwordField.TabIndex = 107;
            // 
            // startButton
            // 
            startButton.Location = new Point(33, 271);
            startButton.Name = "startButton";
            startButton.Size = new Size(120, 24);
            startButton.TabIndex = 109;
            startButton.Text = "Start";
            startButton.UseVisualStyleBackColor = true;
            startButton.Click += OnClickStart;
            // 
            // playerGridOuter
            // 
            playerGridOuter.BackColor = SystemColors.ActiveCaptionText;
            playerGridOuter.Controls.Add(playerGridInner);
            playerGridOuter.Location = new Point(748, 12);
            playerGridOuter.Name = "playerGridOuter";
            playerGridOuter.Size = new Size(224, 174);
            playerGridOuter.TabIndex = 111;
            // 
            // playerGridInner
            // 
            playerGridInner.BackColor = SystemColors.Control;
            playerGridInner.Controls.Add(playerGridDividerH);
            playerGridInner.Controls.Add(playerGridDividerV);
            playerGridInner.Controls.Add(playerGridHeader);
            playerGridInner.Location = new Point(2, 2);
            playerGridInner.Name = "playerGridInner";
            playerGridInner.Size = new Size(220, 170);
            playerGridInner.TabIndex = 0;
            // 
            // playerGridDividerH
            // 
            playerGridDividerH.BackColor = SystemColors.ActiveCaptionText;
            playerGridDividerH.Location = new Point(0, 39);
            playerGridDividerH.Name = "playerGridDividerH";
            playerGridDividerH.Size = new Size(220, 2);
            playerGridDividerH.TabIndex = 2;
            // 
            // playerGridDividerV
            // 
            playerGridDividerV.BackColor = SystemColors.ActiveCaptionText;
            playerGridDividerV.Location = new Point(109, 0);
            playerGridDividerV.Name = "playerGridDividerV";
            playerGridDividerV.Size = new Size(2, 170);
            playerGridDividerV.TabIndex = 1;
            // 
            // playerGridHeader
            // 
            playerGridHeader.BackColor = SystemColors.ControlDark;
            playerGridHeader.Controls.Add(playerGridHeaderLocation);
            playerGridHeader.Controls.Add(playerGridHeaderName);
            playerGridHeader.Location = new Point(0, 0);
            playerGridHeader.Name = "playerGridHeader";
            playerGridHeader.Size = new Size(220, 40);
            playerGridHeader.TabIndex = 0;
            // 
            // playerGridHeaderLocation
            // 
            playerGridHeaderLocation.Location = new Point(110, 0);
            playerGridHeaderLocation.Name = "playerGridHeaderLocation";
            playerGridHeaderLocation.Size = new Size(110, 40);
            playerGridHeaderLocation.TabIndex = 1;
            playerGridHeaderLocation.Text = "Location";
            playerGridHeaderLocation.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // playerGridHeaderName
            // 
            playerGridHeaderName.Location = new Point(0, 0);
            playerGridHeaderName.Name = "playerGridHeaderName";
            playerGridHeaderName.Size = new Size(110, 40);
            playerGridHeaderName.TabIndex = 0;
            playerGridHeaderName.Text = "Player";
            playerGridHeaderName.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // connectedPlayersLabel
            // 
            connectedPlayersLabel.AutoSize = true;
            connectedPlayersLabel.Location = new Point(349, 40);
            connectedPlayersLabel.Name = "connectedPlayersLabel";
            connectedPlayersLabel.Size = new Size(128, 15);
            connectedPlayersLabel.TabIndex = 112;
            connectedPlayersLabel.Text = "Connected players: 0/8";
            // 
            // ServerForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlDarkDark;
            ClientSize = new Size(984, 561);
            Controls.Add(connectedPlayersLabel);
            Controls.Add(playerGridOuter);
            Controls.Add(startButton);
            Controls.Add(passwordLabel);
            Controls.Add(passwordField);
            Controls.Add(gameNameLabel);
            Controls.Add(gameNameField);
            Controls.Add(serverPortLabel);
            Controls.Add(serverPortField);
            Controls.Add(maxPlayersField);
            Controls.Add(maxPlayersLabel);
            Controls.Add(debugText);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(1000, 600);
            Name = "ServerForm";
            Text = "Windwaker Multiplayer Server";
            FormClosing += OnFormClose;
            Load += OnFormOpen;
            playerGridOuter.ResumeLayout(false);
            playerGridInner.ResumeLayout(false);
            playerGridHeader.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox debugText;
        private Label maxPlayersLabel;
        private TextBox maxPlayersField;
        private TextBox serverPortField;
        private Label serverPortLabel;
        private Label gameNameLabel;
        private TextBox gameNameField;
        private Label passwordLabel;
        private TextBox passwordField;
        private Button startButton;
        private Panel playerGridOuter;
        private Panel playerGridInner;
        private Panel playerGridHeader;
        private Label playerGridHeaderLocation;
        private Label playerGridHeaderName;
        private Panel playerGridDividerH;
        private Panel playerGridDividerV;
        private Label connectedPlayersLabel;
    }
}