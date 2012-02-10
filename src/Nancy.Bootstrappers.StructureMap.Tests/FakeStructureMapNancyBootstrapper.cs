namespace Nancy.BootStrappers.StructureMap.Tests
{
    using Nancy.Bootstrappers.StructureMap;
    using Nancy.Tests.Fakes;

    using global::StructureMap;

    public class FakeStructureMapNancyBootstrapper : StructureMapNancyBootstrapper
    {
        public bool ApplicationContainerConfigured { get; set; }

        public IContainer Container
        {
            get { return ApplicationContainer; }
        }

        public bool RequestContainerConfigured { get; set; }

        protected override void ConfigureApplicationContainer(IContainer existingContainer)
        {
            this.ApplicationContainerConfigured = true;
            base.ConfigureApplicationContainer(existingContainer);
        }

        protected override void ConfigureRequestContainer(IContainer container, NancyContext context)
        {
            base.ConfigureRequestContainer(container, context);

            RequestContainerConfigured = true;

            container.Configure(registry =>
            {
                registry.For<IFoo>().Singleton().Use<Foo>();
                registry.For<IDependency>().Singleton().Use<Dependency>();
            });
        }
    }
}