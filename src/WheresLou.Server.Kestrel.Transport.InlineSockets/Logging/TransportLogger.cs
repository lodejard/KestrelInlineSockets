// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Net;
using Microsoft.Extensions.Logging;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets.Logging
{
    public class TransportLogger : ForwardingLogger, ITransportLogger
    {
        private static readonly LoggerMessage<IPEndPoint> _logBindListenSocket = (LogLevel.Debug, 1, "BindListenSocket", "Binding listen socket to {IPEndPoint}");
        private static readonly LoggerMessage<IPEndPoint> _logUnbindListenSocket = (LogLevel.Debug, 2, "UnbindListenSocket", "Unbinding listen socket from {IPEndPoint}");
        private static readonly LoggerMessage _logStopTransport = (LogLevel.Debug, 3, "StopTransport", "Inline sockets transport is stopped");
        private static readonly LoggerMessage<EndPoint, EndPoint> _logSocketAccepted = (LogLevel.Information, 4, "SocketAccepted", "Socket accepted from {RemoteEndPoint} to {LocalEndPoint}");
        private static readonly LoggerMessage _logConnectionDispatchFailed = (LogLevel.Debug, 5, "ConnectionDispatchFailed", "Unexpected failure thrown from IConnectionDispatcher.OnConnection");

        public TransportLogger(ILoggerFactory loggerFactory)
            : base(loggerFactory, typeof(Transport).FullName)
        {
        }

        public virtual void BindListenSocket(IPEndPoint ipEndPoint) => _logBindListenSocket.Log(this, ipEndPoint, null);

        public virtual void UnbindListenSocket(IPEndPoint ipEndPoint) => _logUnbindListenSocket.Log(this, ipEndPoint, null);

        public virtual void StopTransport() => _logStopTransport.Log(this, null);

        public virtual void SocketAccepted(EndPoint remoteEndPoint, EndPoint localEndPoint) => _logSocketAccepted.Log(this, remoteEndPoint, localEndPoint, null);

        public virtual void ConnectionDispatchFailed(Exception error) => _logConnectionDispatchFailed.Log(this, error);
    }
}
