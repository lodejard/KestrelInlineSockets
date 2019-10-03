// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.Extensions.Logging;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets.Logging
{
    public class ConnectionLogger : ForwardingLogger, IConnectionLogger
    {
        public ConnectionLogger(ILoggerFactory loggerFactory)
            : base(loggerFactory, typeof(Connection).FullName)
        {
        }
    }
}
