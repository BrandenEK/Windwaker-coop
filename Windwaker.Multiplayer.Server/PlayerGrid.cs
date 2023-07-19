using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Windwaker.Multiplayer.Server
{
    public class PlayerGrid
    {
        public const int ROW_HEIGHT = 25;

        private readonly Panel _gridOuter;
        private readonly Panel _gridInner;

        private readonly Panel _gridHeader;
        private readonly Label _gridHeaderName;
        private readonly Label _gridHeaderLocation;

        private readonly Panel _gridFooter;
        private readonly Label _gridFooterText;

        private readonly Panel _gridDividerV;
        private readonly Panel _gridDividerH;

        private readonly List<PlayerGridRow> _gridRows = new();

        public void UpdateGrid(IEnumerable<PlayerData> players, int count)
        {
            _gridFooterText.Text = $"Connected players: {count}/{ServerForm.Settings.ValidMaxPlayers}";

            // Update location/size of existing ui
            _gridOuter.Height = (count + 2) * ROW_HEIGHT + 4;
            _gridInner.Height = _gridOuter.Height - 4;
            _gridDividerV.Height = _gridInner.Height - ROW_HEIGHT;
            _gridFooter.Location = new Point(0, _gridInner.Height - ROW_HEIGHT);
            _gridFooter.BackColor = count % 2 == 0 ? Color.LightGray : Color.DarkGray;

            // Update location/text/color of rows
            int currIdx = 0;
            foreach (var player in players)
            {
                if (currIdx < _gridRows.Count)
                {
                    _gridRows[currIdx].Update(player.Name, player.CurrentSceneName, currIdx);
                }
                else
                {
                    _gridRows.Add(new PlayerGridRow(_gridInner));
                    _gridRows[currIdx].Update(player.Name, player.CurrentSceneName, currIdx);
                }

                currIdx++;
            }

            // Hide any unused rows
            for (; currIdx < _gridRows.Count; currIdx++)
            {
                _gridRows[currIdx].SetVisibility(false);
            }

            _gridDividerH.BringToFront();
            _gridDividerV.BringToFront();
        }

        public PlayerGrid()
        {
            _gridOuter = new Panel
            {
                Name = "playerGridOuter",
                BackColor = Color.Black,
                Location = new Point(448, 12),
                Size = new Size(224, 2 * ROW_HEIGHT + 4),
            };

            _gridInner = new Panel
            {
                Parent = _gridOuter,
                Name = "playerGridInner",
                BackColor = Color.White,
                Location = new Point(2, 2),
                Size = new Size(_gridOuter.Width - 4, _gridOuter.Height - 4),
            };

            _gridDividerV = new Panel
            {
                Parent = _gridInner,
                Name = "playerGridDividerV",
                BackColor = Color.Black,
                Location = new Point(_gridInner.Width / 2 - 1, 0),
                Size = new Size(2, _gridInner.Height - ROW_HEIGHT),
            };
            _gridDividerH = new Panel
            {
                Parent = _gridInner,
                Name = "playerGridDividerH",
                BackColor = Color.Black,
                Location = new Point(0, ROW_HEIGHT - 1),
                Size = new Size(_gridInner.Width, 2),
            };

            _gridHeader = new Panel
            {
                Parent = _gridInner,
                Name = "playerGridHeader",
                BackColor = Color.Gray,
                Location = new Point(0, 0),
                Size = new Size(_gridInner.Width, ROW_HEIGHT),
            };
            _gridHeaderName = new Label
            {
                Parent = _gridHeader,
                Name = "playerGridHeaderName",
                Location = new Point(0, 0),
                Size = new Size(_gridHeader.Width / 2, ROW_HEIGHT),
                Text = "Name",
                TextAlign = ContentAlignment.MiddleCenter,
            };
            _gridHeaderLocation = new Label
            {
                Parent = _gridHeader,
                Name = "playerGridHeaderLocation",
                Location = new Point(_gridHeader.Width / 2, 0),
                Size = new Size(_gridHeader.Width / 2, ROW_HEIGHT),
                Text = "Location",
                TextAlign = ContentAlignment.MiddleCenter,
            };

            _gridFooter = new Panel
            {
                Parent = _gridInner,
                Name = "playerGridFooter",
                BackColor = Color.White,
                Location = new Point(0, _gridInner.Height - ROW_HEIGHT),
                Size = new Size(_gridInner.Width, ROW_HEIGHT),
            };
            _gridFooterText = new Label
            {
                Parent = _gridFooter,
                Name = "playerGridFooterText",
                Location = new Point(0, 0),
                Size = new Size(_gridFooter.Width, ROW_HEIGHT),
                Text = "Connected players: 0/0",
                TextAlign = ContentAlignment.MiddleCenter,
            };

            ServerForm.AddPanel(_gridOuter);
        }
    }
}
