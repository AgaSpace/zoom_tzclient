using System.Collections.Generic;
using Newtonsoft.Json;

namespace TerraZ.ServerData
{
    public struct NetItem
    {
        public static NetItem NewItem(short NetID, short Stack, byte Prefix) =>
            new NetItem()
            {
                _netId = NetID,
                _stack = Stack,
                _prefixId = Prefix
            };

        public NetItem(bool t = true) { _netId = 0; _stack = 0; _prefixId = 0; }

        public NetItem(int id, int stack, byte prefix) { _netId = id; _stack = stack; _prefixId = prefix; }

        [JsonProperty("netID")]
        public int _netId;
        [JsonProperty("stack")]
        public int _stack;
        [JsonProperty("prefix")]
        public byte _prefixId;
    }
}
