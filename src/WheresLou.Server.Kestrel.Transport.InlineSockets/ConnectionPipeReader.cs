// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO.Pipelines;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Internals;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets
{
    public class ConnectionPipeReader : PipeReader
    {
        private readonly ConnectionContext<ConnectionPipeReader> _context;
        private readonly IConnection _connection;
        private readonly INetworkSocket _socket;
        private readonly CancellationTokenSource _writerCompleted;
        private readonly RollingMemory _buffer;

        private bool _bufferHasUnexaminedData;
        private bool _isCanceled;
        private bool _isCompleted;
        private Exception _writerCompletedException;

        public ConnectionPipeReader(
            ConnectionContext<ConnectionPipeReader> context,
            IConnection connection,
            INetworkSocket socket)
        {
            _context = context;
            _connection = connection;
            _socket = socket;
            _writerCompleted = new CancellationTokenSource();
            _buffer = new RollingMemory(_context.Options.MemoryPool);
        }

        public bool IsCanceled => _isCanceled;

        public bool IsCompleted => _isCanceled || _isCompleted;

        public override bool TryRead(out ReadResult result)
        {
            throw new NotImplementedException();
        }

        public override async ValueTask<ReadResult> ReadAsync(CancellationToken cancellationToken)
        {
            // TODO: return unexamined memory immediately
            if (_bufferHasUnexaminedData)
            {
                return new ReadResult(
                    _buffer.GetOccupiedMemory(),
                    isCanceled: IsCanceled,
                    isCompleted: IsCompleted);
            }

            try
            {
                if (!IsCompleted)
                {
                    var memory = _buffer.GetTrailingMemory();

                    _context.Logger.LogTrace("TODO: ReadStarting");
                    var bytes = await _socket.ReceiveAsync(memory, cancellationToken);
                    _context.Logger.LogTrace("TODO: ReadComplete {bytes}", bytes);

                    if (bytes != 0)
                    {
                        var text = Encoding.UTF8.GetString(memory.Slice(0, bytes).ToArray());
                        _bufferHasUnexaminedData = true;
                        _buffer.TrailingMemoryFilled(bytes);
                    }
                    else
                    {
                        _isCompleted = true;
                    }
                }
            }
            catch (TaskCanceledException)
            {
                _context.Logger.LogTrace("TODO: ReadCanceled");
                _isCanceled = true;
            }
            catch (Exception ex)
            {
                _context.Logger.LogTrace("TODO: ReadFailed");
                FireWriterCompleted(ex);
            }

            return new ReadResult(
                _buffer.GetOccupiedMemory(),
                isCanceled: IsCanceled,
                isCompleted: IsCompleted);
        }

        public override void AdvanceTo(SequencePosition consumed)
        {
            AdvanceTo(consumed, consumed);
        }

        public override void AdvanceTo(SequencePosition consumed, SequencePosition examined)
        {
            _buffer.ConsumeOccupiedMemory(consumed);

            _bufferHasUnexaminedData = _buffer.HasUnexaminedData(examined);
        }

        public override void CancelPendingRead()
        {
            _context.Logger.LogTrace("TODO: CancelPendingRead");

            _socket.CancelPendingRead();
        }

        public override void Complete(Exception exception)
        {
            _context.Logger.LogTrace(exception, "TODO: PipeReaderComplete");

            _isCompleted = true;
            _connection.OnPipeReaderComplete(exception);
        }

        public override void OnWriterCompleted(Action<Exception, object> callback, object state)
        {
            _writerCompleted.Token.Register(() => callback(_writerCompletedException, state));
        }

        public void FireWriterCompleted(Exception exception)
        {
            _writerCompletedException = exception;
            _writerCompleted.Cancel();
        }
    }
}
