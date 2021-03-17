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

namespace TerraZ_Client
{
    [ApiVersion(2, 1)]
    public class MyPlugin : TerrariaPlugin
    {
        public override string Name => "Client";

        public static Levels[] players = new Levels[255];
        public static DataBase db;

        public MyPlugin(Main main) : base(main) { }

        public override void Initialize()
        {
            for (int i = 0; i < 255; i++)
            {
                players[i] = Levels.None;
            }

            Hooks.Net.ReceiveData = new Hooks.Net.ReceiveDataHandler(OnReceiveData);

            ServerApi.Hooks.GamePostInitialize.Register(this, PostInit);
        }

        private static HookResult OnReceiveData(MessageBuffer buffer, ref byte packetId, ref int readOffset, ref int start, ref int length)
        {
            if (packetId != 15)
                if (!Enum.IsDefined(typeof(PacketTypes), (int)packetId))
                {
                    return HookResult.Cancel;
                }

            //if (NetHooks._hookManager.InvokeNetGetData(ref packetId, buffer, ref readOffset, ref length))

            object[] parametr = new object[] { packetId, buffer, readOffset, length };

            Type[] types = new[] { typeof(byte).MakeByRefType(), typeof(MessageBuffer), typeof(int).MakeByRefType(), typeof(int).MakeByRefType() };

            bool value = (bool)ServerApi.Hooks.GetType().GetMethod("InvokeNetGetData", 

                BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                CallingConventions.Any,
                types,
                null)
                
            .Invoke(ServerApi.Hooks, parametr);

            packetId = (byte)parametr[0];
            readOffset = (int)parametr[2];
            length = (int)parametr[3];

            if (value)
            {
                return HookResult.Cancel;
            }
            return HookResult.Continue;
        }

        public void PostInit(EventArgs eventArgs)
        {
            NetManager.Instance.Register<Net.ClientModule>();

            ServerApi.Hooks.ServerLeave.Register(this, (args) =>
            {
                players[args.Who] = Levels.None;
            });

            TShockAPI.Hooks.PlayerHooks.PlayerPostLogin += (args) =>
            {
                if (players[args.Player.Index] != Levels.None)
                {
                    string permissions = db.GetPerms(args.Player.Group.Name.ToLower());

                    args.Player.GetPlayerInfo().Permissions = permissions;

                    Net.Controller.SendToClient(args.Player, IndexTypes.Authorization, new Dictionary<string, object> {
                        { "IsAuthorized", true } ,
                        { "Permission", permissions }
                        });
                }
            };

            TShockAPI.Hooks.PlayerHooks.PlayerLogout += (args) =>
            {
                if (players[args.Player.Index] != Levels.None)
                {
                    args.Player.GetPlayerInfo().Permissions = "";

                    Net.Controller.SendToClient(args.Player, IndexTypes.Authorization, new Dictionary<string, object> {
                        { "IsAuthorized", false } ,
                        { "Permission", "" }
                    });
                }
            };

            Commands.ChatCommands.Add(new Command((args) =>
            {
                args.Player.SendInfoMessage("Have client: {0}", players[args.Player.Index].ToString());
                if (players[args.Player.Index] == Levels.ClientUser)
                {
                    args.Player.SendInfoMessage("Your permissions: {0}", args.Player.GetPlayerInfo().Permissions);
                }
            }, "client"));

            IDbConnection DB = new SqliteConnection(string.Format("uri=file://{0},Version=3", Path.Combine(TShock.SavePath, "TZClient.sqlite")));

            db = new DataBase(DB);
        }
    }
}
