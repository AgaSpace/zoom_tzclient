using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraZ.ServerSupport
{
    public class ReceivedDataEventArgs
    {
        public static ReceivedDataEventArgs Build(byte[] Stream, int PlayerIndex)
        {
            byte[] b = new byte[256];
            Array.Copy(Stream, b, Stream.Length);

            ReceivedDataEventArgs args = new ReceivedDataEventArgs();
            args.ActiveStream = new MemoryStream(b);
            args.Reader = new BinaryReader(args.ActiveStream);
            args.PlayerIndex = PlayerIndex;

            return args;
        }
        private ReceivedDataEventArgs() {}

        public MemoryStream ActiveStream { get; private set; }
        public BinaryReader Reader { get; private set; }
        public int PlayerIndex { get; private set; }
    }
}
