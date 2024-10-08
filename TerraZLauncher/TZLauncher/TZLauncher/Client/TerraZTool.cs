﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TZLauncher;
using Terraria.Chat;
using Terraria;
using Terraria.UI.Chat;
using Terraria.GameContent;
using ReLogic;
using ReLogic.Graphics;
using TerraZ.ServerData;

namespace TerraZ.Client
{
    public class TerraZTool : ITool
    {
        public void Initialize()
        {
            Main.OnTickForThirdPartySoftwareOnly += Update;
            Main.OnPostDraw += OnDraw;

            Client.UserInterface.Initialize();

            LauncherCore.WriteSuccess("<=- ===-  TerraZ  -=== -=>");
            LauncherCore.WriteInfo("Initialized TerraZ.Client.TerraZTool");
            LauncherCore.WriteInfoBG("> Enter 'bds' to get list of all binds.");

            Launcher.Commands.Add(new Command("bds", (w) =>
            {
                LauncherCore.WriteSuccess("===== TerraZTool =====");
                foreach (Bind b in Binds)
                {
                    LauncherCore.WriteInfoBG("====== [" + b.Key.ToString() + "] ======");
                    LauncherCore.WriteInfo(b.Description + "\n");
                }
            }));

            WorldEditPoints = Point.First;
            RegionDefPoints = Point.First;

            Binds.Add(Bind.CreateBind(Keys.Z, () =>
            {
                if (Client.UserInterface.ShowGUI)
                {
                    Client.UserInterface.ShowGUI = false;
                }

                int player = ClientUtils.GetPlayerOverMouse();

                if (player != -1 && Main.player[player] != null && Main.player[player].active != false)
                {
                    Client.UserInterface.ResetItems();
                    Client.UserInterface.SelectedPlayer = player;

                    Client.UserInterface.ShowGUI = true;
                }
            }, "Выбор игрока на мышке и открытие гуи с этим персонажем"));
            Binds.Add(Bind.CreateBind(Keys.X, () =>
            {
                if (Main.mapFullscreen)
                {
                    int num = Main.maxTilesX * 16;
                    int num2 = Main.maxTilesY * 16;
                    Vector2 vector = new Vector2(Main.mouseX, Main.mouseY);
                    vector.X -= Main.screenWidth / 2;
                    vector.Y -= Main.screenHeight / 2;
                    Vector2 mapFullscreenPos = Main.mapFullscreenPos;
                    Vector2 vector2 = mapFullscreenPos;
                    vector /= 16f;
                    vector *= 16f / Main.mapFullscreenScale;
                    vector2 += vector;
                    vector2 *= 16f;
                    Player player = Main.player[Main.myPlayer];
                    vector2.Y -= player.height;
                    if (vector2.X < 0f)
                    {
                        vector2.X = 0f;
                    }
                    else if (vector2.X + player.width > num)
                    {
                        vector2.X = num - player.width;
                    }
                    if (vector2.Y < 0f)
                    {
                        vector2.Y = 0f;
                    }
                    else if (vector2.Y + player.height > num2)
                    {
                        vector2.Y = num2 - player.height;
                    }
                    player.position = vector2;
                    player.velocity = Vector2.Zero;
                    player.fallStart = (int)(player.position.Y / 16f);
                    NetMessage.SendData(13, -1, -1, null, Main.myPlayer);
                }
                else
                {
                    Main.LocalPlayer.Teleport(new Vector2(Main.mouseX + Main.screenPosition.X, Main.mouseY + Main.screenPosition.Y), 0, 0);
                    NetMessage.SendData(13, -1, -1, null, Main.myPlayer);
                }

            }, "Телепортация к курсору (На карте тоже работает)."));
            #region IDK old Binds
            /*Binds.Add(Bind.CreateBind(Keys.B, () =>
            {
                if (!Client.HasPermission("terraz.worldedit"))
                    return;

                ChatHelper.SendChatMessageFromClient(new ChatMessage("//p" + (int)WorldEditPoints));
                Main.LocalPlayer.PickTile(Player.tileTargetX, Player.tileTargetY, 1);

                if (WorldEditPoints == Point.First) WorldEditPoints = Point.Second;
           else if (WorldEditPoints == Point.Second) WorldEditPoints = Point.First;
            }, "set WorldEdit points."));

            Binds.Add(Bind.CreateBind(Keys.V, () =>
            {
                if (!Client.HasPermission("terraz.regions"))
                    return;

                ChatHelper.SendChatMessageFromClient(new ChatMessage("/region set " + (int)RegionDefPoints));
                Main.LocalPlayer.PickTile(Player.tileTargetX, Player.tileTargetY, 1);

                if (RegionDefPoints == Point.First) RegionDefPoints = Point.Second;
          else if (RegionDefPoints == Point.Second) RegionDefPoints =  Point.First;

            }, "set regions points."));
            Binds.Add(Bind.CreateBind(Keys.G, () =>
            {
                if (!Client.HasPermission("terraz.regions"))
                    return;

                Main.drawingPlayerChat = true;
                Main.chatText = "/region create ~";
            }, "open chat & put '/region create ~' to chat."));
            Binds.Add(Bind.CreateBind(Keys.H, () =>
            {
                if (!Client.HasPermission("terraz.godmode"))
                    return;

                Functions[0].Value = !Functions[0].Value;
            }, "toggle god-mode."));


            Binds.Add(Bind.CreateBind(Keys.F, () =>
            {
                if (!Client.HasPermission("terraz.teleport"))
                    return;

                Main.LocalPlayer.Teleport(new Vector2(Main.mouseX + Main.screenPosition.X, Main.mouseY + Main.screenPosition.Y), 0, 0);
                NetMessage.SendData(13, -1, -1, null, Main.myPlayer);
            }, "teleport to cursor position."));


            Binds.Add(Bind.CreateBind(Keys.Y, () =>
            {
                foreach (Bind b in Binds)
                {
                    Main.NewText("[" + b.Key + "] - " + b.Description, Color.GreenYellow.R, Color.GreenYellow.G, Color.GreenYellow.B);
                }
            }, "list commands."));

            /*Binds.Add(Bind.CreateBind(Keys.F2, () =>
            {
                if (Main.player != null && Main.player[SelectedPlayer] != null && Main.player[SelectedPlayer].active && Client.HasPermission("terraz.playermanage"))
                {
                    Main.LocalPlayer.Teleport(Main.player[SelectedPlayer].position, 0, 0);
                    NetMessage.SendData(13, -1, -1, null, Main.myPlayer);
                    Main.NewText("Teleported to " + Main.player[SelectedPlayer].name, 0, Color.GreenYellow.G, 0);
                }
            }, "teleport to selected player."));
            Binds.Add(Bind.CreateBind(Keys.F3, () =>
            {
                if (Main.player != null && Main.player[SelectedPlayer] != null && Main.player[SelectedPlayer].active && Client.HasPermission("terraz.playermanage"))
                {
                    ChatHelper.SendChatMessageFromClient(new ChatMessage("/kill " + SelectedPlayer));
                }
            }, "kill selected player."));
            Binds.Add(Bind.CreateBind(Keys.F4, () =>
            {
                if (Main.player != null && Main.player[SelectedPlayer] != null && Main.player[SelectedPlayer].active && Main.ServerSideCharacter && Client.HasPermission("terraz.inventories"))
                {
                    ChatHelper.SendChatMessageFromClient(new ChatMessage("/invsee " + SelectedPlayer));
                }
            }, "check selected player's inventory [SSC]."));
            Binds.Add(Bind.CreateBind(Keys.F5, () =>
            {
                if (Main.player != null && Main.player[SelectedPlayer] != null && Main.player[SelectedPlayer].active && Main.ServerSideCharacter && Client.HasPermission("terraz.inventories"))
                {
                    ChatHelper.SendChatMessageFromClient(new ChatMessage("/invsee -s"));
                }
            }, "save manages on selected player's inventory [SSC]."));
            Binds.Add(Bind.CreateBind(Keys.F6, () =>
            {
                if (Main.player != null && Main.player[SelectedPlayer] != null && Main.player[SelectedPlayer].active && Main.ServerSideCharacter && Client.HasPermission("terraz.inventories"))
                {
                    ChatHelper.SendChatMessageFromClient(new ChatMessage("/invsee "));
                }
            }, "back-up your inventory [SSC]."));
            /* 
            if (Main.player != null && Main.player[SelectedPlayer] != null && Main.player[SelectedPlayer].active)
            {
                Main.NewText(Main.player[SelectedPlayer].name, 0, 255, 0);
            } 
            */
            #endregion


            Functions.Add(Function.CreateFunction(false, () =>
            {
                Main.LocalPlayer.noFallDmg = true;
                Main.LocalPlayer.wingAccRunSpeed = 15f;
                Main.LocalPlayer.wingTime = Main.LocalPlayer.wingTimeMax;
                Main.LocalPlayer.statLife = Main.LocalPlayer.statLifeMax;
                Main.LocalPlayer.statMana = Main.LocalPlayer.statManaMax;
                Main.LocalPlayer.dashDelay = 0;
                Main.LocalPlayer.immune = true;
                Main.LocalPlayer.immuneTime = 60;
            }));
        }

