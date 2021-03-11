using System;
using System.Collections.Generic;
using System.Linq;
using TerraZ.Client;

namespace TerraZ.Hooks
{
	public static class HookRegistrator
	{
		public static void Register(HookID HookID, NotReferenceHook Method) => HookRegistrator.RegisteredHooks.Add(Hook.Build(HookID, Method));
		public static void Register(HookID HookID, ReferenceHook Method) => HookRegistrator.RegisteredHooks.Add(Hook.Build(HookID, Method));
		public static bool InvokeWithResult(HookID HookID, EventArgs Args)
		{
			bool result = false;
			try
			{
				foreach (Hook hook in RegisteredHooks.Where((Hook r) => r.HookID == HookID))
				{
					(hook.Method as ReferenceHook)(ref result, Args);
				}
			}
			catch (Exception ex)
			{
				if (TZLauncher.Launcher.Terraria.GetType("Main").GetValue<bool>("gameMenu"))
				{
					TZLauncher.Launcher.Terraria.GetType("Main").StaticInvoke("NewText", "Exception: " + ex.ToString(), byte.MaxValue, 0, 0);
				}
			}
			return result;
		}
		public static void InvokeWithoutResult(HookID HookID, EventArgs Args)
		{
			try
			{
				foreach (Hook hook in RegisteredHooks.Where((Hook r) => r.HookID == HookID))
				{
					(hook.Method as NotReferenceHook)(Args);
				}
			}
			catch (Exception ex)
			{
				if (TZLauncher.Launcher.Terraria.GetType("Main").GetValue<bool>("gameMenu"))
				{
					TZLauncher.Launcher.Terraria.GetType("Main").StaticInvoke("NewText", "Exception: " + ex.ToString(), byte.MaxValue, 0, 0);
				}
			}
		}

		internal static List<Hook> RegisteredHooks = new List<Hook>();
	}
}
