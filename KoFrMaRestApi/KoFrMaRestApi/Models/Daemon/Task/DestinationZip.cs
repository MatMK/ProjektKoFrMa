using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.Daemon.Task
{
    public class DestinationZip : IDestination
    {
        /// <summary>
        /// Path to where the zip archive will be stored
        /// </summary>
        public IDestinationPath Path { get; set; }

        /// What level of compression will be used
        /// 0 = Optimal
        /// 1 = Fastest
        /// 2 = No Compression
        public byte CompressionLevel { get; set; }

        /// <summary>
        /// After what number of MiBs will be the archive split (optional, if not entered or null archive won't be split)
        /// </summary>
        public int? SplitAfter { get; set; }
    }
}