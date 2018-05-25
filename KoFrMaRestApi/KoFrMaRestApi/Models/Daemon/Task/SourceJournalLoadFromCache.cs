using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.Daemon.Task
{
    public class SourceJournalLoadFromCache:ISource
    {
        /// <summary>
        /// Task ID indetifying from where the backup journal will be loaded from offline cache, only works if the file is in the offline cache, otherwise it is needed to send it!
        /// </summary>
        public int JournalID { get; set; }
    }
}