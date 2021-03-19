using System;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using System.Linq;
using System.Collections.Generic;

namespace TerraZ.ServerSupport
{
    [ApiVersion(2, 1)]
    public class Plugin : TerrariaPlugin
    {
        public Plugin(Main game) : base(game) => Order = int.MinValue;

        public override string Author => "bat627";
        public override string Name => "TerraZ.ServerSupport";
        public override Version Version => base.Version;

        public override void Initialize()
        {
            Instance = new Initializer();
            Instance.BuildGroups();
            Instance.InitializeHooks(this);
        }

        internal static List<Group> ClientGroups;
        internal static Plugin.Initializer Instance;

        public class Initializer
        {
            public void BuildGroups()
            {
                ClientGroups = new List<Group>();
                ClientGroups.Add(Group.Generate("guest", new string[0]));
                ClientGroups.Add(Group.Generate("member", new string[] { "UserInterface/DeleteItems", "UserInterface/BanksAccess", "UserInterface/Freeze", "UserInterface/TShock/Invsee" }));
                ClientGroups.Add(Group.Generate("operator", new string[] { "Operator/UseAll" }));
            }
            public void InitializeHooks(TerrariaPlugin Plugin)
            {
            }
        }
    }
}
