
namespace Windwaker.Multiplayer.Server
{
    internal class PlayerData
    {
        public string Name => _name;

        public string CurrentSceneName
        {
            get
            {
                return _scene switch
                {
                    0x00 => "Great Sea",
                    0x02 => "Forsaken Fortress",
                    0x03 => "Dragon Roost Cavern",
                    0x04 => "Forbidden Woods",
                    0x05 => "Tower of the Gods",
                    0x06 => "Earth Temple",
                    0x07 => "Wind Temple",
                    0x08 => "Ganon's Tower",
                    0x09 => "Hyrule",
                    0x0A => "Ship interior",
                    0x0B => "House interior",
                    0x0C => "Cave interior",
                    0x0D => "Misc interior",
                    _ => "...",
                };
            }
        }

        public void UpdateScene(byte scene)
        {
            _scene = scene;
        }

        public PlayerData(string name)
        {
            _name = name;
            _scene = 0xFF;
        }

        private readonly string _name;

        private byte _scene;
    }
}
