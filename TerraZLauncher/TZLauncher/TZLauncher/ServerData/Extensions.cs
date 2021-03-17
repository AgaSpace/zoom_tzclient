using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraZ.ServerData
{
    public static class Extensions
    {
        public static string ToJson(this object dic)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(dic);
        }

        public static T GetData<T>(this string jsonFormat)
        {
            return (T)Newtonsoft.Json.JsonConvert.DeserializeObject(jsonFormat);
        }
    }
}
