using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Terraria;

namespace TerraZ.Client
{
	public static class ClientUtils
	{
		public static Player GetPlayerOverMouse()
		{
			Rectangle rectangle = new Rectangle((int)((float)Main.mouseX + Main.screenPosition.X), (int)((float)Main.mouseY + Main.screenPosition.Y), 1, 1);
			for (int i = 0; i < 255; i++)
			{
				if (Main.player[i].active && Main.myPlayer != i && !Main.player[i].dead)
				{
					Rectangle value = new Rectangle((int)((double)Main.player[i].position.X + (double)Main.player[i].width * 0.5 - 16.0), (int)(Main.player[i].position.Y + (float)Main.player[i].height - 48f), 32, 48);
					if (rectangle.Intersects(value))
					{
						return Main.player[i];
					}
				}
			}
			return null;
		}
		public static IEnumerable<string> GetPlayersNames()
		{
			foreach (Player player in from w in Main.player
			where w.active
			select w)
			{
				yield return player.name;
			}
		}
		public static IEnumerable<Player> GetActivePlayers()
		{
			foreach (Player player in from w in Main.player where w.active select w)
			{
				yield return player;
			}
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002194 File Offset: 0x00000394
		public static T GetStaticValue<T>(this Type type, string Field)
		{
			return (T)((object)type.GetField(Field, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).GetValue(null));
		}

		// Token: 0x0600001E RID: 30 RVA: 0x000021AA File Offset: 0x000003AA
		public static T GetValue<T>(this Type type, string Field)
		{
			return (T)((object)type.GetField(Field, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).GetValue(type));
		}

		// Token: 0x0600001F RID: 31 RVA: 0x000021C0 File Offset: 0x000003C0
		public static void SetStaticValue(this Type type, string Field, object Value)
		{
			type.GetField(Field, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).SetValue(null, Value);
		}

		// Token: 0x06000020 RID: 32 RVA: 0x000021D2 File Offset: 0x000003D2
		public static void SetValue(this Type type, string Field, object Value)
		{
			type.GetField(Field, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).SetValue(type, Value);
		}
	}
}
