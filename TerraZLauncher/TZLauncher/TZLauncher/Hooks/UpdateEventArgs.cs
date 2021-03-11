using System;
using Microsoft.Xna.Framework;

namespace TerraZ.Hooks
{
	public class UpdateEventArgs : EventArgs
	{
		internal UpdateEventArgs(GameTime GameTime)
		{
			this.GameTime = GameTime;
		}

		public GameTime GameTime { get; private set; }
	}
}
