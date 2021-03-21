using System.IO;
using Terraria.Net;
using TerraZ.Client;
using Terraria;

namespace TerraZ.ServerData
{
    public class SyncNetModule : NetModule
    {
        public override bool Deserialize(BinaryReader reader, int userId)
        {
            TZLauncher.LauncherCore.WriteInfoBG("/RECEIVED::SyncNetModule");
            switch (reader.ReadByte())
            {
                case 1:
                    PermissionsData data0 = reader.ReadString().GetData<PermissionsData>();
                    data0.JSONPrint();
                    Client.Client.ClientPermissions.SetPermissions(data0.Permissions);
                    break;
                case 5:
                    Main.NotifyOfEvent(Terraria.GameContent.GameNotificationType.All);
                    break;
            }
            return true;
        }
    }
}
