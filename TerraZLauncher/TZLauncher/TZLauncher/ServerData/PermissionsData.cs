using TerraZ.Client;
using Terraria;

namespace TerraZ.ServerData
{
    public class PermissionsData
    {
        public PermissionsData(string permissions)
        {
            this.Permissions = permissions;
        }

        public string Permissions;
    }
}
