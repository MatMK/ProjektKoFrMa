using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace KoFrMaDaemon
{
    static class Program
    {
        /// <summary>
        /// Main application, nothing interesting here.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new KoFrMaDaemon()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
