
namespace Windwaker.Multiplayer.Client.Progress
{
    /// <summary>
    /// Generic interface for obtainable progress
    /// </summary>
    //internal interface IObtainable
    //{
    //    public void CheckForProgress(byte value);

    //    public void AddProgress(byte value);

    //    public string GetNotificationPart(byte value);
    //}

    /// <summary>
    /// A base item that has an id and can be obtained
    /// </summary>
    internal abstract class BaseItem
    {
        public string Id => GetType().Name.ToLower();

        public abstract void CheckForProgress(byte value);

        public abstract void AddProgress(byte value);

        public abstract string GetNotificationPart(byte value);
    }

    /// <summary>
    /// An item that can only be either owned or not owned, and also stores a bitfield
    /// </summary>
    internal abstract class SingleItem : BaseItem
    {
        protected abstract string Name { get; }

        protected abstract uint MainAddress { get; }
        protected abstract byte MainValue { get; }
        protected abstract uint BitfieldAddress { get; }

        public override void CheckForProgress(byte value)
        {
            if (value == MainValue && Core.ProgressManager.GetItemLevel(Id) < 1)
            {
                Core.ProgressManager.ObtainItem(Id, 1);
            }
        }

        public override void AddProgress(byte value)
        {
            byte main = (byte)(value == 1 ? MainValue : 0xFF);
            byte bitfield = (byte)(value == 1 ? 0xFF : 0x00);

            Core.DolphinManager.TryWrite(MainAddress, new byte[] { main });
            Core.DolphinManager.TryWrite(BitfieldAddress, new byte[] { bitfield });
        }

        public override string GetNotificationPart(byte value)
        {
            return value > 0 ? $" obtained {Name}" : null;
        }
    }

    /// <summary>
    /// An item that can have multiple levels, and also stores a bitfield
    /// </summary>
    internal abstract class MultipleItem : BaseItem
    {
        protected abstract string[] Names { get; }

        protected abstract uint MainAddress { get; }
        protected abstract byte[] MainValues { get; }
        protected abstract uint BitfieldAddress { get; }

        public override void CheckForProgress(byte value)
        {
            for (int i = MainValues.Length - 1; i >= 0; i--)
            {
                byte level = (byte)(i + 1);
                if (value == MainValues[i] && Core.ProgressManager.GetItemLevel(Id) < level)
                {
                    Core.ProgressManager.ObtainItem(Id, level);
                    break;
                }
            }
        }

        public override void AddProgress(byte value)
        {
            byte main = (byte)(value > 0 ? MainValues[value - 1] : 0xFF);
            byte bitfield = 0x00;
            for (int i = 0; i < value; i++)
            {
                bitfield |= (byte)(1 << i);
            }

            Core.DolphinManager.TryWrite(MainAddress, new byte[] { main });
            Core.DolphinManager.TryWrite(BitfieldAddress, new byte[] { bitfield });
        }

        public override string GetNotificationPart(byte value)
        {
            return value > 0 ? $" obtained {Names[value - 1]}" : null;
        }
    }

    // Bitfield item

    /// <summary>
    /// An item that simply increases in value
    /// </summary>
    internal abstract class ValueItem : BaseItem
    {
        protected abstract string Name { get; }

        protected abstract uint MainAddress { get; }

        public override void CheckForProgress(byte value)
        {
            if (Core.ProgressManager.GetItemLevel(Id) < value)
            {
                Core.ProgressManager.ObtainItem(Id, value);
            }
        }

        public override void AddProgress(byte value)
        {
            Core.DolphinManager.TryWrite(MainAddress, new byte[] { value });
        }

        public override string GetNotificationPart(byte value)
        {
            return value > 0 ? $" obtained {Name}" : null;
        }
    }

    /// <summary>
    /// An item (bottle) that can be obtained as any value, but is only sent as empty
    /// </summary>
    internal abstract class BottleItem : BaseItem
    {
        protected abstract uint MainAddress { get; }
        protected abstract uint BitfieldAddress { get; }

        public override void CheckForProgress(byte value)
        {
            if (value >= 0x50 && value <= 0x59 && Core.ProgressManager.GetItemLevel(Id) < 1)
            {
                Core.ProgressManager.ObtainItem(Id, 1);
            }
        }

        public override void AddProgress(byte value)
        {
            byte main = (byte)(value == 1 ? 0x50 : 0xFF);
            byte bitfield = (byte)(value == 1 ? 0xFF : 0x00);

            Core.DolphinManager.TryWrite(MainAddress, new byte[] { main });
            Core.DolphinManager.TryWrite(BitfieldAddress, new byte[] { bitfield });
        }

        public override string GetNotificationPart(byte value)
        {
            return value > 0 ? $" obtained a new bottle" : null;
        }
    }
}
