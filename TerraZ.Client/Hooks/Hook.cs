using System;
using System.Runtime.CompilerServices;

namespace TerraZ.Hooks
{
	// Token: 0x02000018 RID: 24
	public class Hook
	{
		internal static Hook Build(HookID Hook, NotReferenceHook Method)
		{
			return new Hook(Hook, Method);
		}
		internal static Hook Build(HookID Hook, ReferenceHook Method)
		{
			return new Hook(Hook, Method);
		}
		private Hook(HookID ID, dynamic Method)
		{
			this.HookID = ID;
			this.Method = Method;
		}
		public HookID HookID { get; private set; }
		[Dynamic]
		public dynamic Method { [return: Dynamic] get; [param: Dynamic] private set; }
	}
}
