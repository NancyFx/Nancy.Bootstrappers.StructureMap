namespace Nancy.Bootstrappers.StructureMap.Tests
{
    using System;
    using global::StructureMap;
    using Nancy.Bootstrapper;
    using Nancy.Bootstrappers.StructureMap;
    using Nancy.Tests.Fakes;

    public class FakeStructureMapNancyBootstrapper : StructureMapNancyBootstrapper
    {
        public bool ApplicationContainerConfigured { get; set; }
        public bool RequestContainerConfigured { get; set; }

        private readonly Func<ITypeCatalog, NancyInternalConfiguration> configuration;

        public FakeStructureMapNancyBootstrapper()
            : this(null)
        {
        }

        public FakeStructureMapNancyBootstrapper(Func<ITypeCatalog, NancyInternalConfiguration> configuration)
        {
            this.configuration = configuration;
        }

        protected override Func<ITypeCatalog, NancyInternalConfiguration> InternalConfiguration
        {
            get { return configuration ?? base.InternalConfiguration; }

        }

        public IContainer Container
        {
            get { return ApplicationContainer; }
        }

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
                registry.For<IFoo>().Use<Foo>();
                registry.For<IDependency>().Use<Dependency>();
            });
        }
    }

    public class FakeNancyRequestStartup : IRequestStartup
    {
        public void Initialize(IPipelines pipelines, NancyContext context)
        {
            // Observable side-effect of the execution of this IRequestStartup.
            context.ViewBag.RequestStartupHasRun = true;
        }
    }
}