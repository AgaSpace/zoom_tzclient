using System;
using TShockAPI;
using System.Linq;
using Terraria;
using TerrariaApi.Server.Hooking;
using TerrariaApi.Server;
using System.Collections.Generic;
using System.IO;
using MySql.Data.MySqlClient;
using Mono.Data.Sqlite;
using System.Data;
using System.IO.Streams;
using System.Runtime.CompilerServices;
using System.Reflection;
using OTAPI;
using Terraria.Net;
using static TShockAPI.GetDataHandlers;

namespace TerraZ_Client
{
    [ApiVersion(2, 1)]
    public class MyPlugin : TerrariaPlugin
    {
        public override string Name => "Client";

        //public static Levels[] players = new Levels[255];
        //public static Dictionary<byte, Levels> players = new Dictionary<byte, Levels>();
        public static List<int> players = new List<int>();
        public static DataBase db;

        public MyPlugin(Main main) : base(main) { }

        public override void Initialize()
        {
            ServerApi.Hooks.GamePostInitialize.Register(this, PostInit);
        }

        public void PostInit(EventArgs eventArgs)
        {
            NetManager.Instance.Register<Net.ClientModule>();

            ServerApi.Hooks.ServerLeave.Register(this, (args) =>
            {
                players.Remove(args.Who);
            });

            TShockAPI.Hooks.PlayerHooks.PlayerPostLogin += (args) =>
            {
                if (players.Contains(args.Player.Index))
                {
                    string permissions = db.GetPerms(args.Player.Group.Name.ToLower());

                    args.Player.GetPlayerInfo().Permissions = permissions;

                    Net.Controller.SendToClient(args.Player, IndexTypes.Authorization, new Dictionary<string, object> {
                        { "IsAuthorized", true }
                        });

                    Net.Controller.SendToClient(args.Player, IndexTypes.Permissions, new Dictionary<string, object> {
                        { "Permission", permissions }
                        });
                }
            };

            TShockAPI.Hooks.PlayerHooks.PlayerLogout += (args) =>
            {
                if (players.Contains(args.Player.Index))
                {
                    args.Player.GetPlayerInfo().Permissions = "";

                    Net.Controller.SendToClient(args.Player, IndexTypes.Authorization, new Dictionary<string, object> {
                        { "IsAuthorized", false }
                    });

                    Net.Controller.SendToClient(args.Player, IndexTypes.Permissions, new Dictionary<string, object> {
                        { "Permission", "" }
                    });
                }
            };

            //TShockAPI.GetDataHandlers.PlayerSlot += OnPlayerSlot;

            ServerApi.Hooks.NetGetData.Register(this, (e) =>
            {
                if (!Main.ServerSideCharacter)
                    return;

                if (e.MsgID == PacketTypes.PlayerSlot)
                {
                    using (var reader = new BinaryReader(new MemoryStream(e.Msg.readBuffer, e.Index, e.Length)))
                    {
                        int playerId = reader.ReadByte();
                        if (Netplay.Clients[playerId].State != 10)
                            return;

                        short slot = reader.ReadInt16();

                        if (slot < 99)
                            return;

                        short stack = reader.ReadInt16();
                        byte prefix = reader.ReadByte();
                        short type = reader.ReadInt16();

                        //Player player7 = Main.player[playerId];

                        //Item item = ((slot >= 220f) ? player7.bank4.item[(int)slot - 220] : ((slot >= 180f) ? player7.bank3.item[(int)slot - 180] : ((slot >= 179f) ? player7.trashItem : ((slot >= 139f) ? player7.bank2.item[(int)slot - 139] : ((slot >= 99f) ? player7.bank.item[(int)slot - 99] : ((slot >= 94f) ? player7.miscDyes[(int)slot - 94] : ((slot >= 89f) ? player7.miscEquips[(int)slot - 89] : ((slot >= 79f) ? player7.dye[(int)slot - 79] : ((!(slot >= 59f)) ? player7.inventory[(int)slot] : player7.armor[(int)slot - 59])))))))));

                        //item.netDefaults(type);
                        //item.stack = stack;
                        //item.prefix = prefix;

                        byte[] data = new PacketFactory()
                           .SetType(5)
                           .PackByte((byte)playerId)
                           .PackInt16(slot)
                           .PackInt16(stack)
                           .PackByte(prefix)
                           .PackInt16(type)
                           .GetByteData();

                        players.Where(plr => plr != playerId).ForEach(plr =>
                        {
                            if (TShock.Players[plr].GetPlayerInfo().HavePermission(Permissions.GetBanks))
                                TShock.Players[plr].SendRawData(data);
                        });
                    }
                }
            }, 16);

            Commands.ChatCommands.Add(new Command((args) =>
            {
                args.Player.SendInfoMessage("Have client: {0}", players.Contains(args.Player.Index) ? "yes":"no");
                
                if (players.Contains(args.Player.Index))
                {
                    args.Player.SendInfoMessage("Your permissions: {0}", args.Player.GetPlayerInfo().Permissions);
                }
            }, "client"));

            IDbConnection DB = new SqliteConnection(string.Format("uri=file://{0},Version=3", Path.Combine(TShock.SavePath, "TZClient.sqlite")));

            db = new DataBase(DB);
        }
    }
}
