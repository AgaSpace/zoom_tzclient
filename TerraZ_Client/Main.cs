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
        public static Dictionary<byte, Levels> players = new Dictionary<byte, Levels>();
        public static DataBase db;

        public MyPlugin(Main main) : base(main) { }

        public override void Initialize()
        {
            for (byte i = 0; i < 255; i++)
            {
                players[i] = Levels.None;
            }

            ServerApi.Hooks.GamePostInitialize.Register(this, PostInit);
        }

        public void PostInit(EventArgs eventArgs)
        {
            NetManager.Instance.Register<Net.ClientModule>();

            ServerApi.Hooks.ServerLeave.Register(this, (args) =>
            {
                players.Remove(args.Who.ToInt8());
            });

            TShockAPI.Hooks.PlayerHooks.PlayerPostLogin += (args) =>
            {
                if (players.ContainsKey(args.Player.Index.ToInt8()))
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
                if (players.ContainsKey(args.Player.Index.ToInt8()))
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
                if (e.MsgID == PacketTypes.PlayerSlot)
                {
                    using (var reader = new BinaryReader(new MemoryStream(e.Msg.readBuffer, e.Index, e.Length)))
                    {
                        int playerId = reader.ReadByte();

                        if (playerId == e.Msg.whoAmI)
                            return;

                        int slot = reader.ReadInt16();

                        if (slot < 99)
                            return;

                        int stack = reader.ReadInt16();
                        int prefix = reader.ReadByte();
                        int type = reader.ReadInt16();

                        players.ForEach(plr =>
                        {
                            TShock.Players[plr.Key].SendData(PacketTypes.PlayerSlot, "", playerId, slot, prefix);
                        });
                    }
                }
            }, 16);

            Commands.ChatCommands.Add(new Command((args) =>
            {
                args.Player.SendInfoMessage("Have client: {0}", players[(byte)args.Player.Index].ToString());
                if (players[(byte)args.Player.Index] == Levels.ClientUser)
                {
                    args.Player.SendInfoMessage("Your permissions: {0}", args.Player.GetPlayerInfo().Permissions);
                }
            }, "client"));

            IDbConnection DB = new SqliteConnection(string.Format("uri=file://{0},Version=3", Path.Combine(TShock.SavePath, "TZClient.sqlite")));

            db = new DataBase(DB);
        }
    }
}
