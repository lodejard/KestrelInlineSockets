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

        public ConnectionPipeReader(ILogger<ConnectionPipeReader> logger, ConnectionContext context)
        {
            _logger = logger;
            _context = context;
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
            throw new NotImplementedException();
        }

        public override void OnWriterCompleted(Action<Exception, object> callback, object state)
        {
            throw new NotImplementedException();
        }

        public override ValueTask<ReadResult> ReadAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override bool TryRead(out ReadResult result)
        {
            throw new NotImplementedException();
        }
    }
}
