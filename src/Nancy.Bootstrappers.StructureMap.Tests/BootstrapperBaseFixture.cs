#if !__MonoCS__ 
namespace Nancy.Bootstrappers.StructureMap.Tests
{
    using Bootstrapper;
    using Nancy.Tests.Unit.Bootstrapper.Base;
    using global::StructureMap;

    public class BootstrapperBaseFixture : BootstrapperBaseFixtureBase<IContainer>
    {
        private readonly StructureMapNancyBootstrapper bootstrapper;

        public BootstrapperBaseFixture()
        {
            this.bootstrapper = new FakeStructureMapNancyBootstrapper(this.Configuration);
        }

        protected override NancyBootstrapperBase<IContainer> Bootstrapper
        {
            get { return this.bootstrapper; }
        }
    }
}
#endif