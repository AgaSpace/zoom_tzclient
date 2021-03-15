using System.Collections.Generic;
using System.Linq;

namespace TerraZ_Client
{
    public class PlayerInfo
    {
        public const string Key = "Client_Data";

        public string Permissions = "";
        public List<string> Permis
        {
            get
            {
                string[] permis = Permissions.Split(',');

                return permis.ToList();
            }
        }

        public bool HavePermission(string permission)
        {
            if (Permis.Contains("*") || Permis.Contains("superadmin"))
                return true;

            if (Permis.Contains(permission))
                return true;

            return false;
        }
    }
}
