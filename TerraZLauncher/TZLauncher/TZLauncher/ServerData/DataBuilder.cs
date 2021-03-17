using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Net;

namespace TerraZ.ServerData
{
    public static class DataBuilder
    {
        public static void SendData(byte index, string jsonFormat)
        {
            NetPacket packet = new NetPacket(NetManager.Instance.GetId<SyncNetModule>(), 1 + Encoding.UTF8.GetByteCount(jsonFormat));

            packet.Writer.Write(byte.Parse(2.ToString()));
            packet.Writer.Write(index);

            NetManager.Instance.SendToServer(packet);
        }
    }
}
