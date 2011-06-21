namespace Nancy.BootStrappers.StructureMap.Demo
{
    public class MainModule : NancyModule
    {
        public MainModule()
        {
            Get["/"] = _ => "Hello!";
        }
    }
}