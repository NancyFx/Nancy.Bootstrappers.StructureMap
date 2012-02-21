namespace Nancy.Demo.Bootstrappers.StructureMap
{
    public class MainModule : NancyModule
    {
        public MainModule()
        {
            Get["/"] = _ => View["Index"];
        }
    }
}