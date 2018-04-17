using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.Daemon.Task.BackupJournal
{
    public class BackupJournalObject:ISource
    {
        public string RelativePath { get; set; }
        public List<FileInfoObject> BackupJournalFiles { get; set; }
        public List<FolderObject> BackupJournalFolders { get; set; }
    }
}