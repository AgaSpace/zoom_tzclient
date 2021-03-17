using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
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

		public static void CheckBytes(int bufferIndex = 256)
		{
			lock (NetMessage.buffer[bufferIndex])
			{
				int num = 0;
				int totalData = NetMessage.buffer[bufferIndex].totalData;
				try
				{
					while (totalData >= 2)
					{
						int num3 = BitConverter.ToUInt16(NetMessage.buffer[bufferIndex].readBuffer, num);
						if (totalData >= num3)
						{
							var buffer = NetMessage.buffer[bufferIndex];
							long position = buffer.reader.BaseStream.Position;

							int start = (num + 2);
							int length = (num3 - 2);

							int msgType = buffer.readBuffer[start];

							switch (msgType)
                            {
								case 3:
                                    {
										if (Main.netMode == 1)
										{
											if (Netplay.Connection.State == 1)
											{
												Netplay.Connection.State = 2;
											}
											int num229 = buffer.reader.ReadByte();
											bool value10 = buffer.reader.ReadBoolean();
											Netplay.Connection.ServerSpecialFlags[2] = value10;
											if (num229 != Main.myPlayer)
											{
												Main.player[num229] = Main.ActivePlayerFileData.Player;
												Main.player[Main.myPlayer] = new Player();
											}
											Main.player[num229].whoAmI = num229;
											Main.myPlayer = num229;
											Player player14 = Main.player[num229];

											DataBuilder.SendData((byte)0);

											NetMessage.TrySendData(4, -1, -1, null, num229);
											NetMessage.TrySendData(68, -1, -1, null, num229);
											NetMessage.TrySendData(16, -1, -1, null, num229);
											NetMessage.TrySendData(42, -1, -1, null, num229);
											NetMessage.TrySendData(50, -1, -1, null, num229);
											for (int num230 = 0; num230 < 59; num230++)
											{
												NetMessage.TrySendData(5, -1, -1, null, num229, num230, (int)player14.inventory[num230].prefix);
											}
											for (int num231 = 0; num231 < player14.armor.Length; num231++)
											{
												NetMessage.TrySendData(5, -1, -1, null, num229, 59 + num231, (int)player14.armor[num231].prefix);
											}
											for (int num232 = 0; num232 < player14.dye.Length; num232++)
											{
												NetMessage.TrySendData(5, -1, -1, null, num229, 79 + num232, (int)player14.dye[num232].prefix);
											}
											for (int num233 = 0; num233 < player14.miscEquips.Length; num233++)
											{
												NetMessage.TrySendData(5, -1, -1, null, num229, 89 + num233, (int)player14.miscEquips[num233].prefix);
											}
											for (int num234 = 0; num234 < player14.miscDyes.Length; num234++)
											{
												NetMessage.TrySendData(5, -1, -1, null, num229, 94 + num234, (int)player14.miscDyes[num234].prefix);
											}
											for (int num235 = 0; num235 < player14.bank.item.Length; num235++)
											{
												NetMessage.TrySendData(5, -1, -1, null, num229, 99 + num235, (int)player14.bank.item[num235].prefix);
											}
											for (int num236 = 0; num236 < player14.bank2.item.Length; num236++)
											{
												NetMessage.TrySendData(5, -1, -1, null, num229, 139 + num236, (int)player14.bank2.item[num236].prefix);
											}
											NetMessage.TrySendData(5, -1, -1, null, num229, 179f, (int)player14.trashItem.prefix);
											for (int num237 = 0; num237 < player14.bank3.item.Length; num237++)
											{
												NetMessage.TrySendData(5, -1, -1, null, num229, 180 + num237, (int)player14.bank3.item[num237].prefix);
											}
											for (int num238 = 0; num238 < player14.bank4.item.Length; num238++)
											{
												NetMessage.TrySendData(5, -1, -1, null, num229, 220 + num238, (int)player14.bank4.item[num238].prefix);
											}
											NetMessage.TrySendData(6);
											if (Netplay.Connection.State == 2)
											{
												Netplay.Connection.State = 3;
											}
										}
									}
									break;

								default:
									buffer.GetData(start, length, out var _); 
									break;
							}

							
							buffer.reader.BaseStream.Position = position + num3;
							totalData -= num3;
							num += num3;
							continue;
						}
						break;
					}
				}
				catch (Exception)
				{
					totalData = 0;
					num = 0;
				}
				if (totalData != NetMessage.buffer[bufferIndex].totalData)
				{
					for (int i = 0; i < totalData; i++)
					{
						NetMessage.buffer[bufferIndex].readBuffer[i] = NetMessage.buffer[bufferIndex].readBuffer[i + num];
					}
					NetMessage.buffer[bufferIndex].totalData = totalData;
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
