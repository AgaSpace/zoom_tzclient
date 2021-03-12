using System;
using Microsoft.Xna.Framework;

namespace TerraZ.Hooks
{
	public class DrawEventArgs : EventArgs
	{
		internal DrawEventArgs(GameTime GameTime)
		{
			this.GameTime = GameTime;
		}

		public GameTime GameTime { get; private set; }
	}
}
