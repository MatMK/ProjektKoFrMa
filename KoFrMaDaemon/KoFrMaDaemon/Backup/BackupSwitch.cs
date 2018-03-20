using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using KoFrMaDaemon;
using System.Net;

namespace KoFrMaDaemon.Backup
{
    public class BackupSwitch
    {
        protected DirectoryInfo sourceInfo;
        protected DirectoryInfo destinationInfo;
        public BackupJournalObject BackupJournalNew;
        public DebugLog taskDebugLog;
        private DirectoryInfo temporaryDestinationInfo;

        public void Backup(string source, BackupJournalObject backupJournalSource, List<string> destinations, byte? compressionLevel,NetworkCredential networkCredential, int TaskID, DebugLog debugLog)
        {
            bool atLeastOneDestinationIsLocalFolder = false;
            for (int i = 0; i < destinations.Count; i++)
            {
                if (destinations[i].EndsWith(".zip")|| destinations[i].EndsWith(".7z")|| destinations[i].EndsWith(".rar"))
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
                ServiceKoFrMa.debugLog.WriteToLog("No destination point to local path, creating temporary folder at "+ Path.Combine(Path.GetTempPath(), "KoFrMaBackupTemp"), 7);
                Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "KoFrMaBackupTemp"));
                this.temporaryDestinationInfo = new DirectoryInfo(Path.Combine(Path.GetTempPath(),"KoFrMaBackupTemp"));
            }

            ServiceKoFrMa.debugLog.WriteToLog("Starting the backup...", 7);
            this.BackupToFolder(source, backupJournalSource, this.temporaryDestinationInfo.FullName, TaskID, debugLog);
            ServiceKoFrMa.debugLog.WriteToLog("Creating desired destination outputs...", 7);
            for (int i = 0; i < destinations.Count; i++)
            {
                this.FinishBackup(this.destinationInfo.FullName, destinations[i], compressionLevel,networkCredential);
                //ServiceKoFrMa.debugLog.WriteToLog("Compression done, deleting temporary files that were needed for compression", 6);
                //Directory.Delete(destinationInfo.FullName, true);
                //ServiceKoFrMa.debugLog.WriteToLog("Files successfully deleted, compression is now completed.", 6);
            }
            if (!atLeastOneDestinationIsLocalFolder)
            {
                ServiceKoFrMa.debugLog.WriteToLog("Backup done, deleting temporary files at "+ Path.Combine(Path.GetTempPath(), "KoFrMaBackupTemp"), 7);
                Directory.Delete(Path.Combine(Path.GetTempPath(), "KoFrMaBackupTemp"), true);
            }
            ServiceKoFrMa.debugLog.WriteToLog("Backup done, ending the backup instance.", 7);
        }
        private void BackupToFolder(string source, BackupJournalObject backupJournalSource, string destination, int TaskID,DebugLog debugLog)
        {
            ServiceKoFrMa.debugLog.WriteToLog("Deciding what type of backup it is...", 7);
            if (backupJournalSource != null)
            {
                debugLog.WriteToLog("Starting differential/incremental backup, because journal was received from server", 5);
                BackupDifferential backupDifferential = new BackupDifferential();
                backupDifferential.BackupDifferentialProcess(backupJournalSource, destination,TaskID, debugLog);
                this.sourceInfo = backupDifferential.sourceInfo;
                this.destinationInfo = backupDifferential.destinationInfo;
                BackupJournalNew = backupDifferential.BackupJournalNew;
            }
            else
            {
                debugLog.WriteToLog("Starting full backup, because the there is no info about backup journal", 5);
                BackupFull backupFull = new BackupFull();
                backupFull.BackupFullProcess(source, destination, TaskID, debugLog);
                this.sourceInfo = backupFull.sourceInfo.Parent;
                this.destinationInfo = backupFull.destinationInfo.Parent;
                BackupJournalNew = backupFull.BackupJournalNew;
            }

        }

        private void FinishBackup(string backupPath,string destination, byte? compressionLevel, NetworkCredential networkCredential)
        {
            if (destination.EndsWith(".zip"))
            {
                ServiceKoFrMa.debugLog.WriteToLog("Starting backuping to archive, because the path to destination ends with .zip (" + destination + ')', 5);
                Compression compression = new Compression();
                compression.CompressToZip(backupPath, Path.GetDirectoryName(destination) + @"\" + destinationInfo.Name + ".zip", compressionLevel);
            }
            if (destination.EndsWith(".7z"))
            {
                ServiceKoFrMa.debugLog.WriteToLog("Starting backuping to archive, because the path to destination ends with .7z (" + destination + ')', 5);
                Compression compression = new Compression();
                ServiceKoFrMa.debugLog.WriteToLog("Archive will be made from this folder "+backupPath +@"\ and put into this location "+ Path.GetDirectoryName(destination)+@"\"+destinationInfo.Name+".7z", 7);
                compression.CompressTo7z(ServiceKoFrMa.daemonSettings.SevenZipPath,backupPath + @"\", Path.GetDirectoryName(destination) + @"\" + destinationInfo.Name + ".7z", compressionLevel);
            }
            if (destination.EndsWith(".rar"))
            {
                ServiceKoFrMa.debugLog.WriteToLog("Starting backuping to archive, because the path to destination ends with .rar (" + destination + ')', 5);
                Compression compression = new Compression();
                ServiceKoFrMa.debugLog.WriteToLog("Archive will be made from this folder " + backupPath + @"\ and put into this location " + Path.GetDirectoryName(destination) + @"\" + destinationInfo.Name + ".rar", 7);
                compression.CompressToRar(ServiceKoFrMa.daemonSettings.WinRARPath, backupPath + @"\", Path.GetDirectoryName(destination) + @"\" + destinationInfo.Name + ".rar", compressionLevel);
            }
            else if (destination.StartsWith("\\"))
            {
                ServiceKoFrMa.debugLog.WriteToLog("Starting uploading files to samba/shared server, because the path to destination starts with \\ (" + destination + ')', 5);

            }
            else if (destination.StartsWith("ftp://"))
            {
                ServiceKoFrMa.debugLog.WriteToLog("Starting uploading files to ftp, because the path to destination starts with ftp:// (" + destination + ')', 5);
                FTPConnection fTPConnection = new FTPConnection(destination, networkCredential);
                fTPConnection.UploadToFTP(backupPath);

            }
            else if (destination.StartsWith("sftp://"))
            {
                ServiceKoFrMa.debugLog.WriteToLog("Starting uploading files to ftp, because the path to destination starts with sftp:// (" + destination + ')', 5);
                SSHConnection sSHConnection = new SSHConnection(destination.Substring(7), networkCredential);
                sSHConnection.UploadToSSH(backupPath);
            }

            else
            {
                if (!(backupPath==destinationInfo.FullName))
                {
                    ServiceKoFrMa.debugLog.WriteToLog("Copying files to another local folder...", 6);
                    ServiceKoFrMa.debugLog.WriteToLog("because "+backupPath +" is not "+destinationInfo.FullName, 6);
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
