using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraZ.Hooks;
using TerraZ.Client;
using Microsoft.Xna.Framework.Input;
using TZLauncher;
using Terraria.Chat;
using Terraria;

namespace TerraZ.Client
{
    class TerraZTool : ITool
    {
        public void Initialize()
        {
            HookRegistrator.Register(HookID.Update, Update);

            LauncherCore.WriteSuccess("<=- ===-  TerraZ  -=== -=>");
            LauncherCore.WriteInfo("Initialized TerraZ.Client.TerraZTool");
            LauncherCore.WriteInfoBG("> Enter 'bds' to get list of all commands & binds.");

            Binds.Add(Bind.CreateBind(Keys.B, () =>
            {
                ChatHelper.SendChatMessageFromClient(new ChatMessage("//p" + (int)RegionDefPoints));

                if (RegionDefPoints == Point.First) RegionDefPoints = Point.Second;
                else RegionDefPoints = Point.Second;
            }, "set WorldEdit points."));

            Binds.Add(Bind.CreateBind(Keys.V, () =>
            {
                ChatHelper.SendChatMessageFromClient(new ChatMessage("/region set " + (int)RegionDefPoints));

                if (RegionDefPoints == Point.First) RegionDefPoints = Point.Second;
                else RegionDefPoints = Point.Second;
            }, "set regions points."));
            Binds.Add(Bind.CreateBind(Keys.G, () =>
            {
                Main.drawingPlayerChat = true;
                Main.chatText = "/region create ~";
            }, "open chat & put '/region create ~' to chat input."));
        }
        private void Update(EventArgs args)
        {
            foreach (Bind b in Binds)
            {
                if (Main.keyState.IsKeyDown(b.Key) && Main.oldKeyState.IsKeyUp(b.Key))
                    b.Action();
            }
        }

        List<Bind> Binds = new List<Bind>();
        Point WorldEditPoints;
        Point RegionDefPoints;

        class Bind
        {
            public static Bind CreateBind(Keys Key, Action ToDo, string Description) => new Bind() { Key = Key, Action = ToDo, Description = Description };
            public Keys Key;
            public string Description;
            public Action Action;
        }
        enum Point
        {
            First = 1, Second = 2
        }
    }
}
