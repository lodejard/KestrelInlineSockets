using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets.Internals
{
    public class NetworkListenerSettings
    {
        public IEndPointInformation EndPointInformation { get; set; }
        public bool? AllowNatTraversal { get; set; }
        public bool? ExclusiveAddressUse { get; set; }
        public int? ListenerBacklog { get; set; }
    }
}
