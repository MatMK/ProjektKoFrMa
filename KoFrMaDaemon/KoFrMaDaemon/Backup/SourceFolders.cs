using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoFrMaDaemon.Backup
{
    class SourceFolders:ISource
    {
        /// <summary>
        /// List of paths to folders that would be backuped
        /// </summary>
        public List<string> Paths { get; set; }
    }
}
