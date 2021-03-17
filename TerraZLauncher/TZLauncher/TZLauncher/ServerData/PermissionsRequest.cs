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

        public void Send() => DataBuilder.SendData(1, this.ToJson());
    }
}
