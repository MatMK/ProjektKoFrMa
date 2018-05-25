using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.Daemon.Task
{
    class Destination7z : IDestination
    {
        /// <summary>
        /// Path to where the 7z archive will be stored
        /// </summary>
        public IDestinationPath Path { get; set; }

        /// What level of compression will be used
        /// 0 = No Compression
        /// 1 = Fastest
        /// 3 = Fast
        /// 5 = Normal
        /// 7 = Maximum (not quite true)
        /// 9 = Ultra
        public byte CompressionLevel { get; set; }

        /// <summary>
        /// After what number of MiBs will be the archive split (optional, if not entered or null archive won't be split)
        /// </summary>
        public int? SplitAfter { get; set; }
    }
}