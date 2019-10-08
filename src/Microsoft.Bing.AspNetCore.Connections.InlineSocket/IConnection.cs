// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http.Features;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket
{
    public interface IConnection : IDisposable, IAsyncDisposable
    {
        string ConnectionId { get; }

        IFeatureCollection Features { get; }

        void Abort(ConnectionAbortedException abortReason);

        void OnPipeReaderComplete(Exception exception);

        void OnPipeWriterComplete(Exception exception);
    }
}