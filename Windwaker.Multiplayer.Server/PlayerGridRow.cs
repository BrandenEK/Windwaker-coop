using System.Drawing;
using System.Windows.Forms;

namespace Windwaker.Multiplayer.Server
{
    public class PlayerGridRow
    {
        private readonly Panel _rowHolder;

        private readonly Panel _rowBackground;
        private readonly Panel _rowDivider;

        private readonly Label _rowNameText;
        private readonly Label _rowLocationText;

        public PlayerGridRow(Panel holder)
        {
            _rowHolder = holder;

            _rowBackground = new Panel
            {
                Parent = _rowHolder,
                Name = "gridRowBackground",
                BackColor = Color.White,
                Location = new Point(0, 0),
                Size = new Size(_rowHolder.Width, PlayerGrid.ROW_HEIGHT),
            };
            _rowDivider = new Panel
            {
                Parent = _rowHolder,
                Name = "gridRowDivider",
                BackColor = Color.Black,
                Location = new Point(0, 0),
                Size = new Size(_rowHolder.Width, 2),
            };

            _rowNameText = new Label
            {
                Parent = _rowBackground,
                Name = "gridRowName",
                Location = new Point(0, 0),
                Size = new Size(_rowBackground.Width / 2, PlayerGrid.ROW_HEIGHT),
                TextAlign = ContentAlignment.MiddleCenter,
            };
            _rowLocationText = new Label
            {
                Parent = _rowBackground,
                Name = "gridRowLocation",
                Location = new Point(_rowBackground.Width / 2, 0),
                Size = new Size(_rowBackground.Width / 2, PlayerGrid.ROW_HEIGHT),
                TextAlign = ContentAlignment.MiddleCenter,
            };
        }

        public void Update(string name, string location, int idx)
        {
            _rowBackground.Location = new Point(0, PlayerGrid.ROW_HEIGHT * (idx + 1));
            _rowBackground.BackColor = idx % 2 == 0 ? Color.LightGray : Color.DarkGray;
            _rowDivider.Location = new Point(0, PlayerGrid.ROW_HEIGHT * (idx + 2) - 1);
            _rowDivider.BringToFront();

            _rowNameText.Text = name;
            _rowLocationText.Text = location;
            SetVisibility(true);
        }

        public void SetVisibility(bool visible)
        {
            _rowBackground.Visible = visible;
            _rowDivider.Visible = visible;
            _rowNameText.Visible = visible;
            _rowLocationText.Visible = visible;
        }
    }
}
