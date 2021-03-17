using TerraZ.Client;
using Terraria;

namespace TerraZ.ServerData
{
    public class PermissionsRequest
    {
        public static PermissionsRequest NewState() => new PermissionsRequest();

        private PermissionsRequest()
        {
        }

        public void Send()
        {
            ClientUtils.SendData(new PacketWriter()
                .SetType(82)
                .PackInt16(byte.Parse(3.ToString())) // First RequestID
                .GetByteData());
        }
    }
}
