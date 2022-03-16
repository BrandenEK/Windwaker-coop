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
        public int debugLevel;//0
        public int syncDelay;//2500
        public int defaultPort;//25565
        public bool enableCheats;//true
        public bool runInWatcherMode;//false

        public Config(int dl, int sd, int dp, bool ec, bool wm)
        {
            debugLevel = dl;
            syncDelay = sd;
            defaultPort = dp;
            enableCheats = ec;
            runInWatcherMode = wm;
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
