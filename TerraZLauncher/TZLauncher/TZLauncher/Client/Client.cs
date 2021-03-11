using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using TerraZ.Hooks;

namespace TerraZ.Client
{
	public static class Client
	{
		public static void Initialize()
		{
			Client.ClientTools = new List<ITool>();
			Client.ClientPermissions = new Permissions();
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
