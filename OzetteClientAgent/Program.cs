using System.ServiceProcess;

namespace OzetteClientAgent
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new OzetteClientAgent()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
