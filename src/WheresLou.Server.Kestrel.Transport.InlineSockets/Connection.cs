using System;
using System.Buffers;
using System.Collections;
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
using WheresLou.Server.Kestrel.Transport.InlineSockets.Factories;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Internals;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets
{
    public partial class Connection : TransportConnection, IConnection, IDuplexPipe, IHttpConnectionFeature, IConnectionIdFeature, IConnectionTransportFeature, IConnectionItemsFeature, IMemoryPoolFeature, IApplicationTransportFeature, ITransportSchedulerFeature, IConnectionLifetimeFeature, IConnectionHeartbeatFeature, IConnectionLifetimeNotificationFeature //, IFeatureCollection, IEnumerable<KeyValuePair<Type, object>>, IEnumerable
    {
        private readonly ILogger<Connection> _logger;
        private readonly INetworkProvider _networkProvider;
        private readonly ConnectionContext _context;
        private readonly PipeReader _connectionPipeReader;
        private readonly PipeWriter _connectionPipeWriter;
        private readonly CancellationTokenSource _connectionCloseRequestedTokenSource;

        private string _connectionId;
        private IDuplexPipe _applicationDuplexPipe;
        private IDuplexPipe _transportDuplexPipe;

        public Connection(
            ILogger<Connection> logger,
            INetworkProvider networkProvider,
            IConnectionPipeReaderFactory connectionPipeReaderFactory,
            IConnectionPipeWriterFactory connectionPipeWriterFactory,
            ConnectionContext context)
        {
            _connectionCloseRequestedTokenSource = new CancellationTokenSource();
            _logger = logger;
            _networkProvider = networkProvider;
            _context = context;
            _connectionPipeReader = connectionPipeReaderFactory.Create(context);
            _connectionPipeWriter = connectionPipeWriterFactory.Create(context);

            // this mechanism propogates a server-wide request for graceful shutdown. it is received by the http1/2 framing layer.
            base.ConnectionClosedRequested.Register(OnConnectionCloseRequested);
            base.ConnectionClosedRequested = _connectionCloseRequestedTokenSource.Token;
            _connectionCloseRequestedTokenSource.Token.Register(OnCloseRequested);

            // this mechanism triggers when the connection tranceiving is entirely complete. used for cleanup. associated with abort.
            base.ConnectionClosed = _context.ConnectionClosed.Token;

            void OnConnectionCloseRequested()
            {
                _connectionCloseRequestedTokenSource.Cancel();
            }
        }

        private void OnCloseRequested()
        {
            //throw new NotImplementedException();
        }

        private void OnAbortRequested(ConnectionAbortedException abortReason)
        {
            //throw new NotImplementedException();
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
            get => ((IPEndPoint)_context.Socket.RemoteEndPoint).Address;
            set => throw new NotImplementedException();
        }

        IPAddress IHttpConnectionFeature.LocalIpAddress
        {
            get => ((IPEndPoint)_context.Socket.LocalEndPoint).Address;
            set => throw new NotImplementedException();
        }

        int IHttpConnectionFeature.RemotePort
        {
            get => ((IPEndPoint)_context.Socket.RemoteEndPoint).Port;
            set => throw new NotImplementedException();
        }

        int IHttpConnectionFeature.LocalPort
        {
            get => ((IPEndPoint)_context.Socket.LocalEndPoint).Port;
            set => throw new NotImplementedException();
        }

        IDuplexPipe IConnectionTransportFeature.Transport
        {
            get => this;
            set => _transportDuplexPipe = value;
        }

        IDictionary<object, object> IConnectionItemsFeature.Items
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        MemoryPool<byte> IMemoryPoolFeature.MemoryPool => _context.MemoryPool;

        IDuplexPipe IApplicationTransportFeature.Application
        {
            get => _applicationDuplexPipe;
            set => _applicationDuplexPipe = value;
        }

        PipeScheduler ITransportSchedulerFeature.InputWriterScheduler => PipeScheduler.Inline;

        PipeScheduler ITransportSchedulerFeature.OutputReaderScheduler => PipeScheduler.Inline;

        CancellationToken IConnectionLifetimeFeature.ConnectionClosed
        {
            get => _context.ConnectionClosed.Token;
            set => throw new NotImplementedException();
        }

        CancellationToken IConnectionLifetimeNotificationFeature.ConnectionClosedRequested
        {
            get => _connectionCloseRequestedTokenSource.Token;
            set => throw new NotImplementedException();
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
            base.OnHeartbeat(action, state);
        }

        void IConnectionLifetimeNotificationFeature.RequestClose()
        {
            // signal that a close has been requested
            _connectionCloseRequestedTokenSource.Cancel();
        }

        Task IConnection.TranceiveAsync()
        {
            throw new NotImplementedException();
        }
    }
}
