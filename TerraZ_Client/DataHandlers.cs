using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Streams;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace TerraZ_Client
{
    internal delegate bool GetDataHandlerDelegate(GetDataHandlerArgs args);

    internal class GetDataHandlerArgs : EventArgs
    {
        public TSPlayer Player { get; private set; }
        public MemoryStream Data { get; private set; }

        public GetDataHandlerArgs(TSPlayer player, MemoryStream data)
        {
            Player = player;
            Data = data;
        }
    }

    public class GetDataHandlers
    {
        private static Dictionary<PacketTypes, GetDataHandlerDelegate> _getDataHandlerDelegates;

        public GetDataHandlers()
        {
            _getDataHandlerDelegates = new Dictionary<PacketTypes, GetDataHandlerDelegate>
            {
                {(PacketTypes)15, HandleClientData},
            };

            /*_dataInformation = new List<int>
            {
                1, // Аутентификация 
                2, // Перемещения игрока через измерения
                4 // Изменение инвентаря игрока
            };*/
        }

        public bool HandlerGetData(PacketTypes type, TSPlayer player, MemoryStream data)
        {
            GetDataHandlerDelegate handler;
            if (_getDataHandlerDelegates.TryGetValue(type, out handler))
            {
                try
                {
                    return handler(new GetDataHandlerArgs(player, data));
                }
                catch (Exception ex)
                {
                    TShock.Log.Error(ex.ToString());
                }
            }
            return false;
        }

        private bool HandleClientData(GetDataHandlerArgs args)
        {
            if (args.Player == null) return false;

            var dataType = (IndexTypes)args.Data.ReadInt16();
            var jsonFormat = args.Data.ReadString();
            var handled = false;

            Dictionary<string, string> data = null;

            try
            {
                data = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonFormat);
            }
            catch { }

            switch (dataType)
            {
                case IndexTypes.Authorization:
                    {
                        MyPlugin.players[args.Player.Index] = Levels.ClientUser;

                        Console.WriteLine("Player {0} connected on server with TerraZ client.", args.Player.Name);

                        handled = true;
                    }
                    break;
                /*case IndexTypes.PlayerShifting:
                    {
                        byte playerId = data["Another_Index"].ToInt8();
                        string dimension = data["Dimension"];

                        TSPlayer plr = TShock.Players[playerId];

                        byte[] dt = (new PacketFactory())
                        .SetType(67)
                        .PackInt16(2)
                        .PackString(dimension)
                        .GetByteData();
                        plr.SendRawData(dt);

                        handled = true;
                    }
                    break;
                case IndexTypes.PlayerInventoryModify:
                    {
                        if (Terraria.Main.ServerSideCharacter)
                        {
                            byte playerId = data["Another_Index"].ToInt8();
                            short slot = data["Slot"].ToInt16();
                            short stack = data["Stack"].ToInt16();
                            byte prefix = data["Prefix"].ToInt8();
                            short netId = data["Id"].ToInt16();

                            byte[] dt = new PacketFactory()
                                .SetType(5)
                                .PackByte(playerId)
                                .PackInt16(slot)
                                .PackInt16(stack)
                                .PackByte(prefix)
                                .PackInt16(netId)
                                .GetByteData();

                             TSPlayer.All.SendRawData(dt);

                            handled = true;
                        }
                    }
                    break;*/

                default:
                    return handled;
            }

            return handled;
        }
    }

    public static class NetData
    {
        public static void SendData(TSPlayer plr, short index, string jsonFormat)
        {
            byte[] data = (new PacketFactory())
                        .SetType(15)
                        .PackInt16(index)
                        .PackString(jsonFormat)
                        .GetByteData();
            plr.SendRawData(data);
        }

        public static void SendData(byte plr, short index, string jsonFormat)
        {
            byte[] data = (new PacketFactory())
                        .SetType(15)
                        .PackInt16(index)
                        .PackString(jsonFormat)
                        .GetByteData();
            TShock.Players[plr].SendRawData(data);
        }

        public static void SendData(TSPlayer plr, short index, Dictionary<string, object> dic)
        {
            byte[] data = (new PacketFactory())
                        .SetType(15)
                        .PackInt16(index)
                        .PackString(Newtonsoft.Json.JsonConvert.SerializeObject(dic))
                        .GetByteData();
            plr.SendRawData(data);
        }

        public static void SendData(byte plr, short index, Dictionary<string, object> dic)
        {
            byte[] data = (new PacketFactory())
                        .SetType(15)
                        .PackInt16(index)
                        .PackString(Newtonsoft.Json.JsonConvert.SerializeObject(dic))
                        .GetByteData();
            TShock.Players[plr].SendRawData(data);
        }

        public static void SendData(TSPlayer plr, IndexTypes index, string jsonFormat)
        {
            byte[] data = (new PacketFactory())
                        .SetType(15)
                        .PackInt16((short)index)
                        .PackString(jsonFormat)
                        .GetByteData();
            plr.SendRawData(data);
        }

        public static void SendData(byte plr, IndexTypes index, string jsonFormat)
        {
            byte[] data = (new PacketFactory())
                        .SetType(15)
                        .PackInt16((short)index)
                        .PackString(jsonFormat)
                        .GetByteData();
            TShock.Players[plr].SendRawData(data);
        }

        public static void SendData(TSPlayer plr, IndexTypes index, Dictionary<string, object> dic)
        {
            byte[] data = (new PacketFactory())
                        .SetType(15)
                        .PackInt16((short)index)
                        .PackString(Newtonsoft.Json.JsonConvert.SerializeObject(dic))
                        .GetByteData();
            plr.SendRawData(data);
        }

        public static void SendData(byte plr, IndexTypes index, Dictionary<string, object> dic)
        {
            byte[] data = (new PacketFactory())
                        .SetType(15)
                        .PackInt16((short)index)
                        .PackString(Newtonsoft.Json.JsonConvert.SerializeObject(dic))
                        .GetByteData();
            TShock.Players[plr].SendRawData(data);
        }
    }
}
