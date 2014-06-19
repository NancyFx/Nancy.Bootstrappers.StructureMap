namespace Nancy.Bootstrappers.StructureMap.Tests
{
    using Nancy.Routing;

    public class FakeNancyModuleWithRouteCacheProviderDependency : NancyModule
    {
        public IRouteCacheProvider RouteCacheProvider { get; private set; }

        public FakeNancyModuleWithRouteCacheProviderDependency(IRouteCacheProvider routeCacheProvider)
        {
            RouteCacheProvider = routeCacheProvider;
        }
    }
}