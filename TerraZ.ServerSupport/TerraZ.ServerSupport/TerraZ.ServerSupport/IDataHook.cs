using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraZ.ServerSupport
{
    public interface IDataHook
    {
        bool Handle(ReceivedDataEventArgs Arguments);
        int PacketIndex { get; }
    }
}
