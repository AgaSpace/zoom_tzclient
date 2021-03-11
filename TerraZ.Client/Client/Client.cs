using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using TerraZ.Hooks;

namespace TerraZ.Client
{
	// Token: 0x02000003 RID: 
	public static class Client
	{
		public static void Initialize()
		{
			Client.ClientTools = new List<ITool>();
			Client.ClientPermissions = new Permissions();
			typeof(Main).SetValue("chatMonitor", new ChatMonitor());
		}
		public static void InvokeUpdate(GameTime gt)
		{
			HookRegistrator.InvokeWithoutResult(HookID.Update, new UpdateEventArgs(gt));
		}
		public static void InvokeDraw(GameTime gt)
		{
			HookRegistrator.InvokeWithoutResult(HookID.Draw, new DrawEventArgs(gt));
		}

		public static Permissions ClientPermissions { get; private set; }
		public static List<ITool> ClientTools { get; private set; }
	}
}
