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

        public short NetID;
        public short Stack;
        public byte Prefix;
    }
}
