using TerraZ.Client;

namespace TerraZ.ServerData
{
    public class BanksRequest
    {
        public BanksRequest(byte index)
        {
            this.PlayerIndex = index;
        }

        public byte PlayerIndex;

        public void Send() => DataBuilder.SendData(byte.Parse(4.ToString()), this.ToJson());
    }
}
