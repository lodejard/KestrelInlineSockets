// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.IO.Pipelines;
using Microsoft.AspNetCore.Connections.Features;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets
{
    public partial class Connection : IConnectionTransportFeature, IDuplexPipe
    {
        public IDuplexPipe Transport
        {
            get => _transport;
            set => _transport = value;
        }

        PipeReader IDuplexPipe.Input => _connectionPipeReader;

        PipeWriter IDuplexPipe.Output => _connectionPipeWriter;
    }
}
