using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using System.Runtime.CompilerServices;
using System.Reflection;

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
		}

        private static void OnEnterWorld(Player plr)
        {
			if (plr.whoAmI == Main.myPlayer)
				new PacketWriter()
					.SetType(15)
					.PackInt16(0)
					.GetByteData()
					
					.SendData();
        }

        public  static bool HasPermission(string Permission) => ClientPermissions.HasPermission(Permission);
		internal static Permissions ClientPermissions { get; private set; }
		public  static List<ITool> ClientTools { get; private set; }
	}
}
