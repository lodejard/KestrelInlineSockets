// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

#if NETCOREAPP3_0
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;
using WheresLou.Server.Kestrel.Transport.InlineSockets;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Logging;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Network;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class InlineSocketsExtensions
    {
        public static IServiceCollection AddInlineSocketsTransport(this IServiceCollection services)
        {
            return services
                .AddTransient<IConfigureOptions<InlineSocketsOptions>, InlineSocketsOptionsDefaults>()
                .AddTransient<IConfigureOptions<KestrelServerOptions>, KestrelServerOptionsDefaults>()
                .AddTransient<IConnectionListenerFactory, ConnectionListenerFactory>()
                .AddTransient<INetworkProvider, NetworkProvider>()
                .AddTransient<IListenerLogger, ListenerLogger>()
                .AddTransient<IConnectionLogger, ConnectionLogger>();
        }
    }
}
#endif