        private void OnDraw(GameTime obj)
        {
            Client.UserInterface.Draw();
        }

        private void Update()
        {
            try
            {
                foreach (Function func in Functions)
                {
                    if (func.Value)
                        func.Action();
                }

                foreach (Bind b in Binds)
                {
                    if (Client.UserInterface.ShowGUI)
                    {
                        if (b.Key == Keys.Z)
                        {
                            if (Main.keyState.IsKeyDown(b.Key) && Main.oldKeyState.IsKeyUp(b.Key))
                                b.Action();
                        }
                    }
                }

                if (Main.editChest || Main.editSign || Main.drawingPlayerChat || !Main.hasFocus || Client.UserInterface.ShowGUI) return;

                foreach (Bind b in Binds)
                {
                    if (Main.keyState.IsKeyDown(b.Key) && Main.oldKeyState.IsKeyUp(b.Key))
                        b.Action();
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }

        List<Bind> Binds = new List<Bind>();
        List<Function> Functions = new List<Function>();
        Point WorldEditPoints;
        Point RegionDefPoints;

        class Bind
        {
            public static Bind CreateBind(Keys Key, Action ToDo, string Description) => new Bind() { Key = Key, Action = ToDo, Description = Description };
            public Keys Key;
            public string Description;
            public Action Action;
        }
        class Function
        {
            public static Function CreateFunction(bool Value, Action Action) => new Function() { Value = Value, Action = Action };
            public bool Value;
            public Action Action;
        }
        enum Point
        {
            First = 1, Second = 2
        }
    }
}
