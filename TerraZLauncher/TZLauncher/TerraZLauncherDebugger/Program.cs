using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Terraria;
using TerraZ.Client;
using ImGuiNET;
using ImGuiXNA;
using System.IO;

namespace TerraZLauncherDebugger
{
    static class Program
    {
        static void Main(string[] args)
        {
            Thread t = new Thread(() =>
            {
                Assembly asm = Assembly.LoadFrom("TZLauncher.exe");
                asm.Launch(new string[0] { });
            });
            t.Start();

            Terraria.Main.OnEngineLoad += () =>
            {
                try
                {
                    XNA = new ImGuiXNAState(Terraria.Main.instance);
                    XNA.BuildTextureAtlas();

                    Terraria.Main.OnPostDraw += DrawGui;
                    Terraria.Main.OnTickForInternalCodeOnly += OnUpdate;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Console.ReadLine();
                }
            };
        }

        private static void OnUpdate()
        {
            try
            {
                if (Godmode)
                {
                    Terraria.Main.LocalPlayer.noFallDmg = true;
                    Terraria.Main.LocalPlayer.wingAccRunSpeed = 15f;
                    Terraria.Main.LocalPlayer.wingTime = Terraria.Main.LocalPlayer.wingTimeMax;
                    Terraria.Main.LocalPlayer.statLife = Terraria.Main.LocalPlayer.statLifeMax;
                    Terraria.Main.LocalPlayer.statMana = Terraria.Main.LocalPlayer.statManaMax;
                    Terraria.Main.LocalPlayer.dashDelay = 0;
                    Terraria.Main.LocalPlayer.immune = true;
                    Terraria.Main.LocalPlayer.immuneTime = 60;
                    Terraria.Main.LocalPlayer.creativeGodMode = true;
                }
            } catch { }
        }

        private static void DrawGui(Microsoft.Xna.Framework.GameTime obj)
        {
            try
            {
                XNA.NewFrame(obj);
                ImGui.Begin("developer", ImGuiWindowFlags.Default);

                if (ImGui.Checkbox("godmode", ref Godmode))
                    Terraria.Main.LocalPlayer.creativeGodMode = false;

                ImGui.BeginChildFrame(0, new ImVec2(250f, 250f), ImGuiWindowFlags.Default);
                foreach (Player p in Terraria.Main.player)
                {
                    if (p.active)
                    {
                        ImGui.Text(p.name);
                        ImGui.SameLine(0, 3);
                        if (ImGui.Button("tp<" + p.name + ">"))
                            Terraria.Main.LocalPlayer.Teleport(p.position, 0, 0);
                    }
                }
                ImGui.EndChildFrame();
                ImGui.DragInt("usetime", ref Terraria.Main.LocalPlayer.HeldItem.useTime, 0.5f, -1, 100, "ut: " + Terraria.Main.LocalPlayer.HeldItem.useTime);
                ImGui.DragInt("useAnimation", ref Terraria.Main.LocalPlayer.HeldItem.useAnimation, 0.5f, -1, 100, "ua: " + Terraria.Main.LocalPlayer.HeldItem.useAnimation);
                ImGui.DragInt("damage", ref Terraria.Main.LocalPlayer.HeldItem.damage, 0.5f, -1, 32767, "damage: " + Terraria.Main.LocalPlayer.HeldItem.damage);
                ImGui.DragInt("stack", ref Terraria.Main.LocalPlayer.HeldItem.stack, 0.5f, -1, 32767, "stack: " + Terraria.Main.LocalPlayer.HeldItem.stack);
                ImGui.End();
                XNA.Render();
            }
            catch (Exception ex) 
            {
                LauncherCore.WriteError(ex.ToString());
            }
        }

        static ImGuiXNAState XNA;

        static bool Godmode;
    }
    public static class LauncherCore
    {
        public static void WriteError(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(msg + "\n");
            Console.ResetColor();
        }
        public static void WriteErrorBG(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Red;
            Console.Write(msg + "\n");
            Console.ResetColor();
        }
        public static void WriteSuccess(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(msg + "\n");
            Console.ResetColor();
        }
        public static void WriteSuccessBG(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Green;
            Console.Write(msg + "\n");
            Console.ResetColor();
        }
        public static void WriteInfo(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(msg + "\n");
            Console.ResetColor();
        }
        public static void WriteInfoBG(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Cyan;
            Console.Write(msg + "\n");
            Console.ResetColor();
        }
        public static void Launch(this Assembly asm, string[] args) => asm.EntryPoint.Invoke(null, new object[1] { args });
    }
}
