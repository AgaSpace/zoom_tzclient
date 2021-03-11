using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using TerraZ.Hooks;

namespace TerraZ.Client
{
	// Token: 0x02000003 RID: 3
	public static class Client
	{
		// Token: 0x06000002 RID: 2 RVA: 0x00002050 File Offset: 0x00000250
		public static void Initialize()
		{
			Client.ClientTools = new List<ITool>();
			Client.ClientPermissions = new Permissions();
			typeof(Main).SetValue("chatMonitor", new ChatMonitor());
		}

		// Token: 0x06000003 RID: 3 RVA: 0x0000207F File Offset: 0x0000027F
		public static void InvokeUpdate(GameTime gt)
		{
			HookRegistrator.InvokeWithoutResult(HookID.Update, new UpdateEventArgs(gt));
		}

		// Token: 0x06000004 RID: 4 RVA: 0x0000208D File Offset: 0x0000028D
		public static void InvokeDraw(GameTime gt)
		{
			HookRegistrator.InvokeWithoutResult(HookID.Draw, new DrawEventArgs(gt));
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000005 RID: 5 RVA: 0x0000209B File Offset: 0x0000029B
		// (set) Token: 0x06000006 RID: 6 RVA: 0x000020A2 File Offset: 0x000002A2
		public static Permissions ClientPermissions { get; private set; }

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000007 RID: 7 RVA: 0x000020AA File Offset: 0x000002AA
		// (set) Token: 0x06000008 RID: 8 RVA: 0x000020B1 File Offset: 0x000002B1
		public static List<ITool> ClientTools { get; private set; }
	}
}
