// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bing.AspNetCore.Connections.InlineSocket.Logging;
using Microsoft.Bing.AspNetCore.Connections.InlineSocket.Memory;
using Microsoft.Bing.AspNetCore.Connections.InlineSocket.Network;
using Microsoft.Extensions.Logging;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket
{
    public class ConnectionPipeWriter : PipeWriter, IDisposable
    {
        private readonly IConnectionLogger _logger;
        private readonly IConnection _connection;
        private readonly INetworkSocket _socket;
        private readonly CancellationTokenSource _readerCompleted;
        private readonly RollingMemory _buffer;

        private bool _isCanceled;
        private bool _isCompleted;
#if NETSTANDARD2_0
        private Exception _readerCompletedException;
#endif

        public ConnectionPipeWriter(
            IConnectionLogger logger,
            InlineSocketsOptions options,
            IConnection connection,
            INetworkSocket socket)
        {
            _logger = logger;
            _connection = connection;
            _socket = socket;
            _readerCompleted = new CancellationTokenSource();
            _buffer = new RollingMemory(options.MemoryPool);
        }

        public bool IsCanceled => _isCanceled;

        public bool IsCompleted => _isCanceled || _isCompleted;

        public void Dispose()
        {
            _buffer.Dispose();
        }

        public override Memory<byte> GetMemory(int sizeHint)
        {
            return _buffer.GetTrailingMemory(sizeHint);
        }

        public override Span<byte> GetSpan(int sizeHint)
        {
            return _buffer.GetTrailingMemory(sizeHint).Span;
        }

        public override void Advance(int bytes)
        {
            _buffer.ConsumeTrailingMemory(bytes);
        }

        public override ValueTask<FlushResult> FlushAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!_buffer.IsEmpty)
                {
                    var memory = _buffer.GetOccupiedMemory();

                    _logger.LogTrace("TODO: SendStarting");
                    var bytes = _socket.Send(memory);
                    _logger.LogTrace("TODO: SendComplete");

                    _buffer.ConsumeOccupiedMemory(bytes);
                }
            }
            catch (Exception ex)
            {
                // Return FlushResult.IsCompleted == true from now on
                // because we assume any write exceptions are not temporary
                _isCompleted = true;
                FireReaderCompleted(ex);
            }

            return new ValueTask<FlushResult>(new FlushResult(
                isCanceled: IsCanceled,
                isCompleted: IsCompleted));
        }

        public override void CancelPendingFlush()
        {
            _isCanceled = true;
        }

        public override void Complete(Exception exception = null)
        {
            _logger.LogTrace(exception, "TODO: PipeWriterComplete");

            _isCompleted = true;
            _connection.OnPipeWriterComplete(exception);
        }

#if NETSTANDARD2_0
        public override void OnReaderCompleted(Action<Exception, object> callback, object state)
        {
            _readerCompleted.Token.Register(() => callback(_readerCompletedException, state));
        }

        private void FireReaderCompleted(Exception exception)
        {
            _readerCompletedException = exception;
            _readerCompleted.Cancel();
        }
#else
        private void FireReaderCompleted(Exception exception)
        {
        }
#endif
    }
}
