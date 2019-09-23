// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;
using Microsoft.Extensions.Options;
using WheresLou.Server.Kestrel.Transport.InlineSockets;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Internals;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class InlineSocketsTransportServiceCollectionExtensions
    {
        public static IServiceCollection AddInlineSocketsTransport(this IServiceCollection services)
        {
            return services
                .AddTransient<IConfigureOptions<InlineSocketsTransportOptions>, InlineSocketsTransportOptionsDefaults>()
                .AddTransient<ITransportFactory, TransportFactory>()
                .AddTransient<IConnectionFactory, ConnectionFactory>()
                .AddTransient<INetworkProvider, NetworkProvider>();
        }
    }
}
