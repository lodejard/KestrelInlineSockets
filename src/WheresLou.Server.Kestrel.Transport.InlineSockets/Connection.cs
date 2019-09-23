// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;
using Microsoft.Extensions.Logging;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Internals;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets
{
    public partial class Connection : TransportConnection, IConnection, IDuplexPipe, IHttpConnectionFeature, IConnectionIdFeature, IConnectionTransportFeature, IMemoryPoolFeature, IApplicationTransportFeature, ITransportSchedulerFeature, IConnectionLifetimeFeature, IConnectionHeartbeatFeature, IConnectionLifetimeNotificationFeature
    {
        private readonly ConnectionContext<Connection> _context;
        private readonly INetworkSocket _socket;
        private readonly CancellationTokenSource _connectionCloseRequestedTokenSource;
        private readonly CancellationTokenSource _connectionClosedTokenSource;
        private readonly PipeReader _connectionPipeReader;
        private readonly PipeWriter _connectionPipeWriter;
        private readonly EndPoint _socketRemoteEndPoint;
        private readonly EndPoint _socketLocalEndPoint;
        private string _connectionId;
        private IDuplexPipe _applicationDuplexPipe;
#pragma warning disable IDE0052 // Remove unread private members
        private IDuplexPipe _transportDuplexPipe;
#pragma warning restore IDE0052 // Remove unread private members

        public Connection(
            ConnectionContext<Connection> context,
            IConnectionFactory connectionFactory,
            INetworkSocket socket)
        {
            _context = context;
            _socket = socket;
            _connectionCloseRequestedTokenSource = new CancellationTokenSource();
            _connectionClosedTokenSource = new CancellationTokenSource();
            _connectionPipeReader = connectionFactory.CreatePipeReader(this, socket);
            _connectionPipeWriter = connectionFactory.CreatePipeWriter(this, socket);

            // preserving these values to avoid errors once the socket is closed
            _socketRemoteEndPoint = _socket.RemoteEndPoint;
            _socketLocalEndPoint = _socket.LocalEndPoint;

            // this mechanism propogates a server-wide request for graceful shutdown. it is received by the http1/2 framing layer.
            ConnectionClosedRequested.Register(OnCloseRequested);
            ConnectionClosedRequested = _connectionCloseRequestedTokenSource.Token;

            // this mechanism triggers when the connection tranceiving is entirely complete. used for cleanup. associated with abort.
            ConnectionClosed = _connectionClosedTokenSource.Token;
        }

        public override IFeatureCollection Features => this;

        public override MemoryPool<byte> MemoryPool => ((IMemoryPoolFeature)this).MemoryPool;

        public override PipeScheduler InputWriterScheduler => ((ITransportSchedulerFeature)this).InputWriterScheduler;

        public override PipeScheduler OutputReaderScheduler => ((ITransportSchedulerFeature)this).OutputReaderScheduler;

        public override IDuplexPipe Transport
        {
            get => ((IConnectionTransportFeature)this).Transport;
            set => ((IConnectionTransportFeature)this).Transport = value;
        }

        PipeReader IDuplexPipe.Input => _connectionPipeReader;

        PipeWriter IDuplexPipe.Output => _connectionPipeWriter;

        public override IDictionary<object, object> Items
        {
            get => ((IConnectionItemsFeature)this).Items;
            set => ((IConnectionItemsFeature)this).Items = value;
        }

        public override string ConnectionId
        {
            get => ((IConnectionIdFeature)this).ConnectionId;
            set => ((IConnectionIdFeature)this).ConnectionId = value;
        }

        TransportConnection IConnection.TransportConnection => this;

        string IConnectionIdFeature.ConnectionId
        {
            get => _connectionId;
            set => _connectionId = value;
        }

        string IHttpConnectionFeature.ConnectionId
        {
            get => _connectionId;
            set => _connectionId = value;
        }

        IPAddress IHttpConnectionFeature.RemoteIpAddress
        {
            get => ((IPEndPoint)_socketRemoteEndPoint).Address;
            set => throw new NotImplementedException();
        }

        IPAddress IHttpConnectionFeature.LocalIpAddress
        {
            get => ((IPEndPoint)_socketLocalEndPoint).Address;
            set => throw new NotImplementedException();
        }

        int IHttpConnectionFeature.RemotePort
        {
            get => ((IPEndPoint)_socketRemoteEndPoint).Port;
            set => throw new NotImplementedException();
        }

        int IHttpConnectionFeature.LocalPort
        {
            get => ((IPEndPoint)_socketLocalEndPoint).Port;
            set => throw new NotImplementedException();
        }

        IDuplexPipe IConnectionTransportFeature.Transport
        {
            get => this;
            set => _transportDuplexPipe = value;
        }

        MemoryPool<byte> IMemoryPoolFeature.MemoryPool => _context.Options.MemoryPool;

        IDuplexPipe IApplicationTransportFeature.Application
        {
            get => _applicationDuplexPipe;
            set => _applicationDuplexPipe = value;
        }

        PipeScheduler ITransportSchedulerFeature.InputWriterScheduler => PipeScheduler.Inline;

        PipeScheduler ITransportSchedulerFeature.OutputReaderScheduler => PipeScheduler.Inline;

        CancellationToken IConnectionLifetimeFeature.ConnectionClosed
        {
            get => _connectionClosedTokenSource.Token;
            set => throw new NotImplementedException();
        }

        CancellationToken IConnectionLifetimeNotificationFeature.ConnectionClosedRequested
        {
            get => _connectionCloseRequestedTokenSource.Token;
            set => throw new NotImplementedException();
        }

        void IDisposable.Dispose()
        {
            _socket.Dispose();
            _connectionCloseRequestedTokenSource.Dispose();
            _connectionClosedTokenSource.Dispose();
        }

        public void OnCloseRequested()
        {
            _context.Logger.LogDebug("TODO: CloseRequested");

            // signal close has been requested
            _connectionCloseRequestedTokenSource.Cancel(throwOnFirstException: false);
        }

        public void OnAbortRequested(ConnectionAbortedException abortReason)
        {
            _context.Logger.LogDebug("TODO: AbortRequested");
        }

        public override void Abort()
        {
            OnAbortRequested(null);
        }

        public override void Abort(ConnectionAbortedException abortReason)
        {
            OnAbortRequested(abortReason);
        }

        void IConnectionLifetimeFeature.Abort()
        {
            OnAbortRequested(null);
        }

        void IConnectionHeartbeatFeature.OnHeartbeat(Action<object> action, object state)
        {
            // this method must be delegated to the base implementation because private fields and
            // non-virtual public methods prevent the implementation from being controlled
            OnHeartbeat(action, state);
        }

        void IConnectionLifetimeNotificationFeature.RequestClose()
        {
            OnCloseRequested();
        }

        Task IConnection.TranceiveAsync()
        {
            // TODO: doesn't appear that the connection itself needs to do any connection-long work
            return Task.CompletedTask;
        }

        void IConnection.OnPipeReaderComplete(Exception exception)
        {
            // TODO: does this call also need to happen before the connection is clear to dispose?
        }

        void IConnection.OnPipeWriterComplete(Exception exception)
        {
            // signal that the connection is clear to be disposed
            _connectionClosedTokenSource.Cancel(throwOnFirstException: false);
        }
    }
}
