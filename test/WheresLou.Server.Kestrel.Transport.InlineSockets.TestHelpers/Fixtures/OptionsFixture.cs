using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Tests.Fixtures;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets.TestHelpers.Fixtures
{
    public class OptionsFixture
    {
        private readonly ServicesFixture _services;

        public OptionsFixture(ServicesFixture services)
        {
            _services = services;
        }

        public InlineSocketsOptions InlineSocketsOptions => _services.GetService<IOptions<InlineSocketsOptions>>().Value;
    }
}
