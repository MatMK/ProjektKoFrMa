using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace KoFrMaDaemon.Backup
{
    public class BackupFull:BackupSwitch
    {
        public List<FileInfoObject> FilesCorrect;
        public List<FolderObject> FoldersCorrect;
        public List<CopyErrorObject> FilesErrorCopy;
        public List<CopyErrorObject> FoldersErrorCopy;

        public BackupFull()
        {
            FilesCorrect = new List<FileInfoObject>(100);
            FoldersCorrect = new List<FolderObject>(100);
            FilesErrorCopy = new List<CopyErrorObject>(100);
            FoldersErrorCopy = new List<CopyErrorObject>(100);
        }


        public void BackupFullProcess(string source, string destination, DebugLog serviceDebugLog)
        {
            DateTime timeOfBackup = DateTime.Now;

            string temporaryDebugInfo = "";
            if (serviceDebugLog._logLevel >= 4)
                temporaryDebugInfo = "Full backup started at  " + timeOfBackup.ToString();
            Directory.CreateDirectory(destination + @"\KoFrMaBackup_" + String.Format("{0:yyyy_MM_dd_HH_mm_ss}", timeOfBackup) + @"_Full\KoFrMaBackup");
            base.destinationInfo = new DirectoryInfo(destination + @"\KoFrMaBackup_" + String.Format("{0:yyyy_MM_dd_HH_mm_ss}", timeOfBackup) + @"_Full\KoFrMaBackup");

            serviceDebugLog.WriteToLog("Log of including operations is located in " + base.destinationInfo.Parent.FullName + @"\KoFrMaDebug.log", 4);

            DebugLog DebugLog = new DebugLog(base.destinationInfo.Parent.FullName + @"\KoFrMaDebug.log", serviceDebugLog._logLevel);

            DebugLog.WriteToLog("Subdirectory for the backup was created at " + base.destinationInfo.FullName, 5);

            DebugLog.WriteToLog(temporaryDebugInfo, 4);
            temporaryDebugInfo = null;

            base.sourceInfo = new DirectoryInfo(source);

            if (!base.sourceInfo.Exists)
            {
                DebugLog.WriteToLog("Fatal Error: Cannot backup because source folder doesn't exists!", 2);
            }



            try
            {
                DebugLog.WriteToLog("Backuping now...", 4);
                this.CopyDirectoryRecursivly(base.sourceInfo, base.destinationInfo);
                DebugLog.WriteToLog("Backup done, " + FilesCorrect.Count.ToString() + " files and " + FoldersCorrect.Count.ToString() + " folders successfully backuped, it was unable to backup " + FilesErrorCopy.Count + " files and " + FoldersErrorCopy.Count + " folders", 5);
            }
            catch (Exception x)
            {
                DebugLog.WriteToLog("Error " + x.Message + " occured and backup couldn't be fully done", 3);
            }

            DebugLog.WriteToLog("Creating transaction jounal of successfully backuped files and folders...", 5);
            BackupJournalOperations BackupJournal = new BackupJournalOperations();
            BackupJournal.CreateBackupJournal(new BackupJournalObject() { RelativePath = source, BackupJournalFiles = FilesCorrect, BackupJournalFolders = FoldersCorrect }, base.destinationInfo.Parent.FullName + @"\KoFrMaBackup.dat", DebugLog);
            DebugLog.WriteToLog("Journal successfully created", 5);
            TimeSpan backupTook = DateTime.Now - timeOfBackup;
            DebugLog.WriteToLog("Full backup was completed in " + backupTook.TotalSeconds + " s", 4);
        }

        private void CopyDirectoryRecursivly(DirectoryInfo from, DirectoryInfo to)
        {
            if (!to.Exists)
            {
                to.Create();
            }

            foreach (FileInfo item in from.GetFiles())
            {
                try
                {
                    item.CopyTo(to.FullName + @"\" + item.Name);
                    FilesCorrect.Add(new FileInfoObject { RelativePath = item.FullName.Remove(0, base.sourceInfo.FullName.Length), Length = item.Length, CreationTimeUtc = item.CreationTimeUtc, LastWriteTimeUtc = item.LastWriteTimeUtc, Attributes = item.Attributes.ToString(), MD5 = this.CalculateMD5(item.FullName) });
                }
                catch (Exception ex)
                {
                    this.FilesErrorCopy.Add(new CopyErrorObject() { FullPath = item.FullName, ExceptionMessage = ex.Message });
                }

            }

            foreach (DirectoryInfo item in from.GetDirectories())
            {
                try
                {
                    this.CopyDirectoryRecursivly(item, to.CreateSubdirectory(item.Name));
                    this.FoldersCorrect.Add(new FolderObject() { RelativePath = item.FullName.Remove(0, base.sourceInfo.FullName.Length), CreationTimeUtc = item.CreationTimeUtc, LastWriteTimeUtc = item.LastWriteTimeUtc, Attributes = item.Attributes.ToString() });
                }
                catch (Exception ex)
                {
                    this.FoldersErrorCopy.Add(new CopyErrorObject() { FullPath = item.FullName, ExceptionMessage = ex.Message });
                }

            }
        }


    }
}
