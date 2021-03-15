using System;
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

namespace TerraZ.Client
{
    class TerraZTool : ITool
    {
        public void Initialize()
        {
            Main.OnTickForThirdPartySoftwareOnly += Update;
            Main.OnPostDraw += OnDraw;

            UserInterface = new MainUI();
            UserInterface.Initialize();

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
            UserInterface.Draw();
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

                if (Main.editChest || Main.editSign || Main.drawingPlayerChat || !Main.hasFocus || UserInterface.ShowGUI) return;

                foreach (Bind b in Binds)
                {
                    if (Main.keyState.IsKeyDown(b.Key) && Main.oldKeyState.IsKeyUp(b.Key))
                        b.Action();
                }

                OldMouse = NewMouse;
                NewMouse = Mouse.GetState();

                if (OldMouse.ScrollWheelValue < NewMouse.ScrollWheelValue)
                {
                    SelectedPlayer--;
                    if (SelectedPlayer < 0 || SelectedPlayer > 255 || (Main.keyState.IsKeyDown(Keys.Left) && Main.oldKeyState.IsKeyUp(Keys.Left)))
                        SelectedPlayer = 0;

                    if (Main.player != null && Main.player[SelectedPlayer] != null && Main.player[SelectedPlayer].active)
                    {
                        Main.NewText(Main.player[SelectedPlayer].name, 0, 255, 0);
                    }
                }
                else if (OldMouse.ScrollWheelValue > NewMouse.ScrollWheelValue || (Main.keyState.IsKeyDown(Keys.Right) && Main.oldKeyState.IsKeyUp(Keys.Right)))
                {
                    SelectedPlayer++;
                    if (SelectedPlayer < 0 || SelectedPlayer > 255)
                        SelectedPlayer = 0;

                    if (Main.player != null && Main.player[SelectedPlayer] != null && Main.player[SelectedPlayer].active)
                    {
                        Main.NewText(Main.player[SelectedPlayer].name, 0, 255, 0);
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }

        List<Bind> Binds = new List<Bind>();
        List<Function> Functions = new List<Function>();
        Point WorldEditPoints;
        Point RegionDefPoints;

        MouseState OldMouse;
        MouseState NewMouse;

        int SelectedPlayer = 0;
        MainUI UserInterface;

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
