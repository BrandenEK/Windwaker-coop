
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
}
