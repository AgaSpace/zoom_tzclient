using System;
using System.Runtime.CompilerServices;

namespace TerraZ.Hooks
{
	// Token: 0x02000018 RID: 24
	public class Hook
	{
		// Token: 0x06000064 RID: 100 RVA: 0x000023CD File Offset: 0x000005CD
		internal static Hook Build(HookID Hook, NotReferenceHook Method)
		{
			return new Hook(Hook, Method);
		}

		// Token: 0x06000065 RID: 101 RVA: 0x000023CD File Offset: 0x000005CD
		internal static Hook Build(HookID Hook, ReferenceHook Method)
		{
			return new Hook(Hook, Method);
		}

		// Token: 0x06000066 RID: 102 RVA: 0x000023D6 File Offset: 0x000005D6
		private Hook(HookID ID, dynamic Method)
		{
			this.HookID = ID;
			this.Method = Method;
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000067 RID: 103 RVA: 0x000023EC File Offset: 0x000005EC
		// (set) Token: 0x06000068 RID: 104 RVA: 0x000023F4 File Offset: 0x000005F4
		public HookID HookID { get; private set; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000069 RID: 105 RVA: 0x000023FD File Offset: 0x000005FD
		// (set) Token: 0x0600006A RID: 106 RVA: 0x00002405 File Offset: 0x00000605
		[Dynamic]
		public dynamic Method { [return: Dynamic] get; [param: Dynamic] private set; }
	}
}
