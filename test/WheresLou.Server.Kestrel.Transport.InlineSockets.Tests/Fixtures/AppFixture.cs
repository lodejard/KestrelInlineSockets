using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.Features;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets.Tests.Fixtures
{
    public class AppFixture : IHttpApplication<IFeatureCollection>, IDisposable
    {
        public IFeatureCollection CreateContext(IFeatureCollection contextFeatures)
        {
            return contextFeatures;
        }

        public void DisposeContext(IFeatureCollection context, Exception exception)
        {

        }

        public async Task ProcessRequestAsync(IFeatureCollection context)
        {

        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
