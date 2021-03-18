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
                    PermissionsData data0 = reader.ReadString().GetData<PermissionsData>();
                    data0.JSONPrint();
                    Client.Client.ClientPermissions.SetPermissions(data0.Permissions);
                    break;
                case 4:
                    BanksData data1 = reader.ReadString().GetData<BanksData>();
                    data1.JSONPrint();
                    for (int i = 0; i < data1.PiggyBank.Length; i++)
                        Main.player[data1.PlayerIndex].bank.item[i] = data1.PiggyBank[i].ToTerrariaItem();

                    for (int i = 0; i < data1.Safe.Length; i++)
                        Main.player[data1.PlayerIndex].bank2.item[i] = data1.Safe[i].ToTerrariaItem();

                    for (int i = 0; i < data1.DefenderForge.Length; i++)
                        Main.player[data1.PlayerIndex].bank3.item[i] = data1.DefenderForge[i].ToTerrariaItem();

                    for (int i = 0; i < data1.VoidBag.Length; i++)
                        Main.player[data1.PlayerIndex].bank4.item[i] = data1.VoidBag[i].ToTerrariaItem();
                    break;
                case 8:
                    SlotData data2 = reader.ReadString().GetData<SlotData>();
                    data2.JSONPrint();
                    switch (data2.SlotType)
                    {
                        case 1: // Piggy bank
                            Main.player[data2.PlayerIndex].bank.item[data2.SlotReference] = data2.NetItem.ToTerrariaItem();
                            // ...
                            break;
                        case 2: // Safe
                            Main.player[data2.PlayerIndex].bank2.item[data2.SlotReference] = data2.NetItem.ToTerrariaItem();
                            // ...
                            break;
                        case 3: // Defender's Forge
                            Main.player[data2.PlayerIndex].bank3.item[data2.SlotReference] = data2.NetItem.ToTerrariaItem();
                            // ...
                            break;
                        case 4: // Void Bag
                            Main.player[data2.PlayerIndex].bank4.item[data2.SlotReference] = data2.NetItem.ToTerrariaItem();
                            // ...
                            break;
                    }
                    break;
            }
            return true;
        }
    }
}
