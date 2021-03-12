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
		public static int GetPlayerOverMouse()
		{
			Player[] p = Main.player;
			Rectangle rectangle = new Rectangle(
				(int)((float)Main.mouseX + Main.screenPosition.X), 
				(int)((float)Main.mouseY + Main.screenPosition.Y), 1, 1);
			for (int i = 0; i < 255; i++)
			{
				if (p[i].active && Main.myPlayer != i && !p[i].dead)
				{
					Rectangle value = new Rectangle((int)((double)p[i].position.X + (double)p[i].width * 0.5 - 16.0), (int)(p[i].position.Y + (float)p[i].height - 48f), 32, 48);
					if (rectangle.Intersects(value))
					{
						return i;
					}
				}
			}
			return -1;
		}

		public static T GetStaticValue<T>(this Type type, string Field)
		{
			return (T)((object)type.GetField(Field, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).GetValue(null));
		}
		public static T GetValue<T>(this Type type, string Field)
		{
			return (T)((object)type.GetField(Field, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).GetValue(type));
		}
		public static void SetStaticValue(this Type type, string Field, object Value)
		{
			type.GetField(Field, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).SetValue(null, Value);
		}
		public static void SetValue(this Type type, string Field, object Value)
		{
			type.GetField(Field, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).SetValue(type, Value);
		}
		public static void StaticInvoke(this Type type, string Method, params object[] Value)
		{
			type.Invoke(null, Value);
		}
		public static void Invoke(this Type type, string Method, params object[] Value)
		{
			type.Invoke(Method, Value);
		}
	}
}
