using System;
using System.Collections.Generic;
using System.Text;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets.Internals
{
    public interface INetworkProvider
    {
        INetworkListener CreateListener(NetworkListenerSettings settings);
    }
}
