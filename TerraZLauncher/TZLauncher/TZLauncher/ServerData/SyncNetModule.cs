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
            switch (reader.ReadByte())
            {
                case 1:
                    PermissionsData dat = reader.ReadString().GetData<PermissionsData>();
                    Client.Client.ClientPermissions.SetPermissions(dat.Permissions);
                    break;
                case 2:
                    BanksData data = reader.ReadString().GetData<BanksData>();
                    for (int i = 0; i < data.PiggyBank.Length; i++)
                        Main.player[data.PlayerIndex].bank.item[i] = data.PiggyBank[i].ToTerrariaItem();

                    for (int i = 0; i < data.Safe.Length; i++)
                        Main.player[data.PlayerIndex].bank2.item[i] = data.Safe[i].ToTerrariaItem();

                    for (int i = 0; i < data.DefenderForge.Length; i++)
                        Main.player[data.PlayerIndex].bank3.item[i] = data.DefenderForge[i].ToTerrariaItem();

                    for (int i = 0; i < data.VoidBag.Length; i++)
                        Main.player[data.PlayerIndex].bank4.item[i] = data.VoidBag[i].ToTerrariaItem();
                    break;
            }
            return true;
        }
    }
}
