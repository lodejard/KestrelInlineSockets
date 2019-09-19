using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;
using Microsoft.Extensions.DependencyInjection;
using WheresLou.Server.Kestrel.Transport.InlineSockets;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Factories;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class InlineSocketsServiceCollectionExtensions
    {
        public static IServiceCollection AddInlineSocketsTransport(this IServiceCollection services)
        {
            return services
                .AddTransient<ITransportFactory, TransportFactory>()
                .AddTransient<IConnectionFactory, ConnectionFactory>()
                .AddTransient<IConnectionPipeReaderFactory, ConnectionPipeReaderFactory>()
                .AddTransient<IConnectionPipeWriterFactory, ConnectionPipeWriterFactory>()
                ;
        }
    }
}
