using System;
using System.IO.Pipelines;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets
{
    public class ConnectionPipeWriter : PipeWriter
    {
        private readonly ILogger<ConnectionPipeWriter> _logger;
        private readonly ConnectionContext _context;
        private readonly CancellationTokenSource _readerCompleted;
        private readonly RollingMemory _buffer;
        private Exception _readerCompletedException;


        public ConnectionPipeWriter(ILogger<ConnectionPipeWriter> logger, ConnectionContext context)
        {
            _logger = logger;
            _context = context;
            _readerCompleted = new CancellationTokenSource();
            _buffer = new RollingMemory(context.MemoryPool);
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
            while (!_buffer.Empty)
            {
                var memory = _buffer.GetOccupiedMemory();
                var bytes = _context.Socket.Send(memory);
                _buffer.ConsumeOccupiedMemory(bytes);
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
            // TODO: verify this is the correct order of operations
            _context.ConnectionClosed.Cancel();

            // TODO: is this complete the writer? or the "reader" feeding back from the socket send?
            //Interlocked.CompareExchange(ref _readerCompletedException, exception, null);
            //_readerCompleted.Cancel(throwOnFirstException: false);
        }

        public override void OnReaderCompleted(Action<Exception, object> callback, object state)
        {
            _readerCompleted.Token.Register(() => callback(_readerCompletedException, state));
        }
    }
}
