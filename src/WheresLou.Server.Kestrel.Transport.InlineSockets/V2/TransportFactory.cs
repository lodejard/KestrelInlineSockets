// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

#if NETSTANDARD2_0
using System;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;
using Microsoft.Extensions.Options;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Logging;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Network;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets
{
    public class TransportFactory : ITransportFactory
    {
        private readonly Func<IEndPointInformation, IConnectionDispatcher, ITransport> _create;

        public TransportFactory(
            IListenerLogger logger,
            IOptions<InlineSocketsOptions> options,
            INetworkProvider networkProvider)
        {
            _create = Create;

            ITransport Create(
                IEndPointInformation endPointInformation,
                IConnectionDispatcher dispatcher)
            {
                return new Transport(
                    logger,
                    options.Value,
                    networkProvider,
                    endPointInformation,
                    dispatcher);
            }
        }

        public virtual ITransport Create(IEndPointInformation endPointInformation, IConnectionDispatcher dispatcher) => _create(endPointInformation, dispatcher);
    }
}
#endif
