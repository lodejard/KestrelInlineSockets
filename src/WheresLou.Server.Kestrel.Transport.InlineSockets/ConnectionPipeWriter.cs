// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Internals;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets
{
    public class ConnectionPipeWriter : PipeWriter
    {
        private readonly ConnectionContext<ConnectionPipeWriter> _context;
        private readonly IConnection _connection;
        private readonly INetworkSocket _socket;
        private readonly CancellationTokenSource _readerCompleted;
        private readonly RollingMemory _buffer;
        private Exception _readerCompletedException;

        public ConnectionPipeWriter(
            ConnectionContext<ConnectionPipeWriter> context,
            IConnection connection,
            INetworkSocket socket)
        {
            _context = context;
            _connection = connection;
            _socket = socket;
            _readerCompleted = new CancellationTokenSource();
            _buffer = new RollingMemory(_context.Options.MemoryPool);
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
                    var bytes = _socket.Send(memory);
                    _buffer.ConsumeOccupiedMemory(bytes);
                }
            }
            catch (Exception ex)
            {
                // TODO: should this re-throw, or should it fall through to return IsCompleted true instead?
                FireReaderCompleted(ex);
                throw;
            }

            return new ValueTask<FlushResult>(new FlushResult(
                isCanceled: false,
                isCompleted: false));
        }

        public override void CancelPendingFlush()
        {
            throw new NotImplementedException();
        }

        public override void Complete(Exception exception = null)
        {
            _context.Logger.LogTrace(exception, "TODO: PipeWriterComplete");

            _connection.OnPipeWriterComplete(exception);
        }

        public override void OnReaderCompleted(Action<Exception, object> callback, object state)
        {
            _readerCompleted.Token.Register(() => callback(_readerCompletedException, state));
        }

        private void FireReaderCompleted(Exception exception)
        {
            _readerCompletedException = exception;
            _readerCompleted.Cancel();
        }
    }
}
