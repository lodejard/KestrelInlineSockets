// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

#if NETCOREAPP3_0
using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Http.Features;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket
{
    public class ConnectionContext : Microsoft.AspNetCore.Connections.ConnectionContext
    {
        private readonly IConnection _connection;
        private FeatureReference<IConnectionIdFeature> _connectionIdFeature;
        private FeatureReference<IHttpConnectionFeature> _httpConnectionFeature;
        private FeatureReference<IConnectionItemsFeature> _connectionItemsFeature;
        private FeatureReference<IConnectionTransportFeature> _connectionTransportFeature;
        private FeatureReference<IConnectionLifetimeFeature> _connectionLifetimeFeature;

        public ConnectionContext(IConnection connection)
        {
            _connection = connection;
        }

        public override IFeatureCollection Features => _connection.Features;

        public override string ConnectionId
        {
            get => _connectionIdFeature.Fetch(Features).ConnectionId;
            set => _connectionIdFeature.Fetch(Features).ConnectionId = value;
        }

        public override IDictionary<object, object> Items
        {
            get => _connectionItemsFeature.Fetch(Features).Items;
            set => _connectionItemsFeature.Fetch(Features).Items = value;
        }

        public override IDuplexPipe Transport
        {
            get => _connectionTransportFeature.Fetch(Features).Transport;
            set => _connectionTransportFeature.Fetch(Features).Transport = value;
        }

        public override CancellationToken ConnectionClosed
        {
            get => _connectionLifetimeFeature.Fetch(Features).ConnectionClosed;
            set => _connectionLifetimeFeature.Fetch(Features).ConnectionClosed = value;
        }

        public override EndPoint LocalEndPoint
        {
            get
            {
                var feature = _httpConnectionFeature.Fetch(Features);
                return new IPEndPoint(feature.LocalIpAddress, feature.LocalPort);
            }

            set
            {
                var feature = _httpConnectionFeature.Fetch(Features);
                feature.LocalIpAddress = ((IPEndPoint)value).Address;
                feature.LocalPort = ((IPEndPoint)value).Port;
            }
        }

        public override EndPoint RemoteEndPoint
        {
            get
            {
                var feature = _httpConnectionFeature.Fetch(Features);
                return new IPEndPoint(feature.RemoteIpAddress, feature.RemotePort);
            }

            set
            {
                var feature = _httpConnectionFeature.Fetch(Features);
                feature.RemoteIpAddress = ((IPEndPoint)value).Address;
                feature.RemotePort = ((IPEndPoint)value).Port;
            }
        }

        public override void Abort()
        {
            _connectionLifetimeFeature.Fetch(Features).Abort();
        }

        public override void Abort(ConnectionAbortedException abortReason)
        {
            _connection.Abort(abortReason);
        }

        public override async ValueTask DisposeAsync()
        {
            await _connection.DisposeAsync();
        }
    }
}
#endif
