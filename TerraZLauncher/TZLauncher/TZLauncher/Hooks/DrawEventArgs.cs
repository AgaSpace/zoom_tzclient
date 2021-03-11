using System;
using Microsoft.Xna.Framework;

namespace TerraZ.Hooks
{
	// Token: 0x02000016 RID: 22
	public class DrawEventArgs : EventArgs
	{
		// Token: 0x0600005E RID: 94 RVA: 0x0000238D File Offset: 0x0000058D
		internal DrawEventArgs(GameTime GameTime)
		{
			this.GameTime = GameTime;
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600005F RID: 95 RVA: 0x0000239C File Offset: 0x0000059C
		// (set) Token: 0x06000060 RID: 96 RVA: 0x000023A4 File Offset: 0x000005A4
		public GameTime GameTime { get; private set; }
	}
}
