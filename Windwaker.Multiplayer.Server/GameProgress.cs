using System.Collections.Generic;

namespace Windwaker.Multiplayer.Server
{
    public class GameProgress
    {
        private readonly Dictionary<string, byte> items = new();

        public void ResetProgress()
        {
            ServerForm.Log("Resetting game progress!");
            items.Clear();
        }

        public bool AddItem(string item, byte value)
        {
            ServerForm.Log("Attempting to add item: " + item);
            if (items.TryGetValue(item, out byte current))
            {
                // If the item already exists, make sure it is better before adding it
                if (value > current)
                {
                    items[item] = value;
                    return true;
                }
                return false;
            }
            else
            {
                // If this item doesn't even exist yet, add it
                items.Add(item, value);
                return true;
            }
        }
    }
}
