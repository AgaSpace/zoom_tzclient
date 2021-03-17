using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Net;

namespace TerraZ_Client.Net
{
    public class ClientModule : NetModule
    {
        public override bool Deserialize(BinaryReader reader, int userId)
        {
            return Controller.Deserialise(TShockAPI.TShock.Players[userId], (IndexTypes)reader.ReadInt16(), reader.ReadString());
        }
    }
}
