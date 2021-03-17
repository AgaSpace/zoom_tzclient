using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Terraria;
using Terraria.Net;
using TerraZ.ServerData;

namespace TerraZ.Client
{
	public static class Client
	{
		public static void Initialize()
		{
			ClientTools = new List<ITool>();
			ClientTools.Add(new TerraZTool());
			foreach (ITool t in ClientTools) t.Initialize();
			ClientPermissions = new Permissions();

			NetManager.Instance.Register<SyncNetModule>();

			Main.chatMonitor = new ChatMonitor();

			ReplaceMethod(typeof(NetMessage).GetMethod("CheckBytes", Flags), typeof(ClientUtils).GetMethod("CheckBytes", Flags));
		}
		public static BindingFlags Flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
		public static unsafe void ReplaceMethod(MethodInfo _from, MethodInfo _to) // ReplaceMethod(typeof(Type1).GetMethod("Method"), typeof(Type2).GetMethod("Method2"));
		{
			*((int*)_from.MethodHandle.Value.ToPointer() + 2) = *((int*)_to.MethodHandle.Value.ToPointer() + 2);
		}

		public   static bool HasPermission(string Permission) => ClientPermissions.HasPermission(Permission);
		internal static Permissions ClientPermissions { get; private set; }
		public   static List<ITool> ClientTools { get; private set; }
	}
}
