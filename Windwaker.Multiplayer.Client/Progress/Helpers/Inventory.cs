
namespace Windwaker.Multiplayer.Client.Progress.Helpers
{
    internal class Telescope : SingleItem
    {
        protected override string Id => "telescope";
        protected override string Name => "the Telescope";

        protected override uint MainAddress => 0x4C44;
        protected override byte MainValue => 0x20;
        protected override uint BitfieldAddress => 0x4C59;
    }
    internal class Sail : SingleItem
    {
        protected override string Id => "sail";
        protected override string Name => "the Sail";

        protected override uint MainAddress => 0x4C45;
        protected override byte MainValue => 0x78;
        protected override uint BitfieldAddress => 0x4C5A;
    }
    internal class Windwaker : SingleItem
    {
        protected override string Id => "windwaker";
        protected override string Name => "the Windwaker";

        protected override uint MainAddress => 0x4C46;
        protected override byte MainValue => 0x22;
        protected override uint BitfieldAddress => 0x4C5B;
    }
    internal class GrapplingHook : SingleItem
    {
        protected override string Id => "grapplinghook";
        protected override string Name => "the Grappling Hook";

        protected override uint MainAddress => 0x4C47;
        protected override byte MainValue => 0x25;
        protected override uint BitfieldAddress => 0x4C5C;
    }
    internal class SpoilsBag : SingleItem
    {
        protected override string Id => "spoilsbag";
        protected override string Name => "the Spoils Bag";

        protected override uint MainAddress => 0x4C48;
        protected override byte MainValue => 0x24;
        protected override uint BitfieldAddress => 0x4C5D;
    }
    internal class Boomerang : SingleItem
    {
        protected override string Id => "boomerang";
        protected override string Name => "the Boomerang";

        protected override uint MainAddress => 0x4C49;
        protected override byte MainValue => 0x2D;
        protected override uint BitfieldAddress => 0x4C5E;
    }
    internal class DekuLeaf : SingleItem
    {
        protected override string Id => "dekuleaf";
        protected override string Name => "the Deku Leaf";

        protected override uint MainAddress => 0x4C4A;
        protected override byte MainValue => 0x34;
        protected override uint BitfieldAddress => 0x4C5F;
    }

    internal class TingleTuner : SingleItem
    {
        protected override string Id => "tingletuner";
        protected override string Name => "the Tingle Tuner";

        protected override uint MainAddress => 0x4C4B;
        protected override byte MainValue => 0x21;
        protected override uint BitfieldAddress => 0x4C60;
    }
    internal class PictoBox : MultipleItem
    {
        protected override string Id => "pictobox";
        protected override string[] Names => new string[]
        {
            "the Picto Box",
            "the Deluxe Picto Box",
        };

        protected override uint MainAddress => 0x4C4C;
        protected override byte[] MainValues => new byte[] { 0x23, 0x26 };
        protected override uint BitfieldAddress => 0x4C61;
    }
    internal class IronBoots : SingleItem
    {
        protected override string Id => "ironboots";
        protected override string Name => "the Iron Boots";

        protected override uint MainAddress => 0x4C4D;
        protected override byte MainValue => 0x29;
        protected override uint BitfieldAddress => 0x4C62;
    }
    internal class MagicArmor : SingleItem
    {
        protected override string Id => "magicarmor";
        protected override string Name => "the Magic Armor";

        protected override uint MainAddress => 0x4C4E;
        protected override byte MainValue => 0x2A;
        protected override uint BitfieldAddress => 0x4C63;
    }
    internal class BaitBag : SingleItem
    {
        protected override string Id => "baitbag";
        protected override string Name => "the Bait Bag";

        protected override uint MainAddress => 0x4C4F;
        protected override byte MainValue => 0x2C;
        protected override uint BitfieldAddress => 0x4C64;
    }
    internal class Bow : MultipleItem
    {
        protected override string Id => "bow";
        protected override string[] Names => new string[]
        {
            "the Hero's Bow",
            "Fire and Ice Arrows",
            "Light Arrows",
        };

        protected override uint MainAddress => 0x4C50;
        protected override byte[] MainValues => new byte[] { 0x27, 0x35, 0x36 };
        protected override uint BitfieldAddress => 0x4C65;
    }
    internal class Bombs : SingleItem
    {
        protected override string Id => "bombs";
        protected override string Name => "Bombs";

        protected override uint MainAddress => 0x4C51;
        protected override byte MainValue => 0x31;
        protected override uint BitfieldAddress => 0x4C66;
    }

    internal class Bottle1 : BottleItem
    {
        protected override string Id => "bottle1";

        protected override uint MainAddress => 0x4C52;
        protected override uint BitfieldAddress => 0x4C67;
    }
    internal class Bottle2 : BottleItem
    {
        protected override string Id => "bottle2";

        protected override uint MainAddress => 0x4C53;
        protected override uint BitfieldAddress => 0x4C68;
    }
    internal class Bottle3 : BottleItem
    {
        protected override string Id => "bottle3";

        protected override uint MainAddress => 0x4C54;
        protected override uint BitfieldAddress => 0x4C69;
    }
    internal class Bottle4 : BottleItem
    {
        protected override string Id => "bottle4";

        protected override uint MainAddress => 0x4C55;
        protected override uint BitfieldAddress => 0x4C6A;
    }
    internal class DeliveryBag : SingleItem
    {
        protected override string Id => "deliverybag";
        protected override string Name => "the Delivery Bag";

        protected override uint MainAddress => 0x4C56;
        protected override byte MainValue => 0x30;
        protected override uint BitfieldAddress => 0x4C6B;
    }
    internal class Hookshot : SingleItem
    {
        protected override string Id => "hookshot";
        protected override string Name => "the Hookshot";

        protected override uint MainAddress => 0x4C57;
        protected override byte MainValue => 0x2F;
        protected override uint BitfieldAddress => 0x4C6C;
    }
    internal class SkullHammer : SingleItem
    {
        protected override string Id => "skullhammer";
        protected override string Name => "the Skull Hammer";

        protected override uint MainAddress => 0x4C58;
        protected override byte MainValue => 0x33;
        protected override uint BitfieldAddress => 0x4C6D;
    }
}
