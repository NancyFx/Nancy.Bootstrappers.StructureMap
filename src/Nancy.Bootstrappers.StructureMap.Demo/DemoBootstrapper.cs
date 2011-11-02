namespace Nancy.BootStrappers.StructureMap.Demo
{
    using Nancy.Bootstrappers.StructureMap;

    public class DemoBootstrapper : StructureMapNancyBootstrapper
    {
        protected override void ApplicationStartup(global::StructureMap.IContainer container, Bootstrapper.IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);
        }
    }
}