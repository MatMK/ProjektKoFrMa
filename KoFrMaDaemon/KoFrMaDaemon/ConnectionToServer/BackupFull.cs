using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace KoFrMaDaemon.ConnectionToServer
{
    public class BackupFull:BackupSwitch
    {
        public List<FileInfoObject> FilesCorrect = new List<FileInfoObject>(1000);
        public List<FolderObject> FoldersCorrect = new List<FolderObject>(1000);
        public List<String> FilesError = new List<string>(100);
        public List<String> FoldersError = new List<string>(100);



        public void BackupFullProcess(string source, string destination, DebugLog serviceDebugLog)
        {
            DateTime timeOfBackup = DateTime.Now;

            string temporaryDebugInfo = "";
            if (serviceDebugLog._logLevel >= 4)
                temporaryDebugInfo = "Full backup started at  " + timeOfBackup.ToString();
            Directory.CreateDirectory(destination + @"\KoFrMaBackup_" + String.Format("{0:yyyy_MM_dd_HH_mm_ss}", timeOfBackup) + @"_Full\KoFrMaBackup");
            //destinationInfo = new DirectoryInfo(destination).CreateSubdirectory("KoFrMaBackup_" + String.Format("{0:yyyy_MM_dd_HH_mm_ss}", timeOfBackup)+"_Full").CreateSubdirectory("KoFrMaBackup");
            destinationInfo = new DirectoryInfo(destination + @"\KoFrMaBackup_" + String.Format("{0:yyyy_MM_dd_HH_mm_ss}", timeOfBackup) + @"_Full\KoFrMaBackup");

            serviceDebugLog.WriteToLog("Log of including operations is located in " + destinationInfo.Parent.FullName + @"\KoFrMaDebug.log", 4);

            DebugLog DebugLog = new DebugLog(destinationInfo.Parent.FullName + @"\KoFrMaDebug.log", serviceDebugLog._logLevel);

            DebugLog.WriteToLog("Subdirectory for the backup was created at " + destinationInfo.FullName, 5);

            DebugLog.WriteToLog(temporaryDebugInfo, 4);
            temporaryDebugInfo = null;

            sourceInfo = new DirectoryInfo(source);

            if (!sourceInfo.Exists)
            {
                DebugLog.WriteToLog("Fatal Error: Cannot backup because source folder doesn't exists!", 2);
            }



            try
            {
                DebugLog.WriteToLog("Backuping now...", 4);
                this.CopyDirectoryRecursivly(sourceInfo, destinationInfo);
                DebugLog.WriteToLog("Backup done, " + FilesCorrect.Count.ToString() + " files and " + FoldersCorrect.Count.ToString() + " folders successfully backuped, it was unable to backup " + FilesError.Count + " files and " + FoldersError.Count + " folders", 5);
            }
            catch (Exception x)
            {
                DebugLog.WriteToLog("Error " + x.Message + " occured and backup couldn't be fully done", 3);
            }

            DebugLog.WriteToLog("Creating transaction jounal of successfully backuped files and folders...", 5);
            BackupJournalOperations BackupJournal = new BackupJournalOperations();
            BackupJournal.CreateBackupJournal(new BackupJournalObject() { RelativePath = source, BackupJournalFiles = FilesCorrect, BackupJournalFolders = FoldersCorrect }, destinationInfo.Parent.FullName + @"\KoFrMaBackup.dat", DebugLog);
            //BackupLog.CreateBackupLog(BackupLog.LoadBackupList(destinationInfo.Parent.FullName + @"\" + "KoFrMaBackup.dat"), destinationInfo.Parent.FullName + @"\" + "KoFrMaBackup2.dat");
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
                    FilesCorrect.Add(new FileInfoObject { RelativePathName = item.FullName.Remove(0, sourceInfo.FullName.Length), Length = item.Length, CreationTimeUtc = item.CreationTimeUtc, LastWriteTimeUtc = item.LastWriteTimeUtc, Attributes = item.Attributes.ToString(), MD5 = this.CalculateMD5(item.FullName) });
                    //this.FilesCorrect.Add(item.DirectoryName + '|' + item.FullName + '|' +  item.Length.ToString() + '|' + item.CreationTimeUtc.ToString() + '|' + item.LastWriteTimeUtc.ToString() + '|' + item.LastAccessTimeUtc.ToString() + '|' + item.Attributes.ToString() + '|' + this.CalculateMD5(item.FullName));
                }
                catch (Exception x)
                {
                    this.FilesError.Add(item.FullName);
                }

            }

            foreach (DirectoryInfo item in from.GetDirectories())
            {
                try
                {
                    this.CopyDirectoryRecursivly(item, to.CreateSubdirectory(item.Name));
                    //this.FoldersCorrect.Add(item.FullName.Remove(0, sourceInfo.FullName.Length));
                    this.FoldersCorrect.Add(new FolderObject() { FolderPath = item.FullName.Remove(0, sourceInfo.FullName.Length), CreationTimeUtc = item.CreationTimeUtc, LastWriteTimeUtc = item.LastWriteTimeUtc, Attributes = item.Attributes.ToString() });
                }
                catch (Exception x)
                {
                    this.FoldersError.Add(item.FullName);
                }

            }
        }


    }
}
