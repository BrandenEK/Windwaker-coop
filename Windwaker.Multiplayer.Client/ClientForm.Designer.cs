using System.Drawing;
using System.Windows.Forms;

namespace Windwaker.Multiplayer.Client
{
    partial class ClientForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClientForm));
            connectBtn = new Button();
            serverIpField = new TextBox();
            sideServerHeader = new Label();
            serverPortField = new TextBox();
            passwordField = new TextBox();
            sidePlayerHeader = new Label();
            playerNameField = new TextBox();
            oldgame = new ComboBox();
            serverIpLabel = new Label();
            serverPortLabel = new Label();
            passwordLabel = new Label();
            gameNameLabel = new Label();
            playerNameLabel = new Label();
            sidePanel = new Panel();
            sidePanelConnect = new Panel();
            sideServerPanel = new Panel();
            sideRoomPanel = new Panel();
            gameNameField = new TextBox();
            sideRoomHeader = new Label();
            sidePlayerPanel = new Panel();
            debugText = new RichTextBox();
            statusBox = new Panel();
            syncInventory = new CheckBox();
            sideSyncHeader = new Label();
            syncEquipment = new CheckBox();
            syncCharts = new CheckBox();
            syncWorld = new CheckBox();
            syncStats = new CheckBox();
            syncKeys = new CheckBox();
            sidePanel.SuspendLayout();
            sidePanelConnect.SuspendLayout();
            sideServerPanel.SuspendLayout();
            sideRoomPanel.SuspendLayout();
            sidePlayerPanel.SuspendLayout();
            SuspendLayout();
            // 
            // connectBtn
            // 
            connectBtn.Location = new Point(248, 34);
            connectBtn.Name = "connectBtn";
            connectBtn.Size = new Size(120, 24);
            connectBtn.TabIndex = 5;
            connectBtn.Text = "Connect";
            connectBtn.UseVisualStyleBackColor = true;
            connectBtn.Click += OnClickConnect;
            // 
            // serverIpField
            // 
            serverIpField.Location = new Point(12, 58);
            serverIpField.MaxLength = 15;
            serverIpField.Name = "serverIpField";
            serverIpField.PlaceholderText = "127.0.0.1";
            serverIpField.Size = new Size(90, 23);
            serverIpField.TabIndex = 1;
            // 
            // sideServerHeader
            // 
            sideServerHeader.Dock = DockStyle.Top;
            sideServerHeader.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            sideServerHeader.Location = new Point(0, 0);
            sideServerHeader.Name = "sideServerHeader";
            sideServerHeader.Size = new Size(160, 30);
            sideServerHeader.TabIndex = 99;
            sideServerHeader.Text = "Server info";
            sideServerHeader.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // serverPortField
            // 
            serverPortField.Location = new Point(108, 58);
            serverPortField.MaxLength = 5;
            serverPortField.Name = "serverPortField";
            serverPortField.PlaceholderText = "8989";
            serverPortField.Size = new Size(40, 23);
            serverPortField.TabIndex = 2;
            // 
            // passwordField
            // 
            passwordField.Location = new Point(12, 104);
            passwordField.MaxLength = 16;
            passwordField.Name = "passwordField";
            passwordField.PlaceholderText = "No password";
            passwordField.Size = new Size(136, 23);
            passwordField.TabIndex = 3;
            // 
            // sidePlayerHeader
            // 
            sidePlayerHeader.Dock = DockStyle.Top;
            sidePlayerHeader.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            sidePlayerHeader.Location = new Point(0, 0);
            sidePlayerHeader.Name = "sidePlayerHeader";
            sidePlayerHeader.Size = new Size(160, 30);
            sidePlayerHeader.TabIndex = 99;
            sidePlayerHeader.Text = "Player info";
            sidePlayerHeader.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // playerNameField
            // 
            playerNameField.Location = new Point(12, 58);
            playerNameField.MaxLength = 16;
            playerNameField.Name = "playerNameField";
            playerNameField.PlaceholderText = "New player";
            playerNameField.Size = new Size(136, 23);
            playerNameField.TabIndex = 0;
            // 
            // oldgame
            // 
            oldgame.FormattingEnabled = true;
            oldgame.Items.AddRange(new object[] { "Windwaker", "Ocarina of Time", "Zelda I", "Zelda II" });
            oldgame.Location = new Point(512, 25);
            oldgame.Name = "oldgame";
            oldgame.Size = new Size(136, 23);
            oldgame.TabIndex = 101;
            oldgame.Visible = false;
            // 
            // serverIpLabel
            // 
            serverIpLabel.AutoSize = true;
            serverIpLabel.Location = new Point(12, 40);
            serverIpLabel.Name = "serverIpLabel";
            serverIpLabel.Size = new Size(63, 15);
            serverIpLabel.TabIndex = 102;
            serverIpLabel.Text = "Ip address:";
            // 
            // serverPortLabel
            // 
            serverPortLabel.AutoSize = true;
            serverPortLabel.Location = new Point(108, 40);
            serverPortLabel.Name = "serverPortLabel";
            serverPortLabel.Size = new Size(32, 15);
            serverPortLabel.TabIndex = 103;
            serverPortLabel.Text = "Port:";
            // 
            // passwordLabel
            // 
            passwordLabel.AutoSize = true;
            passwordLabel.Location = new Point(12, 86);
            passwordLabel.Name = "passwordLabel";
            passwordLabel.Size = new Size(60, 15);
            passwordLabel.TabIndex = 104;
            passwordLabel.Text = "Password:";
            // 
            // gameNameLabel
            // 
            gameNameLabel.AutoSize = true;
            gameNameLabel.Location = new Point(12, 40);
            gameNameLabel.Name = "gameNameLabel";
            gameNameLabel.Size = new Size(74, 15);
            gameNameLabel.TabIndex = 105;
            gameNameLabel.Text = "Game name:";
            // 
            // playerNameLabel
            // 
            playerNameLabel.AutoSize = true;
            playerNameLabel.Location = new Point(12, 40);
            playerNameLabel.Name = "playerNameLabel";
            playerNameLabel.Size = new Size(75, 15);
            playerNameLabel.TabIndex = 106;
            playerNameLabel.Text = "Player name:";
            // 
            // sidePanel
            // 
            sidePanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            sidePanel.BackColor = SystemColors.ActiveCaptionText;
            sidePanel.Controls.Add(sidePanelConnect);
            sidePanel.Controls.Add(sideServerPanel);
            sidePanel.Controls.Add(sideRoomPanel);
            sidePanel.Controls.Add(sidePlayerPanel);
            sidePanel.Location = new Point(0, 0);
            sidePanel.Name = "sidePanel";
            sidePanel.Size = new Size(166, 570);
            sidePanel.TabIndex = 107;
            // 
            // sidePanelConnect
            // 
            sidePanelConnect.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            sidePanelConnect.BackColor = SystemColors.ControlDark;
            sidePanelConnect.Controls.Add(syncKeys);
            sidePanelConnect.Controls.Add(syncStats);
            sidePanelConnect.Controls.Add(syncWorld);
            sidePanelConnect.Controls.Add(syncCharts);
            sidePanelConnect.Controls.Add(syncEquipment);
            sidePanelConnect.Controls.Add(sideSyncHeader);
            sidePanelConnect.Controls.Add(syncInventory);
            sidePanelConnect.Location = new Point(3, 362);
            sidePanelConnect.Name = "sidePanelConnect";
            sidePanelConnect.Size = new Size(160, 196);
            sidePanelConnect.TabIndex = 111;
            // 
            // sideServerPanel
            // 
            sideServerPanel.BackColor = SystemColors.ControlDark;
            sideServerPanel.Controls.Add(sideServerHeader);
            sideServerPanel.Controls.Add(serverPortLabel);
            sideServerPanel.Controls.Add(serverIpLabel);
            sideServerPanel.Controls.Add(serverIpField);
            sideServerPanel.Controls.Add(serverPortField);
            sideServerPanel.Location = new Point(3, 259);
            sideServerPanel.Name = "sideServerPanel";
            sideServerPanel.Size = new Size(160, 100);
            sideServerPanel.TabIndex = 110;
            // 
            // sideRoomPanel
            // 
            sideRoomPanel.BackColor = SystemColors.ControlDark;
            sideRoomPanel.Controls.Add(gameNameField);
            sideRoomPanel.Controls.Add(sideRoomHeader);
            sideRoomPanel.Controls.Add(gameNameLabel);
            sideRoomPanel.Controls.Add(passwordLabel);
            sideRoomPanel.Controls.Add(passwordField);
            sideRoomPanel.Location = new Point(3, 106);
            sideRoomPanel.Name = "sideRoomPanel";
            sideRoomPanel.Size = new Size(160, 150);
            sideRoomPanel.TabIndex = 109;
            // 
            // gameNameField
            // 
            gameNameField.Location = new Point(12, 58);
            gameNameField.MaxLength = 16;
            gameNameField.Name = "gameNameField";
            gameNameField.PlaceholderText = "Windwaker";
            gameNameField.Size = new Size(136, 23);
            gameNameField.TabIndex = 108;
            // 
            // sideRoomHeader
            // 
            sideRoomHeader.Dock = DockStyle.Top;
            sideRoomHeader.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            sideRoomHeader.Location = new Point(0, 0);
            sideRoomHeader.Name = "sideRoomHeader";
            sideRoomHeader.Size = new Size(160, 30);
            sideRoomHeader.TabIndex = 107;
            sideRoomHeader.Text = "Room info";
            sideRoomHeader.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // sidePlayerPanel
            // 
            sidePlayerPanel.BackColor = SystemColors.ControlDark;
            sidePlayerPanel.Controls.Add(playerNameLabel);
            sidePlayerPanel.Controls.Add(playerNameField);
            sidePlayerPanel.Controls.Add(sidePlayerHeader);
            sidePlayerPanel.Location = new Point(3, 3);
            sidePlayerPanel.Name = "sidePlayerPanel";
            sidePlayerPanel.Size = new Size(160, 100);
            sidePlayerPanel.TabIndex = 108;
            // 
            // debugText
            // 
            debugText.BackColor = Color.FromArgb(40, 40, 40);
            debugText.BorderStyle = BorderStyle.FixedSingle;
            debugText.Location = new Point(672, 12);
            debugText.Name = "debugText";
            debugText.ReadOnly = true;
            debugText.ScrollBars = RichTextBoxScrollBars.Vertical;
            debugText.Size = new Size(300, 537);
            debugText.TabIndex = 108;
            debugText.Text = "";
            // 
            // statusBox
            // 
            statusBox.BackColor = Color.Red;
            statusBox.BorderStyle = BorderStyle.Fixed3D;
            statusBox.Location = new Point(172, 12);
            statusBox.Name = "statusBox";
            statusBox.Size = new Size(60, 60);
            statusBox.TabIndex = 109;
            // 
            // syncInventory
            // 
            syncInventory.AutoSize = true;
            syncInventory.Location = new Point(12, 40);
            syncInventory.Name = "syncInventory";
            syncInventory.Size = new Size(108, 19);
            syncInventory.TabIndex = 102;
            syncInventory.Text = "Inventory items";
            syncInventory.UseVisualStyleBackColor = true;
            // 
            // sideSyncHeader
            // 
            sideSyncHeader.Dock = DockStyle.Top;
            sideSyncHeader.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            sideSyncHeader.Location = new Point(0, 0);
            sideSyncHeader.Name = "sideSyncHeader";
            sideSyncHeader.Size = new Size(160, 30);
            sideSyncHeader.TabIndex = 104;
            sideSyncHeader.Text = "Sync info";
            sideSyncHeader.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // syncEquipment
            // 
            syncEquipment.AutoSize = true;
            syncEquipment.Location = new Point(12, 65);
            syncEquipment.Name = "syncEquipment";
            syncEquipment.Size = new Size(116, 19);
            syncEquipment.TabIndex = 105;
            syncEquipment.Text = "Equipment items";
            syncEquipment.UseVisualStyleBackColor = true;
            // 
            // syncCharts
            // 
            syncCharts.AutoSize = true;
            syncCharts.Location = new Point(12, 115);
            syncCharts.Name = "syncCharts";
            syncCharts.Size = new Size(100, 19);
            syncCharts.TabIndex = 106;
            syncCharts.Text = "Map && Charts";
            syncCharts.UseVisualStyleBackColor = true;
            // 
            // syncWorld
            // 
            syncWorld.AutoSize = true;
            syncWorld.Location = new Point(12, 140);
            syncWorld.Name = "syncWorld";
            syncWorld.Size = new Size(87, 19);
            syncWorld.TabIndex = 107;
            syncWorld.Text = "World State";
            syncWorld.UseVisualStyleBackColor = true;
            // 
            // syncStats
            // 
            syncStats.AutoSize = true;
            syncStats.Location = new Point(12, 90);
            syncStats.Name = "syncStats";
            syncStats.Size = new Size(86, 19);
            syncStats.TabIndex = 108;
            syncStats.Text = "Player Stats";
            syncStats.UseVisualStyleBackColor = true;
            // 
            // syncKeys
            // 
            syncKeys.AutoSize = true;
            syncKeys.Location = new Point(12, 165);
            syncKeys.Name = "syncKeys";
            syncKeys.Size = new Size(82, 19);
            syncKeys.TabIndex = 109;
            syncKeys.Text = "Small Keys";
            syncKeys.UseVisualStyleBackColor = true;
            // 
            // ClientForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlDarkDark;
            ClientSize = new Size(984, 561);
            Controls.Add(statusBox);
            Controls.Add(connectBtn);
            Controls.Add(debugText);
            Controls.Add(sidePanel);
            Controls.Add(oldgame);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(1000, 600);
            Name = "ClientForm";
            Text = "Windwaker Multiplayer Client";
            FormClosing += OnFormClose;
            Load += OnFormOpen;
            sidePanel.ResumeLayout(false);
            sidePanelConnect.ResumeLayout(false);
            sidePanelConnect.PerformLayout();
            sideServerPanel.ResumeLayout(false);
            sideServerPanel.PerformLayout();
            sideRoomPanel.ResumeLayout(false);
            sideRoomPanel.PerformLayout();
            sidePlayerPanel.ResumeLayout(false);
            sidePlayerPanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private Button connectBtn;
        private TextBox serverIpField;
        private Label sideServerHeader;
        private TextBox serverPortField;
        private TextBox passwordField;
        private Label sidePlayerHeader;
        private TextBox playerNameField;
        private ComboBox oldgame;
        private Label serverIpLabel;
        private Label serverPortLabel;
        private Label passwordLabel;
        private Label gameNameLabel;
        private Label playerNameLabel;
        private Panel sidePanel;
        private Panel sidePlayerPanel;
        private Panel sideRoomPanel;
        private Label sideRoomHeader;
        private Panel sideServerPanel;
        private Panel sidePanelConnect;
        private TextBox gameNameField;
        private RichTextBox debugText;
        private Panel statusBox;
        private Label sideSyncHeader;
        private CheckBox syncInventory;
        private CheckBox syncKeys;
        private CheckBox syncStats;
        private CheckBox syncWorld;
        private CheckBox syncCharts;
        private CheckBox syncEquipment;
    }
}