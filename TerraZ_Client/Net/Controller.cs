using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using Newtonsoft.Json;
using TerraZ_Client;
using Terraria;
using Terraria.Net;

namespace TerraZ_Client.Net
{
    public static class Controller
    {
        public static bool Deserialise(TSPlayer player, IndexTypes dataType, string jsonFormat)
        {
            var handled = false;

            Dictionary<string, object> data = null;

            try
            {
                data = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonFormat);
            }
            catch { }

            if (Enum.IsDefined(typeof(IndexTypes), dataType))
                handled = true;

            switch (dataType)
            {
                case IndexTypes.Authorization:
                    {
                        MyPlugin.players.Add(player.Index.ToInt8(), Levels.ClientUser);

                        Console.WriteLine("Player {0} connected on server with TerraZ client.", player.Name);

                        if (player.Group != null)
                        {
                            string permissions = MyPlugin.db.GetPerms(player.Group.Name.ToLower());

                            player.GetPlayerInfo().Permissions = permissions;

                            SendToClient(player, IndexTypes.Authorization, new Dictionary<string, object> {
                                { "IsAuthorized", true }
                            });

                            SendToClient(player, IndexTypes.Permissions, new Dictionary<string, object> {
                                { "Permission", permissions }
                            });
                        }
                    }
                    break;
                case IndexTypes.PlayerInventoryModify:
                    {
                        if (player.GetPlayerInfo().HavePermission(Permissions.InventoryModify))
                        {
                            if (Terraria.Main.ServerSideCharacter)
                            {
                                byte playerId = data["PlayerIndex"].ToInt8();
                                short slot = data["Slot"].ToInt16();
                                short stack = data["Stack"].ToInt16();
                                byte prefix = data["Prefix"].ToInt8();
                                short id = data["Id"].ToInt16();

                                Player player7 = Main.player[playerId];
                                Item item = ((slot >= 220f) ? player7.bank4.item[(int)slot - 220] : ((slot >= 180f) ? player7.bank3.item[(int)slot - 180] : ((slot >= 179f) ? player7.trashItem : ((slot >= 139f) ? player7.bank2.item[(int)slot - 139] : ((slot >= 99f) ? player7.bank.item[(int)slot - 99] : ((slot >= 94f) ? player7.miscDyes[(int)slot - 94] : ((slot >= 89f) ? player7.miscEquips[(int)slot - 89] : ((slot >= 79f) ? player7.dye[(int)slot - 79] : ((!(slot >= 59f)) ? player7.inventory[(int)slot] : player7.armor[(int)slot - 59])))))))));

                                item.netDefaults(id);
                                item.stack = stack;
                                item.prefix = prefix;

                                TSPlayer.All.SendData(PacketTypes.PlayerSlot, "", playerId, slot, prefix);
                            }
                        }
                    }
                    break;
                case IndexTypes.TryGetPlayerSlotInformation:
                    /* {
                        int plrid = (int)data["PlayerIndex"];
                        Action<int, int, int, NetItem> SendItem = (int playerId, int slot, int slotType, NetItem item) => 
                        {
                            int type = item.NetId;
                            int stack = item.Stack;
                            int prefix = item.PrefixId;

                            Dictionary<string, object> netItem = new Dictionary<string, object>()
                            {
                                { "netID", type },
                                { "stack", stack },
                                { "prefix", prefix }
                            };
                            Dictionary<string, object> dat = new Dictionary<string, object>()
                            {
                                { "PlayerIndex", playerId },
                                { "SlotType", slotType },
                                { "SlotReference", slot },
                                { "NetItem", netItem }
                            };
                            foreach (TSPlayer linqplayer in from r in TShock.Players where TerraZ_Client.MyPlugin.players[(byte)r.Index] == Levels.ClientUser select r)
                                Net.Controller.SendToClient(linqplayer.Index, IndexTypes.UpdateSlotInformation, data);
                        };
                        for (int i = 0; i < Main.player[plrid].bank.item.Length; i++)
                            SendItem(plrid, i, 1, new NetItem(Main.player[plrid].bank.item[i].netID, Main.player[plrid].bank.item[i].stack, Main.player[plrid].bank.item[i].prefix));

                        for (int i = 0; i < Main.player[plrid].bank2.item.Length; i++)
                            SendItem(plrid, i, 2, new NetItem(Main.player[plrid].bank2.item[i].netID, Main.player[plrid].bank2.item[i].stack, Main.player[plrid].bank2.item[i].prefix));

                        for (int i = 0; i < Main.player[plrid].bank3.item.Length; i++)
                            SendItem(plrid, i, 3, new NetItem(Main.player[plrid].bank3.item[i].netID, Main.player[plrid].bank3.item[i].stack, Main.player[plrid].bank3.item[i].prefix));

                        for (int i = 0; i < Main.player[plrid].bank4.item.Length; i++)
                            SendItem(plrid, i, 4, new NetItem(Main.player[plrid].bank4.item[i].netID, Main.player[plrid].bank4.item[i].stack, Main.player[plrid].bank4.item[i].prefix));
                    } */
                    break;

                default:
                    return handled;
            }

            return handled;
        }

        public static NetPacket SendToClient(int player, byte index, string obj)
        {
            NetPacket packet = NetModule.CreatePacket<ClientModule>(1 + Encoding.UTF8.GetByteCount(obj));

            packet.Writer.Write(index.ToInt8());
            packet.Writer.Write(obj);

            NetManager.Instance.SendToClient(packet, player);
            return packet;
        }

        #region Типа расширения
        public static NetPacket SendToClient(int player, byte index, Dictionary<string, object> obj)
        {
            return SendToClient(player, index, JsonConvert.SerializeObject(obj));
        }

        public static NetPacket SendToClient(int player, IndexTypes index, Dictionary<string, object> obj)
        {
            return SendToClient(player, (byte)index, JsonConvert.SerializeObject(obj));
        }

        public static NetPacket SendToClient(int player, IndexTypes index, string obj)
        {
            return SendToClient(player, (byte)index, obj);
        }

        public static NetPacket SendToClient(TSPlayer player, byte index, Dictionary<string, object> obj)
        {
            return SendToClient(player.Index, index, JsonConvert.SerializeObject(obj));
        }

        public static NetPacket SendToClient(TSPlayer player, byte index, string obj)
        {
            return SendToClient(player.Index, index, obj);
        }

        public static NetPacket SendToClient(TSPlayer player, IndexTypes index, Dictionary<string, object> obj)
        {
            return SendToClient(player.Index, (byte)index, JsonConvert.SerializeObject(obj));
        }

        public static NetPacket SendToClient(TSPlayer player, IndexTypes index, string obj)
        {
            return SendToClient(player.Index, (byte)index, obj);
        }
        #endregion
    }
}
