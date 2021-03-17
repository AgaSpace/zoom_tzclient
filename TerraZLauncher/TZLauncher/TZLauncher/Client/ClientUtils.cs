using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.Localization;
using Terraria.UI.Gamepad;
using TerraZ.ServerData;

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

		public static bool canAgainSendPackage = true;

		public static void CheckBytes(int bufferIndex = 256)
		{
			lock (NetMessage.buffer[bufferIndex])
			{
				int num = 0;
				int num2 = NetMessage.buffer[bufferIndex].totalData;
				try
				{
					while (num2 >= 2)
					{
						int num3 = BitConverter.ToUInt16(NetMessage.buffer[bufferIndex].readBuffer, num);
						if (num2 >= num3)
						{
							long position = NetMessage.buffer[bufferIndex].reader.BaseStream.Position;
							byte b = NetMessage.buffer[bufferIndex].readBuffer[num + 2];

							if (b == 7)
                            {
								if (canAgainSendPackage == true)
                                {
									canAgainSendPackage = false;
									DataBuilder.SendData(0);
								}
							}

							NetMessage.buffer[bufferIndex].GetData(num + 2, num3 - 2, out var _);
							
							NetMessage.buffer[bufferIndex].reader.BaseStream.Position = position + num3;
							num2 -= num3;
							num += num3;
							continue;
						}
						break;
					}
				}
				catch (Exception)
				{
					num2 = 0;
					num = 0;
				}
				if (num2 != NetMessage.buffer[bufferIndex].totalData)
				{
					for (int i = 0; i < num2; i++)
					{
						NetMessage.buffer[bufferIndex].readBuffer[i] = NetMessage.buffer[bufferIndex].readBuffer[i + num];
					}
					NetMessage.buffer[bufferIndex].totalData = num2;
				}
				NetMessage.buffer[bufferIndex].checkBytes = false;
			}
		}

		public static void ThreadPush(this Action t)
        {
			try
			{
				Thread thread = new Thread(() => t());
				thread.Start();
			}
			catch (Exception ex) 
			{
				TZLauncher.LauncherCore.WriteError("            ===== THREAD EXCEPTION =====            ");
				TZLauncher.LauncherCore.WriteErrorBG(ex.ToString());
				Console.ReadLine();
			}
        }
		public static void SendData(this byte[] data)
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
