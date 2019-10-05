// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http.Features;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets
{
    public interface IConnection : IDisposable
    {
        IFeatureCollection Features { get; }

        void Abort(ConnectionAbortedException abortReason);

        Task DisposeAsync();

        void OnPipeReaderComplete(Exception exception);

        void OnPipeWriterComplete(Exception exception);
    }
}
