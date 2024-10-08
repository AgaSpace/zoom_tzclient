﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;
using TerraZ.Client;
using Terraria;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TZLauncher
{
    internal class Launcher
    {
        internal static void Main(string[] args)
        {
            try
            {
                TerrariaThread = new Thread(() =>
                {
                    TerrariaAssembly = Assembly.LoadFrom("Terraria.exe");

                    LauncherCore.WriteSuccess("====== " + TerrariaAssembly.FullName + " ======");
                    LauncherCore.WriteSuccessBG("ImageRuntimeVersion::" + TerrariaAssembly.ImageRuntimeVersion);
                    LauncherCore.WriteSuccessBG("Location::" + TerrariaAssembly.Location);

                    TerrariaAssembly.Launch(new string[0] { });
                });
                TerrariaThread.Start();

                CommandsThread = new Thread(() =>
                {
                    while (true)
                    {
                        string sc = Console.ReadLine();
                        try
                        {
                            string[] splt = sc.Split(' ');
                            Command cmd = Commands.Find((e) => e.Name == splt[0]);
                            if (cmd != null) cmd.Delegate(splt);
                            else LauncherCore.WriteError(">>> Command not exists.");
                        } catch (Exception ex) { LauncherCore.WriteError("=====  Exception  ====="); LauncherCore.WriteErrorBG(ex.ToString()); }
                    }
                });
                CommandsThread.Start();

                Terraria.Main.OnEngineLoad += () => Client.Initialize();
                while (true)
                {
                    if (!TerrariaThread.IsAlive) Environment.Exit(0);
                }
            } catch (Exception ex) { Console.WriteLine(ex.ToString()); Console.ReadLine();  }
        }
        static Thread TerrariaThread;
        static Thread CommandsThread;
        internal static Assembly TerrariaAssembly;
        internal static List<Command> Commands = new List<Command>();
        internal static bool DebugMode;
    }
    public class Command
    {
        public Command(string Name, Action<string[]> Delegate)
        {
            this.Name = Name;
            this.Delegate = Delegate;
        }
        public string Name { get; private set; }
        public Action<string[]> Delegate { get; private set; }
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
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(msg + "\n");
            Console.ResetColor();
        }
        public static void WriteInfoBG(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.Write(msg + "\n");
            Console.ResetColor();
        }
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        public static void Launch(this Assembly asm, string[] args) => asm.EntryPoint.Invoke(null, new object[1] { args });
    }
}
