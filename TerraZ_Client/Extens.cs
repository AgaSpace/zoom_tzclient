using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using System.Reflection;
using Terraria;

namespace TerraZ_Client
{
    public static class Extensions
    {
        public static PlayerInfo GetPlayerInfo(this TSPlayer tsplayer)
        {
            if (!tsplayer.ContainsData(PlayerInfo.Key))
                tsplayer.SetData(PlayerInfo.Key, new PlayerInfo());

            return tsplayer.GetData<PlayerInfo>(PlayerInfo.Key);
        }

        public static int ToInt32(this object line)
        {
            return int.Parse(line.ToString());
        }

        public static long ToInt64(this object line)
        {
            return long.Parse(line.ToString());
        }

        public static short ToInt16(this object line)
        {
            return short.Parse(line.ToString());
        }

        public static byte ToInt8(this object line)
        {
            return byte.Parse(line.ToString());
        }
    }
}
