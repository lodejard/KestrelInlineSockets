// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets
{
    public interface IConnection : IDisposable
    {
        TransportConnection TransportConnection { get; }

        void OnPipeReaderComplete(Exception exception);

        void OnPipeWriterComplete(Exception exception);
    }
}
