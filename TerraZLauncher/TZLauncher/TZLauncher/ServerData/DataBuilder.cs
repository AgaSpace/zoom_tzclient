using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Net;
using TerraZ.Client;

namespace TerraZ.ServerData
{
    public static class DataBuilder
    {
        public static void SendData(byte index, string jsonFormat = "{ \"none\" }")
        {

            NetPacket packet = new NetPacket(NetManager.Instance.GetId<SyncNetModule>(), 1 + Encoding.UTF8.GetByteCount(jsonFormat));
            
            packet.Writer.Write(byte.Parse(index.ToString()));
            packet.Writer.Write(jsonFormat);

            NetManager.Instance.SendToServer(packet);
        }

        public static void SendData(byte index, object obj)
        {
            SendData(index, obj.ToJson());
        }
    }
}
