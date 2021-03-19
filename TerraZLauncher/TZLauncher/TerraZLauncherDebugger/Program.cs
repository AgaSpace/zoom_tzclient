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
                XNA = new ImGuiXNAState(Terraria.Main.instance);
                if (!File.Exists(Environment.CurrentDirectory + "\\imgui.ini"))
                    File.Create(Environment.CurrentDirectory + "\\imgui.ini");
                XNA.BuildTextureAtlas();

                Terraria.Main.OnPostDraw += DrawGui;
                Terraria.Main.OnTickForInternalCodeOnly += OnUpdate;
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
                if (Netplay.Disconnect) return;

                XNA.NewFrame(obj);

                if (ImGui.Checkbox("godmode", ref Godmode))
                    Terraria.Main.LocalPlayer.creativeGodMode = false;

                if (ImGui.Button("add tool")) Client.ClientTools.Add(new TerraZTool());

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
