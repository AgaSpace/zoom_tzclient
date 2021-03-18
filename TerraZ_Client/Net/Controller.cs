﻿using System;
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
                        MyPlugin.players[player.Index] = Levels.ClientUser;

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
                case IndexTypes.TryGetPlayerInventory:
                    {
                        if (player.GetPlayerInfo().HavePermission(Permissions.GetBanks))
                        {
                            if (Terraria.Main.ServerSideCharacter)
                            {
                                byte playerId = data["Player"].ToInt8();
                                byte bType = data["Type"].ToInt8();

                                Player tplayer = Main.player[playerId];

                                string result;

                                switch (bType)
                                {
                                    case 0:
                                        {
                                            NetItem[] items = new NetItem[tplayer.bank.item.Length];

                                            for (int i = 0; i < tplayer.bank.item.Length; i++)
                                            {
                                                Item it = tplayer.bank.item[i];
                                                items[i] = new NetItem(it.netID, it.stack, it.prefix);
                                            }

                                            result = JsonConvert.SerializeObject(items);
                                        }
                                        break;
                                    case 1:
                                        {
                                            NetItem[] items = new NetItem[tplayer.bank2.item.Length];

                                            for (int i = 0; i < tplayer.bank2.item.Length; i++)
                                            {
                                                Item it = tplayer.bank2.item[i];
                                                items[i] = new NetItem(it.netID, it.stack, it.prefix);
                                            }

                                            result = JsonConvert.SerializeObject(items);
                                        }
                                        break;
                                    case 2:
                                        {
                                            NetItem[] items = new NetItem[tplayer.bank3.item.Length];

                                            for (int i = 0; i < tplayer.bank3.item.Length; i++)
                                            {
                                                Item it = tplayer.bank3.item[i];
                                                items[i] = new NetItem(it.netID, it.stack, it.prefix);
                                            }

                                            result = JsonConvert.SerializeObject(items);
                                        }
                                        break;
                                    case 3:
                                        {
                                            NetItem[] items = new NetItem[tplayer.bank4.item.Length];

                                            for (int i = 0; i < tplayer.bank4.item.Length; i++)
                                            {
                                                Item it = tplayer.bank4.item[i];
                                                items[i] = new NetItem(it.netID, it.stack, it.prefix);
                                            }

                                            result = JsonConvert.SerializeObject(items);
                                        }
                                        break;

                                    default: result = ""; break;
                                }

                                if (result == "" || string.IsNullOrEmpty(result))
                                    break;

                                SendToClient(player, IndexTypes.TryGetPlayerInventory, new Dictionary<string, object>
                                {
                                    { "Player", playerId },
                                    { "Items", result }
                                });

                            }
                        }
                    }
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
