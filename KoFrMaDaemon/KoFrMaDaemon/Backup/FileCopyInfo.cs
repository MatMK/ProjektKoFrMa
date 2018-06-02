using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoFrMaDaemon.Backup
{
    public class FileCopyInfo
    {
        /// <summary>
        /// Info about file that would be copied
        /// </summary>
        public FileInfo SourceFileInfo { get; set; }
        /// <summary>
        /// Full path to where the file should be copied
        /// </summary>
        public string DestinationPath { get; set; }
    }
}
