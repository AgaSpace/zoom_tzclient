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
			ClientTools = new List<ITool>();
			ClientTools.Add(new TerraZTool());
			foreach (ITool t in ClientTools) t.Initialize();
			ClientPermissions = new Permissions();
		}
		public static void InvokeUpdate(GameTime gt)
		{
			try
			{
				HookRegistrator.InvokeWithoutResult(HookID.Update, new UpdateEventArgs(gt));
			} catch (Exception ex) { Console.WriteLine(ex.ToString()); }
		}

		public static void InvokeDraw(GameTime gt)
		{
			try
			{
				HookRegistrator.InvokeWithoutResult(HookID.Draw, new DrawEventArgs(gt));
			} catch (Exception ex) { Console.WriteLine(ex.ToString());  }
		}

		public static Permissions ClientPermissions { get; private set; }
		public static List<ITool> ClientTools { get; private set; }
	}
}
