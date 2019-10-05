using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets.TestHelpers.Fixtures
{
    public class TimeoutFixture : IDisposable
    {
        private readonly Lazy<CancellationTokenSource> _cts;

        public TimeoutFixture()
        {
            _cts = new Lazy<CancellationTokenSource>(() => new CancellationTokenSource(Debugger.IsAttached ? TimeSpan.FromMinutes(5) : TimeSpan.FromSeconds(2.5)));
        }

        public CancellationToken Token => _cts.Value.Token;

        public void Dispose()
        {
        }
    }
}
