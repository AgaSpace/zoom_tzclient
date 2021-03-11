using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraZ.Hooks;
using TerraZ.Client;
using Microsoft.Xna.Framework.Input;

namespace TerraZ.Client
{
    class TerraZTool : ITool
    {
        public void Initialize()
        {
            HookRegistrator.Register(HookID.Update, Update);
        }
        private void Update(EventArgs args)
        {
            Old = New; New = Keyboard.GetState();
        }

        KeyboardState Old;
        KeyboardState New;
    }
}
