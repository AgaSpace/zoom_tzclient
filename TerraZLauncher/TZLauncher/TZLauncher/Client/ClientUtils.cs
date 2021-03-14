using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;

namespace TerraZ.Client
{
	public static class ClientUtils
	{
		public static int GetPlayerOverMouse()
		{
			Player[] p = Main.player;
			Rectangle rectangle = new Rectangle(Main.mouseX + (int)Main.screenPosition.X, Main.mouseY + (int)Main.screenPosition.Y, 1, 1);
			for (int i = 0; i < 255; i++)
				if (p[i].active && Main.myPlayer != i && !p[i].dead)
				{
					Rectangle value = new Rectangle((int)(p[i].position.X + p[i].width * 0.5 - 16.0), (int)(p[i].position.Y + p[i].height - 48f), 32, 48);
					if (rectangle.Intersects(value))
					{
						return i;
					}
				}
			return -1;
		}
		public static void SendData(byte[] data)
		{
			Netplay.Connection.Socket.AsyncSend(data, 0, data.Length, Netplay.Connection.ClientWriteCallBack, null);
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
