using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

            for (;;)
            {
                switch (Console.ReadLine())
                {
                    case "/pause":
                        LauncherCore.WriteInfoBG("LAUNCHER::PAUSE");
                        t.Suspend();
                        break;
                    case "/resume":
                        LauncherCore.WriteInfoBG("LAUNCHER::RESUME");
                        t.Resume();
                        break;
                    case "/stop":
                        LauncherCore.WriteInfoBG("LAUNCHER::STOP");
                        t.Abort();
                        break;
                    case "/push":
                        LauncherCore.WriteInfoBG("LAUNCHER::PUSH");
                        t = new Thread(() =>
                        {
                            Assembly asm = Assembly.LoadFrom("TZLauncher.exe");
                            asm.Launch(new string[0] { });
                        });
                        t.Start();
                        break;
                }
            }
        }
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
