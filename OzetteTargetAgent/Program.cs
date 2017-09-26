using System.ServiceProcess;

namespace OzetteTargetAgent
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
                new OzetteTargetAgent()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
