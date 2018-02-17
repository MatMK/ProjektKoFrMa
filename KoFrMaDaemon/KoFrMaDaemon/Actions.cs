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

        public void BackupFullFolder(string source, string destination, byte logLevel)
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


        public void BackupDifferential(string destination, string OriginalBackupDatFilePath, byte logLevel)
        {
            DateTime timeOfBackup = DateTime.Now;
            string temporaryDebugInfo;
            if (logLevel >= 4)
                temporaryDebugInfo = "Starting the differential/incremental backup " + timeOfBackup.ToString();
            destinationInfo = new DirectoryInfo(destination).CreateSubdirectory("KoFrMaBackup_" + String.Format("{0:yyyy_MM_dd_HH_mm_ss}", timeOfBackup)).CreateSubdirectory("KoFrMaBackup");
            BackupLog = new LogOperations(destinationInfo.Parent.FullName + @"\" + "KoFrMaBackup.dat");
            DebugLog = new LogOperations(destinationInfo.Parent.FullName + @"\" + "KoFrMaDebug.log");
            if (logLevel >= 5)
                DebugLog.WriteToLog("Subdirectory for the backup was created.");

            string source = BackupLog.LoadBackupRelativePath(OriginalBackupDatFilePath);

            sourceInfo = new DirectoryInfo(source);

            if (logLevel >= 5)
                DebugLog.WriteToLog("Creating list of current files and folders...");
            this.CopyDirectoryRecursivly(sourceInfo, null, false);
            List<FileInfoObject> CurrentFiles = this.ReturnHashCodes(FilesCorrect);

            if (logLevel >= 7)
                DebugLog.CreateBackupJournal(CurrentFiles, "current files", destinationInfo.Parent.FullName + @"\KoFrMaBackupDebugCurrentFiles.dat", new List<string>());

            FilesCorrect = new List<FileInfoObject>();
            List<string> CurrentFolders = FoldersCorrect;
            FoldersCorrect = new List<string>();
            FoldersError = new List<string>();
            FilesError = new List<string>();

            if (logLevel >= 5)
            {
                DebugLog.WriteToLog("List of current files and folders successfully created, conntaining " + CurrentFiles.Count + " files and " + CurrentFolders.Count + " folders. Unable to load " + FilesError.Count + " files and " + FoldersError.Count + " folders.");
                DebugLog.WriteToLog("Loading list of files from original source of the backup...");
            }

            List<FileInfoObject> OriginalFiles = BackupLog.LoadBackupJournalFiles(OriginalBackupDatFilePath);
            List<FolderObject> CurrentFoldersObjects = new List<FolderObject>();
            if (logLevel >= 5)
            {
                DebugLog.WriteToLog("Original source of backup contains " + OriginalFiles.Count + " files");
                DebugLog.WriteToLog("Loading list of folders from original source of the backup...");
            }

            List<string> OriginalFolders = BackupLog.LoadBackupJournalFolders(OriginalBackupDatFilePath);
            List<FolderObject> OriginalFoldersObjects = new List<FolderObject>();
            if (logLevel >= 5)
                DebugLog.WriteToLog("Original source of backup contains " + OriginalFolders.Count + " folders");
            List<string> FilesToCopy = new List<string>(FilesCorrect.Count / 4);
            List<string> FilesToDelete = new List<string>(FilesCorrect.Count / 8);
            List<string> FoldersToCreate = new List<string>();
            List<string> FoldersToDelete = new List<string>();
            if (logLevel >= 5)
                DebugLog.WriteToLog("Comparing list of current files to original source of the backup...");
            bool sameObject;
            foreach (FileInfoObject itemCurrent in CurrentFiles)
            {
                sameObject = false;
                foreach (FileInfoObject itemOriginal in OriginalFiles)
                {

                    if (itemCurrent.HashRow == itemOriginal.HashRow)
                    {
                        if (itemCurrent.RelativePathName == itemOriginal.RelativePathName
                            && itemCurrent.Length == itemOriginal.Length
                            && itemCurrent.CreationTimeUtc == itemOriginal.CreationTimeUtc
                            && itemCurrent.LastWriteTimeUtc == itemOriginal.LastWriteTimeUtc
                            && itemCurrent.MD5 == itemOriginal.MD5
                            && itemCurrent.Attributes == itemOriginal.Attributes)
                        {
                            sameObject = true;
                            itemOriginal.Paired = true;

                            if (logLevel >= 6)
                                DebugLog.WriteToLog("Same object = true");

                            break;
                        }

                        else if (logLevel >= 6)
                        {
                            if (itemCurrent.RelativePathName != itemOriginal.RelativePathName)
                            {
                                DebugLog.WriteToLog("RelativePathName Error" + itemCurrent.RelativePathName + " is not " + itemOriginal.RelativePathName);
                            }
                            if (itemCurrent.Length != itemOriginal.Length)
                            {
                                DebugLog.WriteToLog("Length Error" + itemCurrent.Length.ToString() + " is not " + itemOriginal.Length.ToString());
                            }
                            if (itemCurrent.CreationTimeUtc != itemOriginal.CreationTimeUtc)
                            {
                                DebugLog.WriteToLog("CreationTimeUtc Error: " + itemCurrent.CreationTimeUtc.ToString() + " is not " + itemOriginal.CreationTimeUtc.ToString());
                            }
                            if (itemCurrent.LastWriteTimeUtc != itemOriginal.LastWriteTimeUtc)
                            {
                                DebugLog.WriteToLog("LastWriteTimeUtc Error" + itemCurrent.LastWriteTimeUtc.ToString() + " is not " + itemOriginal.LastWriteTimeUtc.ToString());
                            }
                            if (itemCurrent.MD5 != itemOriginal.MD5)
                            {
                                DebugLog.WriteToLog("MD5 Error" + itemCurrent.MD5 + " is not " + itemOriginal.MD5);
                            }
                            if (itemCurrent.Attributes != itemOriginal.Attributes)
                            {
                                DebugLog.WriteToLog("Attributes Error" + itemCurrent.Attributes + " is not " + itemOriginal.Attributes);
                            }
                        }


                    }
                    else if (logLevel >= 7)
                        DebugLog.WriteToLog("HashRow Error: " + itemCurrent.HashRow.ToString() + " is not " + itemOriginal.HashRow.ToString());

                }
                if (!sameObject)
                {
                    FilesToCopy.Add(itemCurrent.RelativePathName);
                }
            }
            if (logLevel >= 5)
            {
                DebugLog.WriteToLog("Comparison of files successfully done, " + FilesToCopy.Count + " files were created or modified since original backup.");

                DebugLog.WriteToLog("Comparing list of current folders to original source of backup...");
            }


            for (int i = 0; i < CurrentFolders.Count; i++)
            {
                CurrentFoldersObjects.Add(new FolderObject { FolderPath = CurrentFolders[i] });
            }
            foreach (FolderObject itemCurrent in CurrentFoldersObjects)
            {
                sameObject = false;
                foreach (FolderObject itemOriginal in OriginalFoldersObjects)
                {
                    if (itemCurrent.FolderPath == itemOriginal.FolderPath)
                    {
                        sameObject = true;
                        itemOriginal.Paired = true;
                        break;
                    }

                }

                if (!sameObject)
                {
                    FoldersToCreate.Add(itemCurrent.FolderPath);
                }
            }
            if (logLevel >= 5)
            {
                DebugLog.WriteToLog("Comparison of folders successfully done, " + FoldersToCreate.Count + " new folders were created since original backup.");
                DebugLog.WriteToLog("Creating list of folders that no longer exists...");
            }

            foreach (FolderObject itemOriginal in OriginalFoldersObjects)
            {
                if (!itemOriginal.Paired)
                {
                    FoldersToDelete.Add(source + itemOriginal.FolderPath);
                }
            }
            if (logLevel >= 5)
                DebugLog.WriteToLog("There is " + FoldersToDelete.Count + " folders that were deleted since the original backup.");

            // M * delete:

            if (logLevel >= 5)
                DebugLog.WriteToLog("Creating list of files that no longer exists...");

            foreach (FileInfoObject itemOriginal in OriginalFiles)
            {
                if (!itemOriginal.Paired)
                {
                    FilesToDelete.Add(source + itemOriginal.RelativePathName);
                }
            }
            if (logLevel >= 5)
                DebugLog.WriteToLog("There is " + FilesToDelete.Count + " files that were deleted since the original backup.");

            if (logLevel >= 5)
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
            if (logLevel >= 5)
                DebugLog.WriteToLog("Backup of folder structure is done, " + this.FoldersCorrect.Count + " folders were successfully created, but it was unable to create " + this.FoldersError.Count + " folders");






            if (logLevel >= 4)
                DebugLog.WriteToLog("Backuping new or modified files to " + destinationInfo.FullName);

            FileInfo tmpFileInfo;
            for (int i = 0; i < FilesToCopy.Count; i++)
            {
                tmpFileInfo = new FileInfo(source + FilesToCopy[i]);
                try
                {
                    tmpFileInfo.CopyTo(destinationInfo.FullName + FilesToCopy[i]);
                    FilesCorrect.Add(new FileInfoObject { RelativePathName = tmpFileInfo.FullName.Remove(0, sourceInfo.FullName.Length), Length = tmpFileInfo.Length, CreationTimeUtc = tmpFileInfo.CreationTimeUtc, LastWriteTimeUtc = tmpFileInfo.LastWriteTimeUtc, Attributes = tmpFileInfo.Attributes.ToString(), MD5 = this.CalculateMD5(tmpFileInfo.FullName) });
                }
                catch (Exception x)
                {
                    this.FilesError.Add(tmpFileInfo.FullName);
                }

            }
            if (logLevel >= 5)
            {
                DebugLog.WriteToLog("File backup is done, " + this.FilesCorrect.Count + " files were successfully copied, but it was unable to copy " + this.FilesError.Count + " files");
                DebugLog.WriteToLog("Creating transaction log of successfully copied files and folders...");
            }
            BackupLog.CreateBackupJournal(FilesCorrect, sourceInfo.FullName, destinationInfo.Parent.FullName + @"\KoFrMaBackup.dat",FoldersCorrect);
            if (logLevel >= 5)
                DebugLog.WriteToLog("Transaction log successfully created in destination "+ destinationInfo.Parent.FullName + @"\KoFrMaBackup.dat");
            BackupLog = null;
            if (logLevel >= 4)
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
                        if(Copy)
                        this.CopyDirectoryRecursivly(item, to.CreateSubdirectory(item.Name), Copy);
                        else
                        this.CopyDirectoryRecursivly(item, null, Copy);
                        this.FoldersCorrect.Add(item.FullName.Remove(0, sourceInfo.FullName.Length));
                    }
                    catch (Exception x)
                    {
                        this.FoldersError.Add(item.FullName);
                    }
                    
                }
        }


        private string CalculateMD5(string filename)
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

        private List<FileInfoObject> ReturnHashCodes (List<FileInfoObject> listWithoutHashCodes)
        {
            List<FileInfoObject> tmpList;

            tmpList = listWithoutHashCodes;
            string tmp;
            foreach (FileInfoObject item in tmpList)
            {
                tmp = item.RelativePathName + '|' + item.Length.ToString() + '|' + item.CreationTimeUtc.ToBinary().ToString() + '|' + item.LastWriteTimeUtc.ToBinary().ToString() + '|' + item.Attributes.ToString() + '|' + item.MD5;
                item.HashRow = tmp.GetHashCode();
            }

        return tmpList;
        }



    }
}
