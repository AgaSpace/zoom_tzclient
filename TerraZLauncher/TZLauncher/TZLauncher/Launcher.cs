using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;

namespace TZLauncher
{
    internal class Launcher
    {
        internal static async Task Main(string[] args)
        {
            TerrariaThread = new Thread(() =>
            {
                string[] terrazArgs = new string[]
                {
                    // -join = авто-подключение к серверу.
                    "-join s.terraz.ru",
                };
                Terraria = Assembly.LoadFrom("Terraria.exe");
                Terraria.Launch(terrazArgs);
            });
            TerrariaThread.Start();
            TerraZ.Client.Client.Initialize();
            await Task.Delay(-1);
        }
        static Thread TerrariaThread;
        internal static Assembly Terraria;
    }

    public static class LauncherCore
    {
        public static void Launch(this Assembly asm, string[] args) => asm.EntryPoint.Invoke(null, new object[1] { args });
    }
}
