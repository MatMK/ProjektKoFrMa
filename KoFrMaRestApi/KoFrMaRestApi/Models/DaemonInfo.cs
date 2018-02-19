using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models
{
    public class DaemonInfo
    {
        public int Version { get; set; }
        public int OS { get; set; }
        public string PC_Unique { get; set; }

        public byte LogLevel { get; set; }
        /*
        0 = Don't create log
        1 = Fatal errors only that shuts down whole service/program
        2 = Errors that cause some process to fail
        3 = Errors that program can handle
        4 = Basic info about operations that program runs
        5 = Debug info that could lead to fixing or optimizing some processes
        6 = Tracing info for every process that is likely to fail
        7 = Tracing info about everything program does
        8 = Tracing info including loop cycles
        9 = Tracing info including large loop cycles that will slow down the process a lot
        10 = Program will be more like a log writer than actually doing the process        
         */
    }
}