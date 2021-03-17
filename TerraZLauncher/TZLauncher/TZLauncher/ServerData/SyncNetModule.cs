using System.IO;
using Terraria.Net;
using TerraZ.Client;    

namespace TerraZ.ServerData
{
    public class SyncNetModule : NetModule
    {
        public override bool Deserialize(BinaryReader reader, int userId)
        {
            switch ((PacketID)reader.ReadByte())
            {
                case PacketID.PermissionsData:
                    PermissionsData dat = reader.ReadString().GetData<PermissionsData>();
                    Client.Client.ClientPermissions.SetPermissions(dat.Permissions);
                    break;
            }
            return true;
        }
    }
}
