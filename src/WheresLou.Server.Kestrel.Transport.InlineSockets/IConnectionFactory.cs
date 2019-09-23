// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.IO.Pipelines;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Internals;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets
{
    public interface IConnectionFactory
    {
        IConnection CreateConnection(INetworkSocket socket);

        PipeReader CreatePipeReader(IConnection connection, INetworkSocket socket);

        PipeWriter CreatePipeWriter(IConnection connection, INetworkSocket socket);
    }
}
