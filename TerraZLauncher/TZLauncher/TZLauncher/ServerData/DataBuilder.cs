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

            packet.Writer.Write(index);
            packet.Writer.Write(jsonFormat);

            NetManager.Instance.SendToServer(packet);
        }

        public static void SendData(byte index, object obj)
        {
            string js = obj.ToJson();

            NetPacket packet = new NetPacket(NetManager.Instance.GetId<SyncNetModule>(), 1 + Encoding.UTF8.GetByteCount(js));

            packet.Writer.Write(index);
            packet.Writer.Write(js);

            NetManager.Instance.SendToServer(packet);
        }
    }
}
