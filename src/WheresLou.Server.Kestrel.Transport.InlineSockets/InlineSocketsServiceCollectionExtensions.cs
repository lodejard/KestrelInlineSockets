using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;
using Microsoft.Extensions.DependencyInjection;
using WheresLou.Server.Kestrel.Transport.InlineSockets;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class InlineSocketsServiceCollectionExtensions
    {
        public static IServiceCollection AddInlineSocketsTransport(this IServiceCollection services)
        {
            return services
                .AddTransient<ITransportFactory, TransportFactory>()
                .AddTransient<IConnectionFactory, ConnectionFactory>();
        }
    }
}
