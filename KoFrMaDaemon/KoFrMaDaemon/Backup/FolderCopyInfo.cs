using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoFrMaDaemon.Backup
{
    public class FolderCopyInfo
    {
        /// <summary>
        /// Info about folder that would be created
        /// </summary>
        public DirectoryInfo SourceFolderInfo { get; set; }
        /// <summary>
        /// Full path to where the folder should be created
        /// </summary>
        public string DestinationPath { get; set; }
    }
}
