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
		}

		public  static bool HasPermission(string Permission) => ClientPermissions.HasPermission(Permission);
		internal static Permissions ClientPermissions { get; private set; }
		public  static List<ITool> ClientTools { get; private set; }
	}
}
