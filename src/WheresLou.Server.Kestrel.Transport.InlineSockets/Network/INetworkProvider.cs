// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace WheresLou.Server.Kestrel.Transport.InlineSockets.Network
{
    public interface INetworkProvider
    {
        INetworkListener CreateListener(NetworkListenerSettings settings);
    }
}
