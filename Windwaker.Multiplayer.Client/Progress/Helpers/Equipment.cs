
namespace Windwaker.Multiplayer.Client.Progress.Helpers
{
    internal class Sword : MultipleItem
    {
        protected override string Id => "sword";
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
        protected override string Id => "shield";
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
        protected override string Id => "powerbracelets";
        protected override string Name => "the Power Bracelets";

        protected override uint MainAddress => 0x4C18;
        protected override byte MainValue => 0x28;
        protected override uint BitfieldAddress => 0x4CBE;
    }
    // Pirates charm
    // Heros charm

    internal class MaxHealth : ValueItem
    {
        protected override string Id => "maxhealth";
        protected override string Name => "a piece of heart";

        protected override uint MainAddress => 0x4C09;
    }
    internal class MaxMagic : ValueItem
    {
        protected override string Id => "maxmagic";
        protected override string Name => "a higher magic capacity";

        protected override uint MainAddress => 0x4C1B;
    }
    internal class MaxArrows : ValueItem
    {
        protected override string Id => "maxarrows";
        protected override string Name => "a higher arrow capacity";

        protected override uint MainAddress => 0x4C77;
    }
    internal class MaxBombs : ValueItem
    {
        protected override string Id => "maxbombs";
        protected override string Name => "a higher bomb capacity";

        protected override uint MainAddress => 0x4C78;
    }
    internal class Wallet : ValueItem
    {
        protected override string Id => "wallet";
        protected override string Name => "a bigger rupee wallet";

        protected override uint MainAddress => 0x4C1A;
    }
}
