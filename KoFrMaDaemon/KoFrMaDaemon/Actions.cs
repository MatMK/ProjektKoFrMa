using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace KoFrMaDaemon
{
    public class Actions
    {
        
        //public string State;

        //const int dim1 = 5;
        //public string[,] poleStr = new string[dim1, dim1];

        public List<FileInfoObject> FilesCorrect =new List<FileInfoObject>(1000);
        public List<String> FoldersCorrect = new List<string>(1000);
        public List<String> FilesError = new List<string>(1000);
        public List<String> FoldersError = new List<string>(1000);

        private LogOperations BackupLog;
        public LogOperations DebugLog;

        DirectoryInfo sourceInfo;
        DirectoryInfo destinationInfo;

        public void BackupFullFolder(string source, string destination, bool createLog)
        {
            DateTime timeOfBackup = DateTime.Now;

            sourceInfo = new DirectoryInfo(source);
            destinationInfo = new DirectoryInfo(destination).CreateSubdirectory("KoFrMaBackup_" + String.Format("{0:yyyy_MM_dd_HH_mm_ss}", timeOfBackup)).CreateSubdirectory("KoFrMaBackup");
            //DirectoryInfo destinationInfo = new DirectoryInfo(destination).CreateSubdirectory("KoFrMaBackup" + timeOfBackup.Year + timeOfBackup.Month + timeOfBackup.Day + timeOfBackup.Hour + timeOfBackup.Minute).CreateSubdirectory("KoFrMaBackup");
            
            BackupLog = new LogOperations(destinationInfo.Parent.FullName + @"\" + "KoFrMaBackup.dat");
            DebugLog = new LogOperations(destinationInfo.Parent.FullName + @"\" + "KoFrMaDebug.log");
            DebugLog.WriteToLog("Vytvořena podsložka pro Full Backup zálohu");

            try
            {
                DebugLog.WriteToLog("Zálohuji složku...");
                this.CopyDirectoryRecursivly(sourceInfo,destinationInfo, true);
                DebugLog.WriteToLog("Složka úspěšně zálohována");
            }
            catch (Exception x)
            {
                DebugLog.WriteToLog("Došlo k chybě " + x.Message + " a záloha nemohla být dokončena");
            }

            DebugLog.WriteToLog("Vytváření logu úspěšně zálohovaných souborů...");
            BackupLog.CreateBackupJournal(FilesCorrect, sourceInfo.FullName ,destinationInfo.Parent.FullName + @"\KoFrMaBackup.dat",FoldersCorrect);
            //BackupLog.CreateBackupLog(BackupLog.LoadBackupList(destinationInfo.Parent.FullName + @"\" + "KoFrMaBackup.dat"), destinationInfo.Parent.FullName + @"\" + "KoFrMaBackup2.dat");
            DebugLog.WriteToLog("Log byl úspěšně vytvořen");
            BackupLog = null;
            DebugLog.WriteToLog("Full backup " + timeOfBackup.ToString() + " byl dokončen");

        }


        public void BackupDifferential(string source, string destination, string OriginalBackupDatFilePath, bool createLog)
        {
            DateTime timeOfBackup = DateTime.Now;
            sourceInfo = new DirectoryInfo(source);
            destinationInfo = new DirectoryInfo(destination).CreateSubdirectory("KoFrMaBackup" + timeOfBackup.Year + timeOfBackup.Month + timeOfBackup.Day + timeOfBackup.Hour + timeOfBackup.Minute).CreateSubdirectory("KoFrMaBackup");
            BackupLog = new LogOperations(destinationInfo.Parent.FullName + @"\" + "KoFrMaBackup.dat");
            DebugLog = new LogOperations(destinationInfo.Parent.FullName + @"\" + "KoFrMaDebug.log");
            DebugLog.WriteToLog("Subdirectory for Differential/Incremental backup was created");
            DebugLog.WriteToLog("Creating list of current files and folders...");
            this.CopyDirectoryRecursivly(sourceInfo, null, false);
            List<FileInfoObject> CurrentFiles = FilesCorrect;
            DebugLog.WriteToLog("List of current files and folders successfully created");
            DebugLog.WriteToLog("Loading list of files from original source of the backup...");
            List<FileInfoObject> OriginalFiles = BackupLog.LoadBackupJournalFiles(OriginalBackupDatFilePath);
            List<string> CurrentFolders = FoldersCorrect;
            List<FolderObject> CurrentFoldersObjects = new List<FolderObject>();
            DebugLog.WriteToLog("Loading list of folders from original source of the backup...");
            List<string> OriginalFolders = BackupLog.LoadBackupJournalFolders(OriginalBackupDatFilePath);
            List<FolderObject> OriginalFoldersObjects = new List<FolderObject>();
            List<bool> OriginalFoldersPaired = new List<bool>();
            List<string> FilesToCopy = new List<string>(FilesCorrect.Count/4);
            List<string> FilesToDeleteFromOriginal = new List<string>(FilesCorrect.Count / 8);
            List<string> FoldersToCreate = new List<string>();
            List<string> FoldersToDelete = new List<string>();
            DebugLog.WriteToLog("Comparing list of current files to original source of the backup...");
            foreach (FileInfoObject itemCurrent in CurrentFiles)
            {
                foreach (FileInfoObject itemOriginal in OriginalFiles)
                {
                    bool sameFile = false;
                    if (itemCurrent.HashRow == itemOriginal.HashRow)
                    {
                        if (itemCurrent.RelativePathName == itemOriginal.RelativePathName
                            && itemCurrent.Length == itemOriginal.Length
                            && itemCurrent.CreationTimeUtc == itemOriginal.CreationTimeUtc
                            && itemCurrent.LastWriteTimeUtc == itemOriginal.LastWriteTimeUtc
                            && itemCurrent.MD5 == itemOriginal.MD5
                            && itemCurrent.Attributes == itemOriginal.Attributes)
                        {
                             sameFile = true;
                             itemOriginal.Paired = true;
                             break;
                        }
                    
                    }
                    if (!sameFile)
                    {
                        FilesToCopy.Add(itemOriginal.RelativePathName);
                    }
                }
            }

            DebugLog.WriteToLog("Comparison of files successfully done");

            DebugLog.WriteToLog("Comparing list of current folders to original source of backup...");

            for (int i = 0; i < CurrentFolders.Count; i++)
            {
                CurrentFoldersObjects.Add(new FolderObject { FolderPath = CurrentFolders[i] });
            }
            foreach (FolderObject itemCurrent in CurrentFoldersObjects)
            {
                bool sameFolder = false;
                foreach (FolderObject itemOriginal in OriginalFoldersObjects)
                {
                    if (itemCurrent.FolderPath==itemOriginal.FolderPath)
                    {
                        sameFolder = true;
                        itemOriginal.Paired = true;
                        break;
                    }
                    if (!sameFolder)
                    {
                        FoldersToCreate.Add(itemOriginal.FolderPath);
                    }
                }
            }
            DebugLog.WriteToLog("Comparison of folders successfully done");
            DebugLog.WriteToLog("Creating list of folders that no longer exists...");
            foreach (FolderObject itemOriginal in OriginalFoldersObjects)
            {
                if (!itemOriginal.Paired)
                {
                    FoldersToDelete.Add(BackupLog.LoadBackupRelativePath(OriginalBackupDatFilePath) + itemOriginal.FolderPath);
                }
            }
            DebugLog.WriteToLog("List of folders that no longer exists successfully created");
            // M * delete:
            DebugLog.WriteToLog("Creating list of files that no longer exists...");
            foreach (FileInfoObject itemOriginal in OriginalFiles)
            {
                if (!itemOriginal.Paired)
                {
                    FilesToDeleteFromOriginal.Add(BackupLog.LoadBackupRelativePath(OriginalBackupDatFilePath) + itemOriginal.RelativePathName);
                }
            }
            DebugLog.WriteToLog("List of files that no longer exists successfully created");


            DebugLog.WriteToLog("Backuping modified folder structure...");
            DirectoryInfo tmpDirectoryInfo;
            for (int i = 0; i < FoldersToCreate.Count; i++)
            {
                tmpDirectoryInfo = new DirectoryInfo(destinationInfo.FullName + FoldersToCreate[i]);
                try
                {
                    tmpDirectoryInfo.Create();
                    this.FoldersCorrect.Add(tmpDirectoryInfo.FullName.Remove(0, sourceInfo.FullName.Length));
                }
                catch (Exception x)
                {
                    this.FoldersError.Add(tmpDirectoryInfo.FullName);
                }
            }

            DebugLog.WriteToLog("Backup of folder structure successfully done");







            DebugLog.WriteToLog("Backuping new or modified files to " + destinationInfo.FullName);
            FileInfo tmpFileInfo;
            for (int i = 0; i < FilesToCopy.Count; i++)
            {
                tmpFileInfo = new FileInfo(BackupLog.LoadBackupRelativePath(OriginalBackupDatFilePath) + FilesToCopy[i]);
                try
                {
                    tmpFileInfo.CopyTo(destinationInfo.FullName + @"\" + FilesToCopy[i]);
                    FilesCorrect.Add(new FileInfoObject { RelativePathName = tmpFileInfo.FullName.Remove(0, sourceInfo.FullName.Length), Length = tmpFileInfo.Length, CreationTimeUtc = tmpFileInfo.CreationTimeUtc, LastWriteTimeUtc = tmpFileInfo.LastWriteTimeUtc, Attributes = tmpFileInfo.Attributes.ToString(), MD5 = this.CalculateMD5(tmpFileInfo.FullName) });
                }
                catch (Exception x)
                {
                    this.FilesError.Add(tmpFileInfo.FullName);
                }
                
            }
            DebugLog.WriteToLog("Creating transaction log of successfully copied files and folders...");
            BackupLog.CreateBackupJournal(FilesCorrect, sourceInfo.FullName, destinationInfo.Parent.FullName + @"\KoFrMaBackup.dat",FoldersCorrect);
            //BackupLog.CreateBackupLog(BackupLog.LoadBackupList(destinationInfo.Parent.FullName + @"\" + "KoFrMaBackup.dat"), destinationInfo.Parent.FullName + @"\" + "KoFrMaBackup2.dat");
            DebugLog.WriteToLog("Transaction log successfully created");
            BackupLog = null;
            DebugLog.WriteToLog("Differential/Incremental backup of " + timeOfBackup.ToString() + " done.");

        }
        
        private void CopyDirectoryRecursivly(DirectoryInfo from, DirectoryInfo to, bool Copy)
        {
            if (Copy&&!to.Exists)
            {
                to.Create();
            }
                
                foreach (FileInfo item in from.GetFiles())
                {
                    try
                    {
                        if (Copy)
                    {
                        item.CopyTo(to.FullName + @"\" + item.Name);
                    }
                        
                        FilesCorrect.Add(new FileInfoObject { RelativePathName = item.FullName.Remove(0,sourceInfo.FullName.Length), Length = item.Length, CreationTimeUtc = item.CreationTimeUtc, LastWriteTimeUtc = item.LastWriteTimeUtc, Attributes = item.Attributes.ToString(), MD5 = this.CalculateMD5(item.FullName) });
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
                        this.CopyDirectoryRecursivly(item, to.CreateSubdirectory(item.Name), Copy);
                        this.FoldersCorrect.Add(item.FullName.Remove(0, sourceInfo.FullName.Length));
                    }
                    catch (Exception x)
                    {
                        this.FoldersError.Add(item.FullName);
                    }
                    
                }
        }


        string CalculateMD5(string filename)
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
