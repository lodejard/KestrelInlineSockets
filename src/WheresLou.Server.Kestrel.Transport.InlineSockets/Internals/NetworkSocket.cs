using System;
using System.Buffers;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets.Internals
{
    public class NetworkSocket : INetworkSocket
    {
        private readonly Socket _socket;

        private readonly object _receiveSync = new object();
        private readonly SocketAsyncEventArgs _receiveEventArgs;
        private TaskCompletionSource<int> _receiveAsyncTaskSource;
        private TaskCompletionSource<int> _receiveAsyncTaskSourceCache;

        public NetworkSocket(Socket socket)
        {
            _socket = socket;
            _receiveEventArgs = new SocketAsyncEventArgs();
            _receiveEventArgs.Completed += ReceiveAsyncCompleted;
        }

        public virtual EndPoint LocalEndPoint => _socket.LocalEndPoint;

        public virtual EndPoint RemoteEndPoint => _socket.RemoteEndPoint;

        public virtual void Dispose()
        {
            _receiveEventArgs.Dispose();
            _socket.Dispose();
        }

        public virtual Task<int> ReceiveAsync(Memory<byte> memory, CancellationToken cancellationToken)
        {
            if (!MemoryMarshal.TryGetArray((ReadOnlyMemory<byte>)memory, out var segment))
            {
                throw new ArgumentException("Memory<byte> must be backed by an array", nameof(memory));
            }

            lock (_receiveSync)
            {
                _receiveEventArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);

                if (_receiveAsyncTaskSource != null)
                {
                    throw new InvalidOperationException("Concurrent calls to ReceiveAsync are not allowed");
                }

                _receiveAsyncTaskSource = _receiveAsyncTaskSourceCache ?? new TaskCompletionSource<int>();
                _receiveAsyncTaskSourceCache = null;

                try
                {
                    var receiveAsyncIsPending = _socket.ReceiveAsync(_receiveEventArgs);
                    if (receiveAsyncIsPending)
                    {
                        return _receiveAsyncTaskSource.Task;
                    }
                }
                catch
                {
                    _receiveAsyncTaskSourceCache = _receiveAsyncTaskSource;
                    _receiveAsyncTaskSource = null;
                    throw;
                }

                _receiveAsyncTaskSourceCache = _receiveAsyncTaskSource;
                _receiveAsyncTaskSource = null;

                if (_receiveEventArgs.SocketError != SocketError.Success)
                {
                    ThrowSocketException(_receiveEventArgs.SocketError);

                    void ThrowSocketException(SocketError e)
                    {
                        throw new SocketException((int)e);
                    }
                }

                return Task.FromResult(_receiveEventArgs.BytesTransferred);
            }
        }

        private void ReceiveAsyncCompleted(object sender, SocketAsyncEventArgs e)
        {
            TaskCompletionSource<int> receiveAsyncTaskSource;
            SocketError socketError;
            int bytesTransferred;
            lock (_receiveSync)
            {
                receiveAsyncTaskSource = _receiveAsyncTaskSource;
                socketError = _receiveEventArgs.SocketError;
                bytesTransferred = _receiveEventArgs.BytesTransferred;

                _receiveAsyncTaskSource = null;
            }

            // TODO: more robust guard against reading from cancelled socket?
            if (socketError != SocketError.Success)
            {
                receiveAsyncTaskSource?.TrySetException(new SocketException((int)socketError));
            }
            else
            {
                receiveAsyncTaskSource?.TrySetResult(bytesTransferred);
            }
        }

        public virtual void CancelPendingRead()
        {
            TaskCompletionSource<int> receiveAsyncTaskSource;

            lock (_receiveSync)
            {
                receiveAsyncTaskSource = _receiveAsyncTaskSource;
                _receiveAsyncTaskSource = null;
            }

            // TODO: more robust guard against reading from cancelled socket?
            receiveAsyncTaskSource?.TrySetCanceled();
        }

        public virtual int Send(ReadOnlySequence<byte> data)
        {
            // TODO: avoid allocating this List<T>
            var segments = new List<ArraySegment<byte>>();
            foreach (var buffer in data)
            {
                MemoryMarshal.TryGetArray(buffer, out var segment);
                segments.Add(segment);
            }
            return _socket.Send(segments, SocketFlags.None);
        }
    }
}
