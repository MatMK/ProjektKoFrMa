using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoFrMaDaemon
{
    public class BackupJournalObject
    {
        public string RelativePath { get; set; }
        public List<FileInfoObject> BackupJournalFiles { get; set; }

        public List<FolderObject> BackupJournalFolders { get; set; }
    }
}
