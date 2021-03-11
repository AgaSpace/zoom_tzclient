using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

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

        public static int ToInt32(this string line)
        {
            return int.Parse(line);
        }

        public static long ToInt64(this string line)
        {
            return long.Parse(line);
        }

        public static short ToInt16(this string line)
        {
            return short.Parse(line);
        }

        public static byte ToInt8(this string line)
        {
            return byte.Parse(line);
        }
    }
}
