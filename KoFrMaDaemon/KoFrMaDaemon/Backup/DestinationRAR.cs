using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoFrMaDaemon.Backup
{
    public class DestinationRar : IDestination
    {
        /// <summary>
        /// Path to where the rar archive will be stored
        /// </summary>
        public IDestinationPath Path { get; set; }

        /// What level of compression will be used
        /// 0 = No Compression
        /// 1 = Fastest
        /// 2 = Fast
        /// 3 = Normal
        /// 4 = Good
        /// 5 = Best
        public byte CompressionLevel { get; set; }

        /// <summary>
        /// After what number of MiBs will be the archive split (optional, if not entered or null archive won't be split)
        /// </summary>
        public int? SplitAfter { get; set; }
    }
}
