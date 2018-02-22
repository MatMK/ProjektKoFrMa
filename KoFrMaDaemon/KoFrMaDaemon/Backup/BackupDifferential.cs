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
        public List<FileInfoObject> FilesCorrect = new List<FileInfoObject>(100);
        public List<FolderObject> FoldersCorrect = new List<FolderObject>(100);
        public List<String> FilesErrorLoad = new List<string>(100);
        public List<String> FilesErrorCopy = new List<string>(100);
        public List<String> FoldersErrorLoad = new List<string>(100);
        public List<String> FoldersErrorCopy = new List<string>(100);

        public void BackupDifferentialProcess(string OriginalBackupDatFilePath, string destination, DebugLog serviceDebugLog)
        {
            DateTime timeOfBackup = DateTime.Now;

            string temporaryDebugInfo = "";
            if (serviceDebugLog._logLevel >= 4)
                temporaryDebugInfo = "Starting the differential/incremental backup in " + timeOfBackup.ToString();

            destinationInfo = new DirectoryInfo(destination).CreateSubdirectory("KoFrMaBackup_" + String.Format("{0:yyyy_MM_dd_HH_mm_ss}", timeOfBackup)).CreateSubdirectory("KoFrMaBackup");

            serviceDebugLog.WriteToLog("Log of including operations is located in " + destinationInfo.Parent.FullName + @"\" + "KoFrMaDebug.log", 4);

            DebugLog DebugLog = new DebugLog(destinationInfo.Parent.FullName + @"\" + "KoFrMaDebug.log", serviceDebugLog._logLevel);

            DebugLog.WriteToLog("Subdirectory for the backup was created at " + destinationInfo.FullName, 5);

            DebugLog.WriteToLog(temporaryDebugInfo, 4);
            temporaryDebugInfo = null;


            DebugLog.WriteToLog("Loading journal of original backup from " + OriginalBackupDatFilePath, 5);
            BackupJournalOperations BackupJournal = new BackupJournalOperations();
            BackupJournalObject backupJournalObject = BackupJournal.LoadBackupJournalObject(OriginalBackupDatFilePath, DebugLog);
            string source = backupJournalObject.RelativePath;
            List<FileInfoObject> OriginalFiles = backupJournalObject.BackupJournalFiles;
            List<FolderObject> OriginalFolders = backupJournalObject.BackupJournalFolders;
            DebugLog.WriteToLog("List of original files and folders loaded, containing " + OriginalFiles.Count + " files and " + OriginalFolders.Count + " folders.", 5);


            DebugLog.WriteToLog("Creating list of current files and folders...", 5);
            sourceInfo = new DirectoryInfo(source);
            //this.CopyDirectoryRecursivly(sourceInfo, null, false);
            BackupJournalObject currentJournalObject = JournalCurrent(sourceInfo);


            DebugLog.WriteToLog("Adding hash column to lists of current files and folders...", 7);
            List<FileInfoObject> CurrentFiles = BackupJournal.ReturnHashCodesFiles(currentJournalObject.BackupJournalFiles);
            List<FolderObject> CurrentFolders = BackupJournal.ReturnHashCodesFolders(currentJournalObject.BackupJournalFolders);


            DebugLog.WriteToLog("List of current files and folders successfully created, containing " + CurrentFiles.Count + " files and " + CurrentFolders.Count + " folders. Unable to load " + FilesErrorLoad.Count + " files and " + FoldersErrorLoad.Count + " folders.", 5);


            if (serviceDebugLog._logLevel >= 6)
            {
                DebugLog.WriteToLog("Writing journal of current files and folders (that will be compared to original journal later) into .dat file for debug purposes...", 6);
                BackupJournal.CreateBackupJournal(new BackupJournalObject() { BackupJournalFiles = CurrentFiles, BackupJournalFolders = CurrentFolders, RelativePath = source }, destinationInfo.Parent.FullName + @"\KoFrMaBackupDebugCurrentFiles.dat", DebugLog);
            }






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
                        if (itemCurrent.RelativePathName == itemOriginal.RelativePathName
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
                            if (itemCurrent.RelativePathName != itemOriginal.RelativePathName)
                            {
                                DebugLog.WriteToLog("RelativePathName Error: " + itemCurrent.RelativePathName + " is not " + itemOriginal.RelativePathName, 8);
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
                    FilesToCopy.Add(itemCurrent.RelativePathName);
                }
            }
            DebugLog.WriteToLog("Comparison of files successfully done, " + FilesToCopy.Count + " files were created or modified since original backup.", 5);

            DebugLog.WriteToLog("Creating list of files that were changed or no longer exists...", 5);
            foreach (FileInfoObject itemOriginal in OriginalFiles)
            {
                if (!itemOriginal.Paired)
                {
                    //možná předělat, ukazuje včetně upravených souborů
                    FilesToDelete.Add(source + itemOriginal.RelativePathName);
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
                        if (itemCurrent.FolderPath == itemOriginal.FolderPath
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
                            if (itemCurrent.FolderPath != itemOriginal.FolderPath)
                            {
                                DebugLog.WriteToLog("FolderPath Error: " + itemCurrent.FolderPath + " is not " + itemOriginal.FolderPath, 8);
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
                    FoldersToCreate.Add(itemCurrent.FolderPath);
                }
            }
            DebugLog.WriteToLog("Comparison of folders successfully done, " + FoldersToCreate.Count + " folders were created or changed since the original backup.", 5);



            DebugLog.WriteToLog("Creating list of folders that were modified or no longer exists...", 5);
            foreach (FolderObject itemOriginal in OriginalFolders)
            {
                if (!itemOriginal.Paired)
                {
                    FoldersToDelete.Add(source + itemOriginal.FolderPath);
                    //in FoldersToDelete při obnově mazat POUZE prázdné!!
                }
            }
            DebugLog.WriteToLog("There is " + FoldersToDelete.Count + " folders that needs to be deleted since the original backup.", 5);






            DebugLog.WriteToLog("Starting to backup files and folders to " + destinationInfo.FullName + @"\", 4);

            DebugLog.WriteToLog("Backuping modified folder structure...", 5);
            FilesCorrect = new List<FileInfoObject>();
            FoldersCorrect = new List<FolderObject>();
            FilesErrorCopy = new List<string>();
            FoldersErrorCopy = new List<string>();
            DirectoryInfo tmpDirectoryInfo;
            for (int i = 0; i < FoldersToCreate.Count; i++)
            {
                tmpDirectoryInfo = new DirectoryInfo(destinationInfo.FullName + @"\" + FoldersToCreate[i]);
                try
                {

                    tmpDirectoryInfo.Create();
                    this.FoldersCorrect.Add(new FolderObject() { FolderPath = tmpDirectoryInfo.FullName.Remove(0, sourceInfo.FullName.Length), CreationTimeUtc = tmpDirectoryInfo.CreationTimeUtc, LastWriteTimeUtc = tmpDirectoryInfo.LastWriteTimeUtc, Attributes = tmpDirectoryInfo.Attributes.ToString() });
                }
                catch (Exception x)
                {
                    this.FoldersErrorCopy.Add(tmpDirectoryInfo.FullName);
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
                    Directory.CreateDirectory(Path.GetDirectoryName(FilesToCopy[i]));
                    tmpFileInfo.CopyTo(destinationInfo.FullName + @"\" + FilesToCopy[i]);
                    FilesCorrect.Add(new FileInfoObject { RelativePathName = tmpFileInfo.FullName.Remove(0, sourceInfo.FullName.Length), Length = tmpFileInfo.Length, CreationTimeUtc = tmpFileInfo.CreationTimeUtc, LastWriteTimeUtc = tmpFileInfo.LastWriteTimeUtc, Attributes = tmpFileInfo.Attributes.ToString(), MD5 = this.CalculateMD5(tmpFileInfo.FullName) });
                }
                catch (Exception ex)
                {
                    this.FilesErrorCopy.Add(tmpFileInfo.FullName);
                    DebugLog.WriteToLog("Unable to copy " + tmpFileInfo.FullName + " to " + destinationInfo.FullName + @"\" + FilesToCopy[i] + " because of exception " + ex.Message + ". Path to destination folder: " + tmpFileInfo.Directory.FullName, 8);
                }

            }


            DebugLog.WriteToLog("File backup is done, " + this.FilesCorrect.Count + " files were successfully copied, it was unable to copy " + this.FilesErrorCopy.Count + " files", 5);


            DebugLog.WriteToLog("Creating transaction log of successfully copied files and folders...", 5);
            BackupJournal.CreateBackupJournal(new BackupJournalObject() { RelativePath = source, BackupJournalFiles = FilesCorrect, BackupJournalFolders = FoldersCorrect }, destinationInfo.Parent.FullName + @"\KoFrMaBackup.dat", DebugLog);
            DebugLog.WriteToLog("Transaction log successfully created in destination " + destinationInfo.Parent.FullName + @"\KoFrMaBackup.dat", 5);



            TimeSpan backupTook = DateTime.Now - timeOfBackup;
            DebugLog.WriteToLog("Differential/Incremental backup done in " + backupTook.TotalSeconds + " s", 4);

        }

        private BackupJournalObject JournalCurrent(DirectoryInfo path)
        {
            BackupJournalObject journalObject = new BackupJournalObject();

            List<FileInfoObject> FileList = new List<FileInfoObject>();

            List<FolderObject> FolderList = new List<FolderObject>();

            ExploreDirectoryRecursively(path, FileList, FolderList);

            return journalObject;

        }

        private void ExploreDirectoryRecursively(DirectoryInfo path, List<FileInfoObject> FileList, List<FolderObject> FolderList)
        {
            foreach (FileInfo item in path.GetFiles())
            {
                try
                {
                    FileList.Add(new FileInfoObject { RelativePathName = item.FullName.Remove(0, sourceInfo.FullName.Length), Length = item.Length, CreationTimeUtc = item.CreationTimeUtc, LastWriteTimeUtc = item.LastWriteTimeUtc, Attributes = item.Attributes.ToString(), MD5 = this.CalculateMD5(item.FullName) });
                    //this.FilesCorrect.Add(item.DirectoryName + '|' + item.FullName + '|' +  item.Length.ToString() + '|' + item.CreationTimeUtc.ToString() + '|' + item.LastWriteTimeUtc.ToString() + '|' + item.LastAccessTimeUtc.ToString() + '|' + item.Attributes.ToString() + '|' + this.CalculateMD5(item.FullName));
                }
                catch (Exception x)
                {
                    this.FilesErrorLoad.Add(item.FullName);
                }

            }

            foreach (DirectoryInfo item in path.GetDirectories())
            {
                try
                {
                    this.ExploreDirectoryRecursively(item, FileList, FolderList);
                    //this.FoldersCorrect.Add(item.FullName.Remove(0, sourceInfo.FullName.Length));
                    FolderList.Add(new FolderObject() { FolderPath = item.FullName.Remove(0, sourceInfo.FullName.Length), CreationTimeUtc = item.CreationTimeUtc, LastWriteTimeUtc = item.LastWriteTimeUtc, Attributes = item.Attributes.ToString() });
                }
                catch (Exception x)
                {
                    this.FoldersErrorLoad.Add(item.FullName);
                }

            }
        }
    }
}
