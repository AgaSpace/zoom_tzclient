using Terraria;
using Microsoft.Xna.Framework;
using TerraZ.Client;
using TerraZ.Hooks;

namespace Client.Tools
{
	public class Core : ITool, IHookChat
	{
		public void Initialize()
		{ }

		public bool OnChat(string msg)
		{
			string[] args = entered.Split(' ');
			switch (args[0])
			{
				case ".test":
					Main.NewText("Test command!");
					break; 
				case ".test2":
					Main.NewText("Second test command!");
					break; 
			}

			return args[0].StartsWith(".");
		}
	}
}