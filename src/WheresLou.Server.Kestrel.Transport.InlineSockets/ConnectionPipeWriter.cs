using System;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets
{
    public class ConnectionPipeWriter : PipeWriter
    {
        private ILogger<ConnectionPipeWriter> _logger;
        private ConnectionContext _context;

        public ConnectionPipeWriter(ILogger<ConnectionPipeWriter> logger, ConnectionContext context)
        {
            _logger = logger;
            _context = context;
        }

        public override void Advance(int bytes)
        {
            throw new NotImplementedException();
        }

        public override void CancelPendingFlush()
        {
            throw new NotImplementedException();
        }

        public override void Complete(Exception exception = null)
        {
            throw new NotImplementedException();
        }

        public override ValueTask<FlushResult> FlushAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override Memory<byte> GetMemory(int sizeHint = 0)
        {
            throw new NotImplementedException();
        }

        public override Span<byte> GetSpan(int sizeHint = 0)
        {
            throw new NotImplementedException();
        }

        public override void OnReaderCompleted(Action<Exception, object> callback, object state)
        {
            throw new NotImplementedException();
        }
    }
}
