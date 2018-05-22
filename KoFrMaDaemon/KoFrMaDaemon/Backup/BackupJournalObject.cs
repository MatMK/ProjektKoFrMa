using System.Collections.Generic;

namespace KoFrMaDaemon.Backup
{
    public class BackupJournalObject:ISource
    {
        /// <summary>
        /// Path(s) to the backup source(s)
        /// </summary>
        public List<string> RelativePaths { get; set; }
        /// <summary>
        /// List of files that were backuped and information about them to make it easy to compare to future backup
        /// </summary>
        public List<FileInfoObject> BackupJournalFiles { get; set; }
        /// <summary>
        /// List of files were deleted since last time and that need to be deleted before the recovery (only applies to differential/incremental backup)
        /// </summary>
        public List<string> BackupJournalFilesDelete { get; set; }
        /// <summary>
        /// List of folders (only folder structure without files) that were backuped along with some attributes for future backup
        /// </summary>
        public List<FolderObject> BackupJournalFolders { get; set; }
        /// <summary>
        /// List of folders (only folder structure without files) that were deleted since last time and that need to be deleted before the recovery (only applies to differential/incremental backup)
        /// </summary>
        public List<string> BackupJournalFoldersDelete { get; set; }
    }
}
