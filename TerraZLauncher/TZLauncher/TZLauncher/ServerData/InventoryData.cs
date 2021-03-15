using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraZ.Client;

namespace TZLauncher.ServerData
{
	public class InventoryData
    {
        public InventoryData(byte index, short slot)
        {
            this.PlayerIndex = index;
            this.Slot = slot;
            this.Stack = 0;
            this.Prefix = 0;
            this.Id = 0;
        }

        public InventoryData(byte index, short slot, short stack, byte prefix, short id)
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

        public void Send()
        {
            ClientUtils.SendData(new PacketWriter()
                .SetType(15)
                .PackInt16(2)
                .PackString(this.ToJson())
                .GetByteData());
        }
    }
}
