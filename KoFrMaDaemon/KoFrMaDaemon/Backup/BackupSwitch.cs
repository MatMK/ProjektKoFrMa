using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using KoFrMaDaemon;

namespace KoFrMaDaemon.Backup
{
    public class BackupSwitch
    {
        protected DirectoryInfo sourceInfo;
        protected DirectoryInfo destinationInfo;
        public BackupJournalObject BackupJournalNew;
        public DebugLog taskDebugLog;
        private DirectoryInfo temporaryDestinationInfo;

        public void Backup(string source, BackupJournalObject backupJournalSource, List<string> destinations, byte? compressionLevel, int TaskID, DebugLog debugLog)
        {
            bool atLeastOneDestinationIsLocalFolder = false;
            for (int i = 0; i < destinations.Count; i++)
            {
                if (destinations[i].EndsWith(".zip"))
                {

                    //debugLog.WriteToLog("Starting backuping to archive, because the path to destination ends with .zip (" + destinations[i] + ')', 5);
                    //Compression compression = new Compression(debugLog);
                    //compression.CompressToZip(destinationInfo.FullName, destinationInfo.Parent.FullName + @"\" + this.destinationInfo.Name + ".zip", compressionLevel);
                    //debugLog.WriteToLog("Compression done, deleting temporary files that were needed for compression", 6);
                    //Directory.Delete(destinationInfo.FullName, true);
                    //debugLog.WriteToLog("Files successfully deleted, compression is now completed.", 6);
                }
                else if (destinations[i].StartsWith("\\"))
                {

                }
                else if (destinations[i].StartsWith("ftp://"))
                {

                }
                else if (destinations[i].StartsWith("sftp://"))
                {

                }
                else
                {
                    if (!atLeastOneDestinationIsLocalFolder)
                    {
                        this.temporaryDestinationInfo = new DirectoryInfo(destinations[i]);
                        atLeastOneDestinationIsLocalFolder = true;
                    }
                }
                //else
                //{
                //    debugLog.WriteToLog("Keeping the plain backup because the task doesn't want to archive", 5);
                //}
                //debugLog.WriteToLog("Backup done, ending the backup instance", 7);


            }

            if (!atLeastOneDestinationIsLocalFolder)
            {
                Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "KoFrMaBackupTemp"));
                this.destinationInfo = new DirectoryInfo(Path.Combine(Path.GetTempPath(),"KoFrMaBackupTemp"));
            }

            this.BackupToFolder(source, backupJournalSource, this.temporaryDestinationInfo.FullName, TaskID, debugLog);

            for (int i = 0; i < destinations.Count; i++)
            {
                this.FinishBackup(this.temporaryDestinationInfo.FullName, destinations[i], compressionLevel);
                //ServiceKoFrMa.debugLog.WriteToLog("Compression done, deleting temporary files that were needed for compression", 6);
                //Directory.Delete(destinationInfo.FullName, true);
                //ServiceKoFrMa.debugLog.WriteToLog("Files successfully deleted, compression is now completed.", 6);
            }
            if (!atLeastOneDestinationIsLocalFolder)
            {
                Directory.Delete(Path.Combine(Path.GetTempPath(), "KoFrMaBackupTemp"), true);
            }
            ServiceKoFrMa.debugLog.WriteToLog("Backup done, ending the backup instance", 7);
        }
        private void BackupToFolder(string source, BackupJournalObject backupJournalSource, string destination, int TaskID,DebugLog debugLog)
        {
            if (backupJournalSource != null)
            {
                debugLog.WriteToLog("Starting differential/incremental backup, because journal was received from server", 5);
                BackupDifferential backupDifferential = new BackupDifferential();
                backupDifferential.BackupDifferentialProcess(backupJournalSource, Path.GetDirectoryName(destination),TaskID, debugLog);
                this.sourceInfo = backupDifferential.sourceInfo;
                this.destinationInfo = backupDifferential.destinationInfo;
                BackupJournalNew = backupDifferential.BackupJournalNew;
            }
            else
            {
                debugLog.WriteToLog("Starting full backup, because the there is no info about backup journal", 5);
                BackupFull backupFull = new BackupFull();
                backupFull.BackupFullProcess(source, Path.GetDirectoryName(destination), TaskID, debugLog);
                this.sourceInfo = backupFull.sourceInfo;
                this.destinationInfo = backupFull.destinationInfo;
                BackupJournalNew = backupFull.BackupJournalNew;
            }

        }

        private void FinishBackup(string backupPath,string destination, byte? compressionLevel)
        {
            if (destination.EndsWith(".zip"))
            {
                ServiceKoFrMa.debugLog.WriteToLog("Starting backuping to archive, because the path to destination ends with .zip (" + destination + ')', 5);
                Compression compression = new Compression();
                compression.CompressToZip(backupPath, destination + @"\" + this.destinationInfo.Name + ".zip", compressionLevel);

            }
            else if (destination.StartsWith("\\"))
            {
                ServiceKoFrMa.debugLog.WriteToLog("Starting backuping to archive, because the path to destination ends with .zip (" + destination + ')', 5);

            }
            else if (destination.StartsWith("ftp://"))
            {
                ServiceKoFrMa.debugLog.WriteToLog("Starting uploading files to ftp, because the path to destination starts with ftp:// (" + destination + ')', 5);

            }
            else if (destination.StartsWith("sftp://"))
            {
                ServiceKoFrMa.debugLog.WriteToLog("Starting uploading files to ftp, because the path to destination starts with sftp:// (" + destination + ')', 5);

            }

            else
            {
                if (!(backupPath==destination))
                {
                    ServiceKoFrMa.debugLog.WriteToLog("Copying files to another local folder...", 6);
                    Directory.CreateDirectory(destination);
                    this.CopyDirectoryRecursivly(new DirectoryInfo(backupPath), new DirectoryInfo(destination));
                }
                else
                {
                    ServiceKoFrMa.debugLog.WriteToLog("Keeping the plain backup where it is.", 6);
                }

            }

        }


            


        private void CopyDirectoryRecursivly(DirectoryInfo from, DirectoryInfo to)
        {
            try
            {
                to.Create();
                foreach (FileInfo item in from.GetFiles())
                {
                    item.CopyTo(to.FullName + @"\" + item.Name);
                }

                foreach (DirectoryInfo item in from.GetDirectories())
                {
                    this.CopyDirectoryRecursivly(item, to.CreateSubdirectory(item.Name));
                }
            }
            catch (Exception)
            {
            }
        }



        protected string CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }
}
