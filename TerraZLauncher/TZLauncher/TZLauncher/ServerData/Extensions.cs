using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TZLauncher.ServerData
{
    public static class Extensions
    {
        public static string ToJson(this object dic)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(dic);
        }

        public static Dictionary<string, object> ToLine(this string jsonFormat)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonFormat);
        }
    }
}
