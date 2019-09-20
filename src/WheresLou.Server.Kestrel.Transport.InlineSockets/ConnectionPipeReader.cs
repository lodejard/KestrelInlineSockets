using System;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets
{
    public class ConnectionPipeReader : PipeReader
    {
        private readonly ILogger<ConnectionPipeReader> _logger;
        private readonly ConnectionContext _context;
        private readonly CancellationTokenSource _writerCompleted;
        private Exception _writerCompletedException;

        public ConnectionPipeReader(ILogger<ConnectionPipeReader> logger, ConnectionContext context)
        {
            _logger = logger;
            _context = context;
            _writerCompleted = new CancellationTokenSource();
        }

        public override bool TryRead(out ReadResult result)
        {
            throw new NotImplementedException();
        }

        public override ValueTask<ReadResult> ReadAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override void AdvanceTo(SequencePosition consumed)
        {
            throw new NotImplementedException();
        }

        public override void AdvanceTo(SequencePosition consumed, SequencePosition examined)
        {
            throw new NotImplementedException();
        }

        public override void CancelPendingRead()
        {
            throw new NotImplementedException();
        }

        public override void Complete(Exception exception = null)
        {
            Interlocked.CompareExchange(ref _writerCompletedException, exception, null);
            _writerCompleted.Cancel(throwOnFirstException: false);
        }

        public override void OnWriterCompleted(Action<Exception, object> callback, object state)
        {
            _writerCompleted.Token.Register(() => callback(_writerCompletedException, state));
        }
    }
}
