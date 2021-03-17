using System.Collections.Generic;
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

			Main.chatMonitor = new ChatMonitor();
            Player.Hooks.OnEnterWorld += OnEnterWorld;

			NetManager.Instance.Register<SyncNetModule>();
		}

        private static void OnEnterWorld(Player plr)
        {
			if (plr.whoAmI == Main.myPlayer)
				new PacketWriter()
					.SetType(82)
					.PackInt16(0)
					.GetByteData()
					
					.SendData();
        }

        public  static bool HasPermission(string Permission) => ClientPermissions.HasPermission(Permission);
		internal static Permissions ClientPermissions { get; private set; }
		public  static List<ITool> ClientTools { get; private set; }
	}
}
