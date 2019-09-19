using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets
{

    public class Transport : ITransport
    {
        private readonly ILogger<Transport> _logger;
        private readonly IConnectionFactory _connectionFactory;
        private readonly TransportContext _context;

        private TcpListener _listener;
        private CancellationTokenSource _acceptLoopTokenSource;
        private Task _acceptLoopTask;

        public Transport(
            ILogger<Transport> logger,
            IConnectionFactory connectionFactory,
            TransportContext context)
        {
            _logger = logger;
            _connectionFactory = connectionFactory;
            _context = context;
        }

        public Task BindAsync()
        {
            _logger.LogDebug(new EventId(1, "BindListenSocket"), "Binding listen socket to {IPEndPoint}", _context.EndPointInformation.IPEndPoint);

            // TODO: logic to bind ipv4 and/or ipv6 ?
            _listener = new TcpListener(_context.EndPointInformation.IPEndPoint);

            // TODO: call _listener.AllowNatTraversal ?
            // TODO: set _listener.ExclusiveAddressUse ?

            // TODO: better listen backlog value
            try
            {
                _listener.Start();
            }
            catch (Exception ex)
            {
                throw;
            }

            _acceptLoopTokenSource = new CancellationTokenSource();
            _acceptLoopTask = Task.Run(() => AcceptLoopAsync(_listener, _acceptLoopTokenSource.Token));

            return Task.CompletedTask;
        }

        public async Task UnbindAsync()
        {
            _listener.Stop();
            await _acceptLoopTask;
        }

        public Task StopAsync()
        {
            return Task.CompletedTask;
        }

        public async Task AcceptLoopAsync(TcpListener listener, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var socket = await listener.AcceptSocketAsync();

                    _logger.LogInformation(new EventId(5, "SocketAccepted"), "Socket accepted from {RemoteEndPoint} to {LocalEndPoint}", socket.RemoteEndPoint, socket.LocalEndPoint);

                    HandleErrors(Task.Run(async () =>
                    {
                        var connection = _connectionFactory.Create(new ConnectionContext(_context.MemoryPool, socket));
                        await _context.ConnectionDispatcher.OnConnection(connection);
                    }));
                }
                catch (SocketException ex) when (ex.SocketErrorCode == SocketError.OperationAborted)
                {
                    // only way to exit loop?
                    return;
                }
            }
        }

        public async void HandleErrors(Task task)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                _logger.LogError(new EventId(6, "OnConnectionError"), ex, "Unexpected failure thrown from IConnectionDispatcher.OnConnection");
            }
        }
    }
}
