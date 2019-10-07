// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

#if NETSTANDARD2_0
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets
{
    public interface IAsyncDisposable
    {
        ValueTask DisposeAsync();
    }
}
#endif
