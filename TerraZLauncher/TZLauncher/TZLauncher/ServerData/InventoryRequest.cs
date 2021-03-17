using TerraZ.Client;

namespace TerraZ.ServerData
{
	public class InventoryRequest
    {
        public InventoryRequest(byte index, short slot)
        {
            this.PlayerIndex = index;
            this.Slot = slot;
            this.Stack = 0;
            this.Prefix = 0;
            this.Id = 0;
        }

        public InventoryRequest(byte index, short slot, short stack, byte prefix, short id)
        {
            this.PlayerIndex = index;
            this.Slot = slot;
            this.Stack = stack;
            this.Prefix = prefix;
            this.Id = id;
        }

        public byte PlayerIndex;
        public short Slot;
        public short Stack;
        public byte Prefix;
        public short Id;

        public void Send() => DataBuilder.SendData(2, this.ToJson());
    }
}
