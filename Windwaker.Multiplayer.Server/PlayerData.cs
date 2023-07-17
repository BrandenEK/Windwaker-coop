
namespace Windwaker.Multiplayer.Server
{
    internal class PlayerData
    {
        public string Name => _name;

        public string CurrentScene => _scene;

        public void UpdateScene(string scene)
        {
            _scene = scene;
        }

        public PlayerData(string name)
        {
            _name = name;
            _scene = "...";
        }

        private readonly string _name;

        private string _scene;
    }
}
