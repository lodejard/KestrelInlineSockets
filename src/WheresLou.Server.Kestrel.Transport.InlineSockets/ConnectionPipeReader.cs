using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Runtime.InteropServices;
using System.Text;
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
        private readonly RollingMemory _buffer;

        private Exception _writerCompletedException;

        private bool _bufferHasUnexaminedData;
        private bool _isCanceled;

        public bool IsCanceled => _isCanceled;
        public bool IsCompleted => _isCanceled || _writerCompleted.IsCancellationRequested;

        public ConnectionPipeReader(ILogger<ConnectionPipeReader> logger, ConnectionContext context)
        {
            _logger = logger;
            _context = context;
            _writerCompleted = new CancellationTokenSource();
            _buffer = new RollingMemory(_context.MemoryPool);
        }

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
                    var bytes = await _context.Socket.ReceiveAsync(memory, cancellationToken);
                    if (bytes != 0)
                    {
                        var text = Encoding.UTF8.GetString(memory.Slice(0, bytes).ToArray());
                        _bufferHasUnexaminedData = true;
                        _buffer.TrailingMemoryFilled(bytes);
                    }
                }
                else
                {
                    var x = 5;
                }
            }
            catch (TaskCanceledException)
            {
                _isCanceled = true;
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
            _context.Socket.CancelPendingRead();
        }

        public override void Complete(Exception exception)
        {
            // TODO: the semantics of this seem to be that the **reader** (the http server) is complete --- it does not indicate that a fin was received from the client
            //Interlocked.CompareExchange(ref _writerCompletedException, exception, null);
            //_writerCompleted.Cancel(throwOnFirstException: false);
        }

        public override void OnWriterCompleted(Action<Exception, object> callback, object state)
        {
            _writerCompleted.Token.Register(() => callback(_writerCompletedException, state));
        }
    }
}
