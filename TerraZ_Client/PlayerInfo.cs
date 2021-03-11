namespace TerraZ_Client
{
    public class PlayerInfo
    {
        public const string Key = "Client_Data";

        public string Permissions = "";
        public string[] Permis
        {
            get
            {
                return Permissions.Split(',');
            }
        }
    }
}
