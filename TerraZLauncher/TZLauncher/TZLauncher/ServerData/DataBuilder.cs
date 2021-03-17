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
            SendData(index, obj.ToJson());
        }



        public static void SendData(PacketID index, string jsonFormat)
        {
            SendData((byte)index, jsonFormat);
        }

        public static void SendData(PacketID index, object obj)
        {
            SendData((byte)index, obj.ToJson());
        }



        public static void SendData(byte index)
        {
            SendData(index, "{}");
        }

        public static void SendData(PacketID index)
        {
            SendData((byte)index, "{}");
        }
    }
}
