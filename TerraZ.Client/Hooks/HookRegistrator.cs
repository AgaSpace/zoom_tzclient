using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace TerraZ.Hooks
{
	// Token: 0x02000011 RID: 17
	public static class HookRegistrator
	{
		// Token: 0x0600004D RID: 77 RVA: 0x0000233B File Offset: 0x0000053B
		public static void Register(HookID HookID, NotReferenceHook Method)
		{
			HookRegistrator.RegisteredHooks.Add(Hook.Build(HookID, Method));
		}

		// Token: 0x0600004E RID: 78 RVA: 0x0000234E File Offset: 0x0000054E
		public static void Register(HookID HookID, ReferenceHook Method)
		{
			HookRegistrator.RegisteredHooks.Add(Hook.Build(HookID, Method));
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00002B8C File Offset: 0x00000D8C
		public static bool InvokeWithResult(HookID HookID, EventArgs Args)
		{
			bool result = false;
			try
			{
				IEnumerable<Hook> registeredHooks = HookRegistrator.RegisteredHooks;
				Func<Hook, bool> <>9__0;
				Func<Hook, bool> predicate;
				if ((predicate = <>9__0) == null)
				{
					predicate = (<>9__0 = ((Hook r) => r.HookID == HookID));
				}
				foreach (Hook hook in registeredHooks.Where(predicate))
				{
					(hook.Method as ReferenceHook)(ref result, Args);
				}
			}
			catch (Exception ex)
			{
				if (Main.gameMenu)
				{
					Main.NewText("Exception: " + ex.ToString(), byte.MaxValue, 0, 0);
				}
			}
			return result;
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00002C4C File Offset: 0x00000E4C
		public static void InvokeWithoutResult(HookID HookID, EventArgs Args)
		{
			try
			{
				IEnumerable<Hook> registeredHooks = HookRegistrator.RegisteredHooks;
				Func<Hook, bool> <>9__0;
				Func<Hook, bool> predicate;
				if ((predicate = <>9__0) == null)
				{
					predicate = (<>9__0 = ((Hook r) => r.HookID == HookID));
				}
				foreach (Hook hook in registeredHooks.Where(predicate))
				{
					(hook.Method as NotReferenceHook)(Args);
				}
			}
			catch (Exception ex)
			{
				if (Main.gameMenu)
				{
					Main.NewText("Exception: " + ex.ToString(), byte.MaxValue, 0, 0);
				}
			}
		}

		// Token: 0x04000023 RID: 35
		internal static List<Hook> RegisteredHooks = new List<Hook>();
	}
}
