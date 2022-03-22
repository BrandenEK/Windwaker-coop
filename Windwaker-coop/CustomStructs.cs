using System;

namespace Windwaker_coop
{
    struct MemoryLocation
    {
        public IntPtr startAddress;
        public int size;
        public string name;
        public string type;

        public int compareId;
        //0 - greater than, 1 - less than, 2 - greater than (excluding 255), 3 - bottles, 8 - different, 9 - bitfields
        public uint lowerValue;
        public uint higherValue;
        public byte defaultValue;
        public uint individualBits;
        public ComparisonData cd;

        public MemoryLocation(uint startAddress, int size, string name, string type, int compareId, uint low, uint high, byte defaultValue, uint indvBits, ComparisonData cd)
        {
            this.startAddress = (IntPtr)startAddress;
            this.size = size;
            this.name = name;
            this.type = type;
            this.compareId = compareId;
            this.defaultValue = defaultValue;
            lowerValue = low;
            higherValue = high;
            individualBits = indvBits;
            this.cd = cd;
        }
    }

    struct ComparisonData
    {
        public uint[] values;
        public string[] text;
        public bool bitfield;

        public ComparisonData(uint[] values, string[] text, bool bitfield)
        {
            this.values = values;
            this.text = text;
            this.bitfield = bitfield;
        }
    }

    struct TimeFlag
    {
        public IntPtr address;
        public byte bit;
        public float newTime;
        //If newtime == -1, then this will actually endTimeZone

        public TimeFlag(IntPtr address, byte bit, float newTime)
        {
            this.address = address;
            this.bit = bit;
            this.newTime = newTime;
        }
    }

    struct Cheat
    {
        public string itemName;
        public uint address;
        public byte noNumberByte;

        public bool requiresNumber;
        public int maxValue;
        public byte[] valuesToWrite;
        //If this cheat requires no number, then the maxValue & valuesToWrite are null

        public Cheat(string itemName, uint address, byte noNumberByte, bool requiresNumber, int maxValue = -1, byte[] valuesToWrite = null)
        {
            this.itemName = itemName;
            this.address = address;
            this.noNumberByte = noNumberByte;
            this.requiresNumber = requiresNumber;
            this.maxValue = maxValue;
            this.valuesToWrite = valuesToWrite;
        }
    }

    [Serializable]
    struct Config
    {
        //Shows more technical parts in the console.  Possible values are 0-2 [0]
        public int debugLevel;
        //The time in milliseconds to wait between each sync loop [2500]
        public int syncDelay;
        //If a port is not specified, this is the port that will be used [25565]
        public int defaultPort;
        //Whether or not cheats should be allowed using the 'give' command [true]
        public bool enableCheats;
        //Watcher Mode is for memory testing [false]
        public bool runInWatcherMode;

        public Config(int dl, int sd, int dp, bool ec, bool wm)
        {
            debugLevel = dl;
            syncDelay = sd;
            defaultPort = dp;
            enableCheats = ec;
            runInWatcherMode = wm;
        }

        public static Config getDefaultConfig()
        {
            return new Config(0, 2500, 25565, true, false);
        }
    }

    [Serializable]
    struct SyncSettings
    {
        public string[] keys;
        public bool[] values;

        public SyncSettings(string[] keys, bool[] values)
        {
            this.keys = keys;
            this.values = values;
        }

        public bool getSetting(string setting)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                if (keys[i] == setting)
                    return values[i];
            }
            return false;
        }
    }
}
