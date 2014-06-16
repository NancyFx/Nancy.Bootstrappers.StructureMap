using Nancy.Routing;

namespace Nancy.Bootstrappers.StructureMap.Tests
{
    public class FakeNancyModuleWithRouteCacheProviderDependency : NancyModule
    {
        public IRouteCacheProvider RouteCacheProvider { get; private set; }

        public FakeNancyModuleWithRouteCacheProviderDependency(IRouteCacheProvider routeCacheProvider)
        {
            RouteCacheProvider = routeCacheProvider;
        }
    }
}