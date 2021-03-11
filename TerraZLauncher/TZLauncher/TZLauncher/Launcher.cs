﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;
using TerraZ.Client;

namespace TZLauncher
{
    internal class Launcher
    {
        internal static async Task Main(string[] args)
        {
            Console.Title = "TerraZ: Starting...";
            TerrariaThread = new Thread(() =>
            {
                string[] terrazArgs = new string[]
                {
                    // -join = авто-подключение к серверу.
                    "-join",
                    "s.terraz.ru"
                };
                Terraria = Assembly.LoadFrom("Terraria.exe");
                Terraria.Launch(terrazArgs);
            });
            TerrariaThread.Start();
            TerraZ.Client.Client.Initialize();
            while (true)
            {
                if (TerrariaThread.IsAlive) Console.Title = "TerraZ: Running";
                else Console.Title = "TerraZ: Not running";
                TerraZ.Client.Client.InvokeUpdate(null);
                TerraZ.Client.Client.InvokeDraw(null);
                Terraria.GetType("Terraria.Main").SetValue("getIP", "s.terraz.ru");
            }
        }
        static Thread TerrariaThread;
        internal static Assembly Terraria;
        static string[] RecentWorlds = new string[0];
        internal static Timer ChooseServerTimer;
    }

    public static class LauncherCore
    {
        public static void Launch(this Assembly asm, string[] args) => asm.EntryPoint.Invoke(null, new object[1] { args });
    }
}
