
namespace Windwaker.Multiplayer.Client.Progress.Helpers
{
    internal class Health : ValueItem
    {
        protected override string Id => "maxhealth";
        protected override string Name => "a piece of heart";

        protected override uint MainAddress => 0x4C09;
    }
    internal class Magic : ValueItem
    {
        protected override string Id => "maxmagic";
        protected override string Name => "a higher magic capacity";

        protected override uint MainAddress => 0x4C1B;
    }
    internal class Arrows : ValueItem
    {
        protected override string Id => "maxarrows";
        protected override string Name => "a higher arrow capacity";

        protected override uint MainAddress => 0x4C77;
    }
    internal class Bombs : ValueItem
    {
        protected override string Id => "maxbombs";
        protected override string Name => "a higher bomb capacity";

        protected override uint MainAddress => 0x4C78;
    }
}
