﻿using System;
using Newtonsoft.Json;

namespace Windwaker_coop
{
    struct MemoryLocation
    {
        public IntPtr startAddress;
        public int size;
        public string name;
        public string type;

        public int compareId;
        //0 - greater than, 1 - less than, 2 - greater than (excluding 255)
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
    /*
    [Serializable]
    struct SyncSettings
    {
        public bool inventoryItems;
        public bool equipmentItems;
        public bool storyItems;

        public bool stats;
        public bool capacities;
        public bool charts;
        public bool seaMap;

        public bool stageInfos;
        public bool events;

        //Experimental
        public bool maxHealth;
        public bool bottles;
        public bool smallKeys;

        public static SyncSettings getDefaultSettings()
        {
            SyncSettings ss = new SyncSettings();
            ss.inventoryItems = true;
            ss.equipmentItems = true;
            ss.storyItems = true;
            ss.stats = true;
            ss.capacities = true;
            ss.charts = true;
            ss.seaMap = true;
            ss.stageInfos = true;
            ss.events = true;

            ss.maxHealth = true;
            ss.bottles = true;
            ss.smallKeys = true;
            return ss;
        }
    }*/
}
