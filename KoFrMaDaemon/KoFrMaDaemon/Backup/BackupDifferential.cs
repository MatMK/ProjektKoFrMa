using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace KoFrMaDaemon.Backup
{
    public class BackupDifferential:BackupSwitch
    {
        public List<FileInfoObject> FilesCorrect;
        public List<FolderObject> FoldersCorrect;
        public List<CopyErrorObject> FilesErrorLoad;
        public List<CopyErrorObject> FilesErrorCopy;
        public List<CopyErrorObject> FoldersErrorLoad;
        public List<CopyErrorObject> FoldersErrorCopy;

        public BackupDifferential()
        {
            FilesCorrect = new List<FileInfoObject>(100);
            FoldersCorrect = new List<FolderObject>(100);
            FilesErrorLoad = new List<CopyErrorObject>(100);
            FilesErrorCopy = new List<CopyErrorObject>(100);
            FoldersErrorLoad = new List<CopyErrorObject>(100);
            FoldersErrorCopy = new List<CopyErrorObject>(100);
        }


        public void BackupDifferentialProcess(BackupJournalObject backupJournalSource, string destination, DebugLog serviceDebugLog)
        {
            DateTime timeOfBackup = DateTime.Now;

            string temporaryDebugInfo = "";
            if (serviceDebugLog._logLevel >= 4)
                temporaryDebugInfo = "Starting the differential/incremental backup in " + timeOfBackup.ToString();

            base.destinationInfo = new DirectoryInfo(destination).CreateSubdirectory("KoFrMaBackup_" + String.Format("{0:yyyy_MM_dd_HH_mm_ss}", timeOfBackup)).CreateSubdirectory("KoFrMaBackup");

            serviceDebugLog.WriteToLog("Log of including operations is located in " + base.destinationInfo.Parent.FullName + @"\" + "KoFrMaDebug.log", 4);

            DebugLog DebugLog = new DebugLog(base.destinationInfo.Parent.FullName + @"\" + "KoFrMaDebug.log", serviceDebugLog._logLevel);

            DebugLog.WriteToLog("Subdirectory for the backup was created at " + base.destinationInfo.FullName, 5);

            DebugLog.WriteToLog(temporaryDebugInfo, 4);
            temporaryDebugInfo = null;


            DebugLog.WriteToLog("Loading journal of original backup from backup journal received from server...", 5);
            BackupJournalOperations BackupJournal = new BackupJournalOperations();
            //BackupJournalObject backupJournalObject = BackupJournal.LoadBackupJournalObject(OriginalBackupDatFilePath, DebugLog);
            BackupJournalObject backupJournalObject = backupJournalSource;
            string source = backupJournalObject.RelativePath;
            List<FileInfoObject> OriginalFiles = backupJournalObject.BackupJournalFiles;
            List<FolderObject> OriginalFolders = backupJournalObject.BackupJournalFolders;
            DebugLog.WriteToLog("List of original files and folders loaded, containing " + OriginalFiles.Count + " files and " + OriginalFolders.Count + " folders.", 5);


            DebugLog.WriteToLog("Creating list of current files and folders...", 5);
            base.sourceInfo = new DirectoryInfo(source);
            BackupJournalObject currentJournalObject = this.JournalCurrent(base.sourceInfo);
            DebugLog.WriteToLog("List of current files and folders loaded, containing " + currentJournalObject.BackupJournalFiles.Count + " files and " + currentJournalObject.BackupJournalFolders.Count + " folders.", 5);

            DebugLog.WriteToLog("Adding hash column to lists of current files and folders...", 7);
            List<FileInfoObject> CurrentFiles = BackupJournal.ReturnHashCodesFiles(currentJournalObject.BackupJournalFiles);
            List<FolderObject> CurrentFolders = BackupJournal.ReturnHashCodesFolders(currentJournalObject.BackupJournalFolders);


            DebugLog.WriteToLog("List of current files and folders successfully created, containing " + CurrentFiles.Count + " files and " + CurrentFolders.Count + " folders. Unable to load " + FilesErrorLoad.Count + " files and " + FoldersErrorLoad.Count + " folders.", 5);


            //if (serviceDebugLog._logLevel >= 7)
            //{
            //    DebugLog.WriteToLog("Writing journal of current files and folders (that will be compared to original journal later) into .dat file for debug purposes...", 7);
            //    BackupJournal.CreateBackupJournal(new BackupJournalObject() { BackupJournalFiles = CurrentFiles, BackupJournalFolders = CurrentFolders, RelativePath = source }, base.destinationInfo.Parent.FullName + @"\KoFrMaBackupDebugCurrentFiles.dat", DebugLog);
            //}






            DebugLog.WriteToLog("Comparing list of current files to original source of the backup...", 5);

            List<string> FilesToCopy = new List<string>(CurrentFiles.Count / 4);
            List<string> FilesToDelete = new List<string>(CurrentFiles.Count / 4);
            List<string> FoldersToCreate = new List<string>(CurrentFolders.Count / 8);
            List<string> FoldersToDelete = new List<string>(CurrentFolders.Count / 8);

            bool sameObject;

            foreach (FileInfoObject itemCurrent in CurrentFiles)
            {
                sameObject = false;
                foreach (FileInfoObject itemOriginal in OriginalFiles)
                {

                    if (itemCurrent.HashRow == itemOriginal.HashRow)
                    {
                        if (itemCurrent.RelativePath == itemOriginal.RelativePath
                            && itemCurrent.Length == itemOriginal.Length
                            && itemCurrent.CreationTimeUtc == itemOriginal.CreationTimeUtc
                            && itemCurrent.LastWriteTimeUtc == itemOriginal.LastWriteTimeUtc
                            && itemCurrent.MD5 == itemOriginal.MD5
                            && itemCurrent.Attributes == itemOriginal.Attributes)
                        {
                            sameObject = true;
                            itemOriginal.Paired = true;

                            if (DebugLog._logLevel >= 9)
                                DebugLog.WriteToLog("Same object = true", 9);

                            break;
                        }

                        else if (DebugLog._logLevel >= 8)
                        {
                            if (itemCurrent.RelativePath != itemOriginal.RelativePath)
                            {
                                DebugLog.WriteToLog("RelativePathName Error: " + itemCurrent.RelativePath + " is not " + itemOriginal.RelativePath, 8);
                            }
                            if (itemCurrent.Length != itemOriginal.Length)
                            {
                                DebugLog.WriteToLog("Length Error: " + itemCurrent.Length.ToString() + " is not " + itemOriginal.Length.ToString(), 8);
                            }
                            if (itemCurrent.CreationTimeUtc != itemOriginal.CreationTimeUtc)
                            {
                                DebugLog.WriteToLog("CreationTimeUtc Error: " + itemCurrent.CreationTimeUtc.ToString() + " is not " + itemOriginal.CreationTimeUtc.ToString(), 8);
                            }
                            if (itemCurrent.LastWriteTimeUtc != itemOriginal.LastWriteTimeUtc)
                            {
                                DebugLog.WriteToLog("LastWriteTimeUtc Error: " + itemCurrent.LastWriteTimeUtc.ToString() + " is not " + itemOriginal.LastWriteTimeUtc.ToString(), 8);
                            }
                            if (itemCurrent.MD5 != itemOriginal.MD5)
                            {
                                DebugLog.WriteToLog("MD5 Error: " + itemCurrent.MD5 + " is not " + itemOriginal.MD5, 8);
                            }
                            if (itemCurrent.Attributes != itemOriginal.Attributes)
                            {
                                DebugLog.WriteToLog("Attributes Error: " + itemCurrent.Attributes + " is not " + itemOriginal.Attributes, 8);
                            }
                        }
                    }
                    //else if (DebugLog._logLevel >= 10)
                    //    DebugLog.WriteToLog("HashRow Error: " + itemCurrent.HashRow.ToString() + " is not " + itemOriginal.HashRow.ToString(), 10);

                }
                if (!sameObject)
                {
                    FilesToCopy.Add(itemCurrent.RelativePath);
                }
            }
            DebugLog.WriteToLog("Comparison of files successfully done, " + FilesToCopy.Count + " files were created or modified since original backup.", 5);

            DebugLog.WriteToLog("Creating list of files that were changed or no longer exists...", 5);
            foreach (FileInfoObject itemOriginal in OriginalFiles)
            {
                if (!itemOriginal.Paired)
                {
                    FilesToDelete.Add(source + itemOriginal.RelativePath);
                }
            }
            DebugLog.WriteToLog("There is " + FilesToDelete.Count + " files that needs to be deleted since the original backup.", 5);





            DebugLog.WriteToLog("Comparing list of current folders to original source of backup...", 5);
            foreach (FolderObject itemCurrent in CurrentFolders)
            {
                sameObject = false;
                foreach (FolderObject itemOriginal in OriginalFolders)
                {

                    if (itemCurrent.HashRow == itemOriginal.HashRow)
                    {
                        if (itemCurrent.RelativePath == itemOriginal.RelativePath
                            && itemCurrent.CreationTimeUtc == itemOriginal.CreationTimeUtc
                            && itemCurrent.LastWriteTimeUtc == itemOriginal.LastWriteTimeUtc
                            && itemCurrent.Attributes == itemOriginal.Attributes)
                        {
                            sameObject = true;
                            itemOriginal.Paired = true;

                            if (DebugLog._logLevel >= 9)
                                DebugLog.WriteToLog("Same object = true", 9);

                            break;
                        }

                        else if (DebugLog._logLevel >= 8)
                        {
                            if (itemCurrent.RelativePath != itemOriginal.RelativePath)
                            {
                                DebugLog.WriteToLog("FolderPath Error: " + itemCurrent.RelativePath + " is not " + itemOriginal.RelativePath, 8);
                            }
                            if (itemCurrent.CreationTimeUtc != itemOriginal.CreationTimeUtc)
                            {
                                DebugLog.WriteToLog("CreationTimeUtc Error: " + itemCurrent.CreationTimeUtc.ToString() + " is not " + itemOriginal.CreationTimeUtc.ToString(), 8);
                            }
                            if (itemCurrent.LastWriteTimeUtc != itemOriginal.LastWriteTimeUtc)
                            {
                                DebugLog.WriteToLog("LastWriteTimeUtc Error: " + itemCurrent.LastWriteTimeUtc.ToString() + " is not " + itemOriginal.LastWriteTimeUtc.ToString(), 8);
                            }
                            if (itemCurrent.Attributes != itemOriginal.Attributes)
                            {
                                DebugLog.WriteToLog("Attributes Error: " + itemCurrent.Attributes + " is not " + itemOriginal.Attributes, 8);
                            }
                        }
                    }
                    //else if (DebugLog._logLevel >= 10)
                    //    DebugLog.WriteToLog("HashRow Error: " + itemCurrent.HashRow.ToString() + " is not " + itemOriginal.HashRow.ToString(),10);

                }
                if (!sameObject)
                {
                    FoldersToCreate.Add(itemCurrent.RelativePath);
                }
            }
            DebugLog.WriteToLog("Comparison of folders successfully done, " + FoldersToCreate.Count + " folders were created or changed since the original backup.", 5);



            DebugLog.WriteToLog("Creating list of folders that were modified or no longer exists...", 5);
            foreach (FolderObject itemOriginal in OriginalFolders)
            {
                if (!itemOriginal.Paired)
                {
                    FoldersToDelete.Add(source + itemOriginal.RelativePath);
                    //in FoldersToDelete při obnově mazat POUZE prázdné!!
                }
            }
            DebugLog.WriteToLog("There is " + FoldersToDelete.Count + " folders that needs to be deleted since the original backup.", 5);






            DebugLog.WriteToLog("Starting to backup files and folders to " + base.destinationInfo.FullName + @"\", 4);

            DebugLog.WriteToLog("Backuping modified folder structure...", 5);
            FilesCorrect = new List<FileInfoObject>();
            FoldersCorrect = new List<FolderObject>();
            DirectoryInfo tmpDirectoryInfo;
            for (int i = 0; i < FoldersToCreate.Count; i++)
            {
                tmpDirectoryInfo = new DirectoryInfo(base.destinationInfo.FullName + @"\" + FoldersToCreate[i]);
                try
                {

                    tmpDirectoryInfo.Create();
                    this.FoldersCorrect.Add(new FolderObject() { RelativePath = tmpDirectoryInfo.FullName.Remove(0, base.sourceInfo.FullName.Length), CreationTimeUtc = tmpDirectoryInfo.CreationTimeUtc, LastWriteTimeUtc = tmpDirectoryInfo.LastWriteTimeUtc, Attributes = tmpDirectoryInfo.Attributes.ToString() });
                }
                catch (Exception ex)
                {
                    this.FoldersErrorCopy.Add(new CopyErrorObject() {FullPath = tmpDirectoryInfo.FullName,ExceptionMessage = ex.Message });
                }
            }
            DebugLog.WriteToLog("Backup of folder structure is done, " + this.FoldersCorrect.Count + " folders were successfully created, it was unable to create " + this.FoldersErrorCopy.Count + " folders", 5);







            DebugLog.WriteToLog("Backuping new or modified files...", 5);
            FileInfo tmpFileInfo;

            for (int i = 0; i < FilesToCopy.Count; i++)
            {
                tmpFileInfo = new FileInfo(source + FilesToCopy[i]);
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(base.destinationInfo.FullName + @"\" + FilesToCopy[i]));
                    tmpFileInfo.CopyTo(base.destinationInfo.FullName + @"\" + FilesToCopy[i]);
                    FilesCorrect.Add(new FileInfoObject { RelativePath = tmpFileInfo.FullName.Remove(0, base.sourceInfo.FullName.Length), Length = tmpFileInfo.Length, CreationTimeUtc = tmpFileInfo.CreationTimeUtc, LastWriteTimeUtc = tmpFileInfo.LastWriteTimeUtc, Attributes = tmpFileInfo.Attributes.ToString(), MD5 = this.CalculateMD5(tmpFileInfo.FullName) });
                }
                catch (Exception ex)
                {
                    this.FilesErrorCopy.Add(new CopyErrorObject() {FullPath = tmpFileInfo.FullName,ExceptionMessage = ex.Message });
                    DebugLog.WriteToLog("Unable to copy " + tmpFileInfo.FullName + " to " + base.destinationInfo.FullName + @"\" + FilesToCopy[i] + " because of exception " + ex.Message + ". Path to destination folder: " + tmpFileInfo.Directory.FullName, 8);
                }

            }


            DebugLog.WriteToLog("File backup is done, " + this.FilesCorrect.Count + " files were successfully copied, it was unable to copy " + this.FilesErrorCopy.Count + " files", 5);


            DebugLog.WriteToLog("Creating transaction log of successfully copied files and folders...", 5);
            BackupJournalNew = new BackupJournalObject()
            {
                RelativePath = source,
                BackupJournalFiles = FilesCorrect,
                BackupJournalFolders = FoldersCorrect
            };
            BackupJournal.CreateBackupJournal(BackupJournalNew, base.destinationInfo.Parent.FullName + @"\KoFrMaBackup.dat", DebugLog);
            DebugLog.WriteToLog("Transaction log successfully created in destination " + base.destinationInfo.Parent.FullName + @"\KoFrMaBackup.dat", 5);



            TimeSpan backupTook = DateTime.Now - timeOfBackup;
            DebugLog.WriteToLog("Differential/Incremental backup done in " + backupTook.TotalSeconds + " s", 4);

        }

        private BackupJournalObject JournalCurrent(DirectoryInfo path)
        {
            BackupJournalObject journalObject = new BackupJournalObject();

            List<FileInfoObject> FileList = new List<FileInfoObject>();

            List<FolderObject> FolderList = new List<FolderObject>();

            ExploreDirectoryRecursively(path, FileList, FolderList);

            journalObject.BackupJournalFiles = FileList;
            journalObject.BackupJournalFolders = FolderList;
            return journalObject;

        }

        private void ExploreDirectoryRecursively(DirectoryInfo path, List<FileInfoObject> FileList, List<FolderObject> FolderList)
        {
            foreach (FileInfo item in path.GetFiles())
            {
                try
                {
                    FileList.Add(new FileInfoObject { RelativePath = item.FullName.Remove(0, base.sourceInfo.FullName.Length), Length = item.Length, CreationTimeUtc = item.CreationTimeUtc, LastWriteTimeUtc = item.LastWriteTimeUtc, Attributes = item.Attributes.ToString(), MD5 = this.CalculateMD5(item.FullName) });
                }
                catch (Exception ex)
                {
                    this.FilesErrorLoad.Add(new CopyErrorObject() { FullPath = item.FullName, ExceptionMessage = ex.Message });
                }

            }

            foreach (DirectoryInfo item in path.GetDirectories())
            {
                try
                {
                    this.ExploreDirectoryRecursively(item, FileList, FolderList);
                    FolderList.Add(new FolderObject() { RelativePath = item.FullName.Remove(0, base.sourceInfo.FullName.Length), CreationTimeUtc = item.CreationTimeUtc, LastWriteTimeUtc = item.LastWriteTimeUtc, Attributes = item.Attributes.ToString() });
                }
                catch (Exception ex)
                {
                    this.FoldersErrorLoad.Add(new CopyErrorObject() { FullPath = item.FullName, ExceptionMessage = ex.Message });
                }

            }
        }
    }
}
