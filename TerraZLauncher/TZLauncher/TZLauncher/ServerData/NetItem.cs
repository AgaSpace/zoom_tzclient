using System.Collections.Generic;
using Newtonsoft.Json;

namespace TerraZ.ServerData
{
    public class NetItem
    {
        public static NetItem NewItem(short NetID, short Stack, byte Prefix) =>
            new NetItem()
            {
                NetID = NetID,
                Stack = Stack,
                Prefix = Prefix
            };

        private NetItem() { }

        public short NetID { get; private set; }
        public short Stack { get; private set; }
        public byte Prefix { get; private set; }
    }
}
