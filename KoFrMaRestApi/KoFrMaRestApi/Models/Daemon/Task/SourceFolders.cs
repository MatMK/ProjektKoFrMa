using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.Daemon.Task
{
    public class SourceFolders:ISource
    {
        /// <summary>
        /// List of paths to backup
        /// </summary>
        public List<string> Paths { get; set; }
    }
}