using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoFrMaDaemon.Backup
{
    public class CopyErrorObject
    {
        /// <summary>
        /// Full path of the file/folder that it was unable to read/write
        /// </summary>
        public string FullPath { get; set; }
        /// <summary>
        /// Reason why the file could not be copied
        /// </summary>
        public string ExceptionMessage { get; set; }
    }
}
