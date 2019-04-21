﻿using System.ServiceProcess;

namespace ArchivialClientAgent
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
                new ArchivialClientAgent()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
