
namespace Windwaker.Multiplayer.Client.Progress.Helpers
{
    internal class Sword : MultipleItem
    {
        protected override string[] Names => new string[]
        {
            "the Hero's Sword",
            "the Master Sword (Powerless)",
            "the Master Sword (Half power)",
            "the Master Sword (Full power)",
        };

        protected override uint MainAddress => 0x4C16;
        protected override byte[] MainValues => new byte[] { 0x38, 0x39, 0x3A, 0x3E };
        protected override uint BitfieldAddress => 0x4CBC;
    }
    internal class Shield : MultipleItem
    {
        protected override string[] Names => new string[]
        {
            "the Hero's Shield",
            "the Mirror Shield",
        };

        protected override uint MainAddress => 0x4C17;
        protected override byte[] MainValues => new byte[] { 0x3B, 0x3C };
        protected override uint BitfieldAddress => 0x4CBD;
    }

    internal class PowerBracelets : SingleItem
    {
        protected override string Name => "the Power Bracelets";

        protected override uint MainAddress => 0x4C18;
        protected override byte MainValue => 0x28;
        protected override uint BitfieldAddress => 0x4CBE;
    }
    internal class PiratesCharm : BitfieldItem
    {
        protected override string[] Names => new string[]
        {
            "obtained the Pirates Charm",
        };

        protected override uint BitfieldAddress => 0x4CBF;
        protected override byte[] BitfieldValues => new byte[] { 0x01 };
    }
    internal class HerosCharm : BitfieldItem
    {
        protected override string[] Names => new string[]
        {
            "obtained the Hero's Charm",
        };

        protected override uint BitfieldAddress => 0x4CC0;
        protected override byte[] BitfieldValues => new byte[] { 0x01 };
    }

    internal class MaxHealth : ValueItem
    {
        protected override string Name => "a piece of heart";

        protected override uint MainAddress => 0x4C09;
    }
    internal class MaxMagic : ValueItem
    {
        protected override string Name => "a higher magic capacity";

        protected override uint MainAddress => 0x4C1B;
    }
    internal class MaxArrows : ValueItem
    {
        protected override string Name => "a higher arrow capacity";

        protected override uint MainAddress => 0x4C77;
    }
    internal class MaxBombs : ValueItem
    {
        protected override string Name => "a higher bomb capacity";

        protected override uint MainAddress => 0x4C78;
    }
    internal class Wallet : ValueItem
    {
        protected override string Name => "a bigger rupee wallet";

        protected override uint MainAddress => 0x4C1A;
    }

    internal class Pearls : BitfieldItem
    {
        protected override string[] Names => new string[]
        {
            "obtained Din's Pearl",
            "obtained Farore's Pearl",
            "obtained Nayru's Pearl",
        };

        protected override uint BitfieldAddress => 0x4CC7;
        protected override byte[] BitfieldValues => new byte[] { 0x02, 0x04, 0x01 };
    }
    internal class Shards : BitfieldItem
    {
        protected override string[] Names => new string[]
        {
            "obtained Triforce Shard #1",
            "obtained Triforce Shard #2",
            "obtained Triforce Shard #3",
            "obtained Triforce Shard #4",
            "obtained Triforce Shard #5",
            "obtained Triforce Shard #6",
            "obtained Triforce Shard #7",
            "obtained Triforce Shard #8",
        };

        protected override uint BitfieldAddress => 0x4CC6;
        protected override byte[] BitfieldValues => new byte[] { 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80 };
    }
    internal class Songs : BitfieldItem
    {
        protected override string[] Names => new string[]
        {
            "learned the Wind's Requiem",
            "learned the Ballad of Gales",
            "learned the Command Melody",
            "learned the Earth God's Lyric",
            "learned the Wind God's Aria",
            "learned the Song of Passing",
        };

        protected override uint BitfieldAddress => 0x4CC5;
        protected override byte[] BitfieldValues => new byte[] { 0x01, 0x02, 0x04, 0x08, 0x10, 0x20 };
    }
    internal class TingleStatues : BitfieldItem
    {
        protected override string[] Names => new string[]
        {
            "obtained the Dragon Tingle Statue",
            "obtained the Forbidden Tingle Statue",
            "obtained the Goddess Tingle Statue",
            "obtained the Earth Tingle Statue",
            "obtained the Wind Tingle Statue",
        };

        protected override uint BitfieldAddress => 0x5296;
        protected override byte[] BitfieldValues => new byte[] { 0x04, 0x08, 0x10, 0x20, 0x40 };
    }
}
