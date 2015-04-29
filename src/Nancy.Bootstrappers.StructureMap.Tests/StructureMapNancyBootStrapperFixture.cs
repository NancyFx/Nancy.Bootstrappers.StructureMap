namespace Nancy.Bootstrappers.StructureMap.Tests
{
    using System;
    using System.Linq;

    using Nancy.Tests;
    using Nancy.Tests.Fakes;
    using Nancy.Routing;

    using Xunit;

    public class StructureMapNancyBootstrapperFixture : IDisposable
    {
        private readonly FakeStructureMapNancyBootstrapper bootstrapper;

        public StructureMapNancyBootstrapperFixture()
        {
            this.bootstrapper = new FakeStructureMapNancyBootstrapper();
            this.bootstrapper.Initialise();
        }

        [Fact]
        public void Should_be_able_to_resolve_engine()
        {
            // Given
            // When
            var result = bootstrapper.GetEngine();

            // Then
            result.ShouldNotBeNull();
            result.ShouldBeOfType<INancyEngine>();
        }

        [Fact]
        public void GetAllModules_Returns_As_MultiInstance()
        {
            // Given
            this.bootstrapper.GetEngine();
            var context = new NancyContext(); ;

            // When
            var output1 = bootstrapper.GetAllModules(context).FirstOrDefault(nm => nm.GetType() == typeof(FakeNancyModuleWithBasePath));
            var output2 = bootstrapper.GetAllModules(context).FirstOrDefault(nm => nm.GetType() == typeof(FakeNancyModuleWithBasePath));

            // Then
            output1.ShouldNotBeNull();
            output2.ShouldNotBeNull();
            output1.ShouldNotBeSameAs(output2);
        }

        [Fact]
        public void GetAllModules_Configures_Child_Container()
        {
            // Given
            this.bootstrapper.GetEngine();
            this.bootstrapper.RequestContainerConfigured = false;

            // When
            this.bootstrapper.GetAllModules(new NancyContext());

            // Then
            this.bootstrapper.RequestContainerConfigured.ShouldBeTrue();
        }

        [Fact]
        public void GetModule_Configures_Child_Container()
        {
            // Given
            this.bootstrapper.GetEngine();
            this.bootstrapper.RequestContainerConfigured = false;

            // When
            this.bootstrapper.GetModule(typeof(FakeNancyModuleWithBasePath), new NancyContext());

            // Then
            this.bootstrapper.RequestContainerConfigured.ShouldBeTrue();
        }

        [Fact]
        public void GetEngine_ConfigureApplicationContainer_Should_Be_Called()
        {
            // Given
            // When
            this.bootstrapper.GetEngine();

            // Then
            this.bootstrapper.ApplicationContainerConfigured.ShouldBeTrue();
        }

        [Fact]
        public void Should_not_return_the_same_instance_when_getmodule_is_called_multiple_times_with_different_context()
        {
            // Given
            this.bootstrapper.GetEngine();
            var context1 = new NancyContext();
            var context2 = new NancyContext();

            // When
            var result = this.bootstrapper.GetModule(typeof(FakeNancyModuleWithDependency), context1) as FakeNancyModuleWithDependency;
            var result2 = this.bootstrapper.GetModule(typeof(FakeNancyModuleWithDependency), context2) as FakeNancyModuleWithDependency;

            // Then
            result.FooDependency.ShouldNotBeNull();
            result2.FooDependency.ShouldNotBeNull();
            result.FooDependency.ShouldNotBeSameAs(result2.FooDependency);
        }

        [Fact]
        public void Should_resolve_module_with_dependency_on_RouteCacheFactory()
        {
            // Given
            this.bootstrapper.GetEngine();
            var context = new NancyContext();

            // When
            var result = this.bootstrapper.GetModule(typeof(FakeNancyModuleWithRouteCacheProviderDependency), context) as FakeNancyModuleWithRouteCacheProviderDependency;

            // Then
            result.RouteCacheProvider.ShouldNotBeNull();
            result.RouteCacheProvider.ShouldBeOfType(typeof(DefaultRouteCacheProvider));
        }

        [Fact]
        public void Should_resolve_IRequestStartup_types()
        {
            // Given
            var nancyEngine = this.bootstrapper.GetEngine();
            var context = new NancyContext();

            // When
            nancyEngine.RequestPipelinesFactory(context);

            // Then
            ((bool)context.ViewBag.RequestStartupHasRun).ShouldBeTrue();
        }

        public void Dispose()
        {
            this.bootstrapper.Dispose();
        }
    }
}