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
                Assembly asm = Assembly.LoadFrom("Terraria.exe");
                asm.Launch(terrazArgs);
            });
            TerrariaThread.Start();
            await Task.Delay(-1);
        }
        static Thread TerrariaThread;
    }

    public static class LauncherCore
    {
        public static void Launch(this Assembly asm, string[] args) => asm.EntryPoint.Invoke(null, new object[1] { args });
    }
}
