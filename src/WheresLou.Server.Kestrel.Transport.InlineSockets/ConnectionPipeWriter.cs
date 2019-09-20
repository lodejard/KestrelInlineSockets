using System;
using System.IO.Pipelines;
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
        private Exception _readerCompletedException;

        public ConnectionPipeWriter(ILogger<ConnectionPipeWriter> logger, ConnectionContext context)
        {
            _logger = logger;
            _context = context;
            _readerCompleted = new CancellationTokenSource();
        }

        public override Memory<byte> GetMemory(int sizeHint = 0)
        {
            throw new NotImplementedException();
        }

        public override Span<byte> GetSpan(int sizeHint = 0)
        {
            throw new NotImplementedException();
        }

        public override void Advance(int bytes)
        {
            throw new NotImplementedException();
        }

        public override ValueTask<FlushResult> FlushAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override void CancelPendingFlush()
        {
            throw new NotImplementedException();
        }

        public override void Complete(Exception exception = null)
        {
            Interlocked.CompareExchange(ref _readerCompletedException, exception, null);
            _readerCompleted.Cancel(throwOnFirstException: false);
        }

        public override void OnReaderCompleted(Action<Exception, object> callback, object state)
        {
            _readerCompleted.Token.Register(() => callback(_readerCompletedException, state));
        }
    }
}
