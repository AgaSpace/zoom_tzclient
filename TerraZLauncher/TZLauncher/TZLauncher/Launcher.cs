using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;
using TerraZ.Client;
using Terraria;
using System.IO;

namespace TZLauncher
{
    internal class Launcher
    {
        internal static void Main(string[] args)
        {
            try
            {
                Console.Title = "TerraZ Injection";
                TerrariaThread = new Thread(() =>
                {
                    string[] terrazArgs = new string[]
                    {
                        // -join = авто-подключение к серверу.
                        "-join",
                        "s.terraz.ru"
                    };
                    TerrariaAssembly = Assembly.LoadFrom("Terraria.exe");
                    TerrariaAssembly.Launch(terrazArgs);
                });
                TerrariaThread.Start();
                Client.Initialize();
                while (true)
                {
                    if (Netplay.ServerIPText != "s.terraz.ru")
                    {
                        Netplay.Disconnect = true;
                        Terraria.Main.statusText = "Неподтвержденный сервер.";
                    }
                    Terraria.Main.getIP = "s.terraz.ru";
                    Console.WriteLine(Terraria.Main.getIP);
                }
            } catch (Exception ex) { Console.WriteLine(ex.ToString());  }
        }
        static Thread TerrariaThread;
        internal static Assembly TerrariaAssembly;
    }

    public static class LauncherCore
    {
        public static void Launch(this Assembly asm, string[] args) => asm.EntryPoint.Invoke(null, new object[1] { args });
    }
}
