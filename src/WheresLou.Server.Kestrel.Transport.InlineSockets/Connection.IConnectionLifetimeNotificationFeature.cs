// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Threading;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.Extensions.Logging;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets
{
    public partial class Connection : IConnectionLifetimeNotificationFeature
    {
        private readonly CancellationTokenSource _connectionCloseRequestedSource = new CancellationTokenSource();

        CancellationToken IConnectionLifetimeNotificationFeature.ConnectionClosedRequested
        {
            get => _connectionCloseRequestedSource.Token;
            set => throw new NotImplementedException();
        }

        void IConnectionLifetimeNotificationFeature.RequestClose()
        {
            _logger.LogDebug("TODO: CloseRequested {ConnectionId}", _connectionId);

            _connectionCloseRequestedSource.Cancel(throwOnFirstException: false);
        }
    }
}
