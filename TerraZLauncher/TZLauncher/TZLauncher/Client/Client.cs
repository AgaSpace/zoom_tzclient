using System;
using System.Collections.Generic;
using System.Text;
using Terraria;
using Terraria.Net;
using TerraZ.ServerData;

namespace TerraZ.Client
{
	public static class Client
	{
		public static unsafe void Initialize()
		{
			ClientTools = new List<ITool>();
			ClientTools.Add(new TerraZTool());
			foreach (ITool t in ClientTools) t.Initialize();
			ClientPermissions = new Permissions();

			NetManager.Instance.Register<SyncNetModule>();

			Main.chatMonitor = new ChatMonitor();
			Player.Hooks.OnEnterWorld += OnEnterWorld;
		}

        private static void OnEnterWorld(Player plr)
        {
			DataBuilder.SendData(0, "{ }");
		}

        public   static bool HasPermission(string Permission) => ClientPermissions.HasPermission(Permission);
		internal static Permissions ClientPermissions { get; private set; }
		public   static List<ITool> ClientTools { get; private set; }
	}
}
