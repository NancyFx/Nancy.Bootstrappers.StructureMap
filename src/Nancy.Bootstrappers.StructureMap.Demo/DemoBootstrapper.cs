namespace Nancy.BootStrappers.StructureMap.Demo
{
    using Nancy.Bootstrappers.StructureMap;

    public class DemoBootstrapper : StructureMapNancyBootstrapper
    {
        protected override void InitialiseInternal(global::StructureMap.IContainer container)
        {
            base.InitialiseInternal(container);
        }
    }
}