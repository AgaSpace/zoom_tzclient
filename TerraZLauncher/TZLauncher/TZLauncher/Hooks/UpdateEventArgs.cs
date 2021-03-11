using System;
using Microsoft.Xna.Framework;

namespace TerraZ.Hooks
{
	// Token: 0x02000017 RID: 23
	public class UpdateEventArgs : EventArgs
	{
		// Token: 0x06000061 RID: 97 RVA: 0x000023AD File Offset: 0x000005AD
		internal UpdateEventArgs(GameTime GameTime)
		{
			this.GameTime = GameTime;
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000062 RID: 98 RVA: 0x000023BC File Offset: 0x000005BC
		// (set) Token: 0x06000063 RID: 99 RVA: 0x000023C4 File Offset: 0x000005C4
		public GameTime GameTime { get; private set; }
	}
}
