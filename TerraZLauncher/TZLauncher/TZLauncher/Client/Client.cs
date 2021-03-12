using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace TerraZ.Client
{
	public static class Client
	{
		public static void Initialize()
		{
			ClientTools = new List<ITool>();
			ClientTools.Add(new TerraZTool());
			foreach (ITool t in ClientTools) t.Initialize();
			ClientPermissions = new Permissions();
		}

		public static Permissions ClientPermissions { get; private set; }
		public static List<ITool> ClientTools { get; private set; }
	}
}
