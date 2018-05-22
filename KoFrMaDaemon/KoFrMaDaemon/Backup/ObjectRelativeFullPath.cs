using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoFrMaDaemon.Backup
{
    public class ObjectRelativeFullPath
    {
        /// <summary>
        /// Full path to file/folder
        /// </summary>
        public string FullPath { get; set; }
        /// <summary>
        /// Relative path to file/folder to the source folder
        /// </summary>
        public string RelativePath { get; set; }
    }
}
