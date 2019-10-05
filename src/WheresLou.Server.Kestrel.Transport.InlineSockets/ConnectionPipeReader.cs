// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO.Pipelines;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Logging;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Memory;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Network;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets
{
    public class ConnectionPipeReader : PipeReader
    {
        private readonly IConnectionLogger _logger;
        private readonly IConnection _connection;
        private readonly INetworkSocket _socket;
        private readonly RollingMemory _buffer;
        private readonly CancellationTokenSource _writerCompleted;

        private bool _bufferHasUnexaminedData;
        private bool _isCanceled;
        private bool _isCompleted;
        private Exception _writerCompletedException;

        public ConnectionPipeReader(
            IConnectionLogger logger,
            InlineSocketsOptions options,
            IConnection connection,
            INetworkSocket socket)
        {
            _logger = logger;
            _connection = connection;
            _socket = socket;
            _buffer = new RollingMemory(options.MemoryPool);
            _writerCompleted = new CancellationTokenSource();
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
                    // TODO: better size hint?
                    var memory = _buffer.GetTrailingMemory();

                    _logger.LogTrace("TODO: ReadStarting");
                    var bytes = await _socket.ReceiveAsync(memory, cancellationToken);
                    _logger.LogTrace("TODO: ReadComplete {bytes}", bytes);

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
                _logger.LogTrace("TODO: ReadCanceled");
                _isCanceled = true;
            }
            catch (Exception ex)
            {
                _logger.LogTrace("TODO: ReadFailed");

                // Return ReadResult.IsCompleted == true from now on
                // because we assume any read exceptions are not temporary
                _isCompleted = true;
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
            _logger.LogTrace("TODO: CancelPendingRead");

            _socket.CancelPendingRead();
        }

        public override void Complete(Exception exception)
        {
            _logger.LogTrace(exception, "TODO: PipeReaderComplete");

            _isCompleted = true;
            _connection.OnPipeReaderComplete(exception);
        }

#if NETSTANDARD2_0
        public override void OnWriterCompleted(Action<Exception, object> callback, object state)
        {
            _writerCompleted.Token.Register(() => callback(_writerCompletedException, state));
        }

        public void FireWriterCompleted(Exception exception)
        {
            _writerCompletedException = exception;
            _writerCompleted.Cancel();
        }
#else
        public void FireWriterCompleted(Exception exception)
        {
        }
#endif
    }
}
