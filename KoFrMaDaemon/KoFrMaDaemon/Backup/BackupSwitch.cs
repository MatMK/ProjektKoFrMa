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

        private Task task = null;

        //private List<string> destinations;
        //private byte? compressionLevel;
        //private NetworkCredential networkCredential;
        private bool atLeastOneDestinationIsLocalFolder = false;



        public List<FileInfoObject> FilesCorrect;
        public List<FolderObject> FoldersCorrect;
        public List<CopyErrorObject> FilesErrorCopy;
        public List<CopyErrorObject> FoldersErrorCopy;

        public List<CopyErrorObject> FilesErrorLoad;
        public List<CopyErrorObject> FoldersErrorLoad;

        public void Backup(Task task)
        {
            this.task = task;
            taskDebugLog = new DebugLog(null, false, task.LogLevel);
            this.taskDebugLog.WriteToLog("Inicializing the backup instance...", 5);
            foreach (IDestination item in task.Destinations)
            {
                if (item is DestinationPlain)
                {
                    if (item.Path is DestinationPathLocal)
                    {
                        this.atLeastOneDestinationIsLocalFolder = true;
                        this.temporaryDestinationInfo = new DirectoryInfo(item.Path.Path);
                    }
                }
            }



            //this.networkCredential = networkCredential;
            //this.compressionLevel = compressionLevel;
            //this.destinations = task.Destinations;
            //for (int i = 0; i < destinations.Count; i++)
            //{


            //    //if (destinations[i].EndsWith(".zip")|| destinations[i].EndsWith(".7z")|| destinations[i].EndsWith(".rar"))
            //    //{

            //    //    //debugLog.WriteToLog("Starting backuping to archive, because the path to destination ends with .zip (" + destinations[i] + ')', 5);
            //    //    //Compression compression = new Compression(debugLog);
            //    //    //compression.CompressToZip(destinationInfo.FullName, destinationInfo.Parent.FullName + @"\" + this.destinationInfo.Name + ".zip", compressionLevel);
            //    //    //debugLog.WriteToLog("Compression done, deleting temporary files that were needed for compression", 6);
            //    //    //Directory.Delete(destinationInfo.FullName, true);
            //    //    //debugLog.WriteToLog("Files successfully deleted, compression is now completed.", 6);
            //    //}
            //    //else if (destinations[i].StartsWith("\\"))
            //    //{

            //    //}
            //    //else if (destinations[i].StartsWith("ftp://"))
            //    //{

            //    //}
            //    //else if (destinations[i].StartsWith("sftp://"))
            //    //{

            //    //}
            //    //else
            //    //{
            //    //    if (!atLeastOneDestinationIsLocalFolder)
            //    //    {
            //    //        this.temporaryDestinationInfo = new DirectoryInfo(destinations[i]);
            //    //        atLeastOneDestinationIsLocalFolder = true;
            //    //    }
            //    //}
            //    ////else
            //    ////{
            //    ////    debugLog.WriteToLog("Keeping the plain backup because the task doesn't want to archive", 5);
            //    ////}
            //    ////debugLog.WriteToLog("Backup done, ending the backup instance", 7);


            //}

            if (!atLeastOneDestinationIsLocalFolder)
            {
                ServiceKoFrMa.debugLog.WriteToLog("No destination points to local path, creating temporary folder at "+ Path.Combine(Path.GetTempPath(), "KoFrMaBackupTemp"), 5);
                Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "KoFrMaBackupTemp"));
                this.temporaryDestinationInfo = new DirectoryInfo(Path.Combine(Path.GetTempPath(),"KoFrMaBackupTemp"));
            }
            else
            {
                this.taskDebugLog.WriteToLog("At least one of destinations is plain local backup, there is no need to create temporary folder.", 6);
            }

            this.taskDebugLog.WriteToLog("Starting the backup...", 4);
            this.BackupToFolder(task, this.temporaryDestinationInfo);
            this.taskDebugLog.WriteToLog("Creating desired destination outputs...", 5);
            this.FinishBackup();
            this.taskDebugLog.WriteToLog("Backup done, ending the backup instance.", 4);
        }
        private void BackupToFolder(Task task, DirectoryInfo destination)
        {
            this.taskDebugLog.WriteToLog("Deciding what type of backup it is...", 7);
            if (task.BackupJournalSource != null)
            {
                this.taskDebugLog.WriteToLog("Starting differential/incremental backup, because journal was received from the server", 5);
                this.BackupDifferentialProcess(task, destination);
            }
            else
            {
                this.taskDebugLog.WriteToLog("Starting full backup, because the there is no info about backup journal", 5);
                this.BackupFullProcess(task, destination);
                //this.sourceInfo = backupFull.sourceInfo.Parent;
                //this.destinationInfo = backupFull.destinationInfo.Parent;
                //BackupJournalNew = backupFull.BackupJournalNew;
            }

        }
        public void BackupFullProcess(Task task,DirectoryInfo destination)
        {
            FilesCorrect = new List<FileInfoObject>(100);
            FoldersCorrect = new List<FolderObject>(100);
            FilesErrorCopy = new List<CopyErrorObject>(100);
            FoldersErrorCopy = new List<CopyErrorObject>(100);

            DateTime timeOfBackup = DateTime.Now;

            string temporaryDebugInfo = "";
            if (ServiceKoFrMa.debugLog._logLevel >= 4)
                temporaryDebugInfo = "Full backup started at  " + timeOfBackup.ToString();
            Directory.CreateDirectory(destination.FullName + @"\KoFrMaBackup_" + String.Format("{0:yyyy_MM_dd_HH_mm_ss}", timeOfBackup) + @"_Full\KoFrMaBackup");
            destinationInfo = new DirectoryInfo(destination + @"\KoFrMaBackup_" + String.Format("{0:yyyy_MM_dd_HH_mm_ss}", timeOfBackup) + @"_Full\KoFrMaBackup");

            ServiceKoFrMa.debugLog.WriteToLog("Log of including operations is located in " + destinationInfo.Parent.FullName + @"\KoFrMaDebug.log", 4);

            //this.taskDebugLog = new DebugLog(destinationInfo.Parent.FullName + @"\KoFrMaDebug.log", ServiceKoFrMa.debugLog.writeToWindowsEventLog, ServiceKoFrMa.debugLog._logLevel);

            this.taskDebugLog.WriteToLog("Subdirectory for the backup was created at " + destinationInfo.FullName, 5);

            this.taskDebugLog.WriteToLog(temporaryDebugInfo, 4);
            temporaryDebugInfo = null;

            sourceInfo = new DirectoryInfo(task.SourceOfBackup);

            if (!sourceInfo.Exists)
            {
                this.taskDebugLog.WriteToLog("Fatal Error: Cannot backup because source folder doesn't exists!", 2);
            }



            try
            {
                this.taskDebugLog.WriteToLog("Backuping now...", 4);
                List<FileInfoObject> FileList = new List<FileInfoObject>() ;
                List<FolderObject> FolderList= new List<FolderObject>();
                this.ExploreDirectoryRecursively(sourceInfo,FileList,FolderList);
                List<string> FileListString = new List<string>(FileList.Count);
                List<string> FolderListString = new List<string>(FolderList.Count);
                for (int i = 0; i < FileList.Count; i++)
                {
                    FileListString.Add(FileList[i].RelativePath);
                }
                for (int i = 0; i < FolderList.Count; i++)
                {
                    FolderListString.Add(FolderList[i].RelativePath);
                }
                this.BackupFileCopy(sourceInfo.FullName, FileListString, FolderListString, task.TemporaryFolderMaxBuffer);
                this.taskDebugLog.WriteToLog("Backup done, " + FilesCorrect.Count.ToString() + " files and " + FoldersCorrect.Count.ToString() + " folders successfully backuped, it was unable to backup " + FilesErrorCopy.Count + " files and " + FoldersErrorCopy.Count + " folders", 5);
            }
            catch (Exception x)
            {
                this.taskDebugLog.WriteToLog("Error " + x.Message + " occured and backup couldn't be fully done", 3);
            }

            this.taskDebugLog.WriteToLog("Creating transaction jounal of successfully backuped files and folders...", 5);
            BackupJournalOperations BackupJournal = new BackupJournalOperations();
            BackupJournalNew = new BackupJournalObject() { RelativePath = task.SourceOfBackup, BackupJournalFiles = FilesCorrect, BackupJournalFolders = FoldersCorrect };
            BackupJournal.CreateBackupJournal(BackupJournalNew, destinationInfo.Parent.FullName + @"\KoFrMaBackup.dat", task.IDTask, this.taskDebugLog);
            this.taskDebugLog.WriteToLog("Journal successfully created", 5);
            TimeSpan backupTook = DateTime.Now - timeOfBackup;
            this.taskDebugLog.WriteToLog("Full backup was completed in " + backupTook.TotalSeconds + " s", 4);
        }


        public void BackupDifferentialProcess(Task task,DirectoryInfo destination)
        {
            FilesCorrect = task.BackupJournalSource.BackupJournalFiles;
            FoldersCorrect = task.BackupJournalSource.BackupJournalFolders;
            List<string> FilesToDelete = task.BackupJournalSource.BackupJournalFilesDelete;
            List<string> FoldersToDelete = task.BackupJournalSource.BackupJournalFoldersDelete;
            FilesErrorLoad = new List<CopyErrorObject>(100);
            FilesErrorCopy = new List<CopyErrorObject>(100);
            FoldersErrorLoad = new List<CopyErrorObject>(100);
            FoldersErrorCopy = new List<CopyErrorObject>(100);

            DateTime timeOfBackup = DateTime.Now;

            string temporaryDebugInfo = "";
            if (ServiceKoFrMa.debugLog._logLevel >= 4)
                temporaryDebugInfo = "Starting the differential/incremental backup in " + timeOfBackup.ToString();

            destinationInfo = new DirectoryInfo(destination.FullName).CreateSubdirectory("KoFrMaBackup_" + String.Format("{0:yyyy_MM_dd_HH_mm_ss}", timeOfBackup)).CreateSubdirectory("KoFrMaBackup");

            ServiceKoFrMa.debugLog.WriteToLog("Log of including operations is located in " + destinationInfo.Parent.FullName + @"\" + "KoFrMaDebug.log", 4);

            //DebugLog DebugLog = new DebugLog(destinationInfo.Parent.FullName + @"\" + "KoFrMaDebug.log", ServiceKoFrMa.debugLog.writeToWindowsEventLog, ServiceKoFrMa.debugLog._logLevel);

            this.taskDebugLog.WriteToLog("Subdirectory for the backup was created at " + destinationInfo.FullName, 5);

            this.taskDebugLog.WriteToLog(temporaryDebugInfo, 4);
            temporaryDebugInfo = null;


            this.taskDebugLog.WriteToLog("Loading journal of original backup from backup journal received from server...", 5);
            BackupJournalOperations BackupJournal = new BackupJournalOperations();
            //BackupJournalObject backupJournalObject = BackupJournal.LoadBackupJournalObject(OriginalBackupDatFilePath, DebugLog);
            BackupJournalObject backupJournalObject = task.BackupJournalSource;
            string source = backupJournalObject.RelativePath;
            List<FileInfoObject> OriginalFiles = backupJournalObject.BackupJournalFiles;
            List<FolderObject> OriginalFolders = backupJournalObject.BackupJournalFolders;
            this.taskDebugLog.WriteToLog("List of original files and folders loaded, containing " + OriginalFiles.Count + " files and " + OriginalFolders.Count + " folders.", 5);


            this.taskDebugLog.WriteToLog("Creating list of current files and folders...", 5);
            sourceInfo = new DirectoryInfo(source);
            BackupJournalObject currentJournalObject = this.JournalCurrent(sourceInfo);
            this.taskDebugLog.WriteToLog("List of current files and folders loaded, containing " + currentJournalObject.BackupJournalFiles.Count + " files and " + currentJournalObject.BackupJournalFolders.Count + " folders.", 5);

            this.taskDebugLog.WriteToLog("Adding hash column to lists of current files and folders...", 7);
            List<FileInfoObject> CurrentFiles = BackupJournal.ReturnHashCodesFiles(currentJournalObject.BackupJournalFiles);
            List<FolderObject> CurrentFolders = BackupJournal.ReturnHashCodesFolders(currentJournalObject.BackupJournalFolders);


            this.taskDebugLog.WriteToLog("List of current files and folders successfully created, containing " + CurrentFiles.Count + " files and " + CurrentFolders.Count + " folders. Unable to load " + FilesErrorLoad.Count + " files and " + FoldersErrorLoad.Count + " folders.", 5);


            //if (serviceDebugLog._logLevel >= 7)
            //{
            //    DebugLog.WriteToLog("Writing journal of current files and folders (that will be compared to original journal later) into .dat file for debug purposes...", 7);
            //    BackupJournal.CreateBackupJournal(new BackupJournalObject() { BackupJournalFiles = CurrentFiles, BackupJournalFolders = CurrentFolders, RelativePath = source }, base.destinationInfo.Parent.FullName + @"\KoFrMaBackupDebugCurrentFiles.dat", DebugLog);
            //}






            this.taskDebugLog.WriteToLog("Comparing list of current files to original source of the backup...", 5);

            List<string> FilesToCopy = new List<string>(CurrentFiles.Count / 4);
            List<string> FoldersToCreate = new List<string>(CurrentFolders.Count / 8);


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

                            if (this.taskDebugLog._logLevel >= 9)
                                this.taskDebugLog.WriteToLog("Same object = true", 9);

                            break;
                        }

                        else if (this.taskDebugLog._logLevel >= 8)
                        {
                            if (itemCurrent.RelativePath != itemOriginal.RelativePath)
                            {
                                this.taskDebugLog.WriteToLog("RelativePathName Error: " + itemCurrent.RelativePath + " is not " + itemOriginal.RelativePath, 8);
                            }
                            if (itemCurrent.Length != itemOriginal.Length)
                            {
                                this.taskDebugLog.WriteToLog("Length Error: " + itemCurrent.Length.ToString() + " is not " + itemOriginal.Length.ToString(), 8);
                            }
                            if (itemCurrent.CreationTimeUtc != itemOriginal.CreationTimeUtc)
                            {
                                this.taskDebugLog.WriteToLog("CreationTimeUtc Error: " + itemCurrent.CreationTimeUtc.ToString() + " is not " + itemOriginal.CreationTimeUtc.ToString(), 8);
                            }
                            if (itemCurrent.LastWriteTimeUtc != itemOriginal.LastWriteTimeUtc)
                            {
                                this.taskDebugLog.WriteToLog("LastWriteTimeUtc Error: " + itemCurrent.LastWriteTimeUtc.ToString() + " is not " + itemOriginal.LastWriteTimeUtc.ToString(), 8);
                            }
                            if (itemCurrent.MD5 != itemOriginal.MD5)
                            {
                                this.taskDebugLog.WriteToLog("MD5 Error: " + itemCurrent.MD5 + " is not " + itemOriginal.MD5, 8);
                            }
                            if (itemCurrent.Attributes != itemOriginal.Attributes)
                            {
                                this.taskDebugLog.WriteToLog("Attributes Error: " + itemCurrent.Attributes + " is not " + itemOriginal.Attributes, 8);
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
            this.taskDebugLog.WriteToLog("Comparison of files successfully done, " + FilesToCopy.Count + " files were created or modified since original backup.", 5);

            this.taskDebugLog.WriteToLog("Creating list of files that were changed or no longer exists...", 5);
            foreach (FileInfoObject itemOriginal in OriginalFiles)
            {
                if (!itemOriginal.Paired)
                {
                    FilesToDelete.Add(itemOriginal.RelativePath);
                }
            }
            this.taskDebugLog.WriteToLog("There is " + (FilesToDelete.Count-task.BackupJournalSource.BackupJournalFilesDelete.Count) + " files that needs to be deleted since the original backup.", 5);





            this.taskDebugLog.WriteToLog("Comparing list of current folders to original source of backup...", 5);
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

                            if (this.taskDebugLog._logLevel >= 9)
                                this.taskDebugLog.WriteToLog("Same object = true", 9);

                            break;
                        }

                        else if (this.taskDebugLog._logLevel >= 8)
                        {
                            if (itemCurrent.RelativePath != itemOriginal.RelativePath)
                            {
                                this.taskDebugLog.WriteToLog("FolderPath Error: " + itemCurrent.RelativePath + " is not " + itemOriginal.RelativePath, 8);
                            }
                            if (itemCurrent.CreationTimeUtc != itemOriginal.CreationTimeUtc)
                            {
                                this.taskDebugLog.WriteToLog("CreationTimeUtc Error: " + itemCurrent.CreationTimeUtc.ToString() + " is not " + itemOriginal.CreationTimeUtc.ToString(), 8);
                            }
                            if (itemCurrent.LastWriteTimeUtc != itemOriginal.LastWriteTimeUtc)
                            {
                                this.taskDebugLog.WriteToLog("LastWriteTimeUtc Error: " + itemCurrent.LastWriteTimeUtc.ToString() + " is not " + itemOriginal.LastWriteTimeUtc.ToString(), 8);
                            }
                            if (itemCurrent.Attributes != itemOriginal.Attributes)
                            {
                                this.taskDebugLog.WriteToLog("Attributes Error: " + itemCurrent.Attributes + " is not " + itemOriginal.Attributes, 8);
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
            this.taskDebugLog.WriteToLog("Comparison of folders successfully done, " + FoldersToCreate.Count + " folders were created or changed since the original backup.", 5);



            this.taskDebugLog.WriteToLog("Creating list of folders that were modified or no longer exists...", 5);
            foreach (FolderObject itemOriginal in OriginalFolders)
            {
                if (!itemOriginal.Paired)
                {
                    FoldersToDelete.Add(itemOriginal.RelativePath);
                    //in FoldersToDelete při obnově mazat POUZE prázdné!!
                }
            }
            this.taskDebugLog.WriteToLog("There is " + (FoldersToDelete.Count - task.BackupJournalSource.BackupJournalFoldersDelete.Count) + " folders that needs to be deleted since the original backup.", 5);






            this.taskDebugLog.WriteToLog("Starting to backup files and folders to " + destinationInfo.FullName + @"\", 4);

            this.BackupFileCopy(source, FilesToCopy, FoldersToCreate, task.TemporaryFolderMaxBuffer);


            this.taskDebugLog.WriteToLog("Creating transaction log of successfully copied files and folders...", 5);
            BackupJournalNew = new BackupJournalObject()
            {
                RelativePath = source,
                BackupJournalFiles = FilesCorrect,
                BackupJournalFolders = FoldersCorrect,
                BackupJournalFilesDelete = FilesToDelete,
                BackupJournalFoldersDelete = FoldersToDelete
            };
            BackupJournal.CreateBackupJournal(BackupJournalNew, destinationInfo.Parent.FullName + @"\KoFrMaBackup.dat", task.IDTask, this.taskDebugLog);
            this.taskDebugLog.WriteToLog("Transaction log successfully created in destination " + destinationInfo.Parent.FullName + @"\KoFrMaBackup.dat", 5);



            TimeSpan backupTook = DateTime.Now - timeOfBackup;
            this.taskDebugLog.WriteToLog("Differential/Incremental backup done in " + backupTook.TotalSeconds + " s", 4);

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
                    FileList.Add(new FileInfoObject { RelativePath = item.FullName.Remove(0, sourceInfo.FullName.Length), Length = item.Length, CreationTimeUtc = item.CreationTimeUtc, LastWriteTimeUtc = item.LastWriteTimeUtc, Attributes = item.Attributes.ToString(), MD5 = this.CalculateMD5(item.FullName) });
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
                    FolderList.Add(new FolderObject() { RelativePath = item.FullName.Remove(0, sourceInfo.FullName.Length), CreationTimeUtc = item.CreationTimeUtc, LastWriteTimeUtc = item.LastWriteTimeUtc, Attributes = item.Attributes.ToString() });
                }
                catch (Exception ex)
                {
                    this.FoldersErrorLoad.Add(new CopyErrorObject() { FullPath = item.FullName, ExceptionMessage = ex.Message });
                }

            }
        }

        private void BackupFileCopy(string source, List<string> filesToCopy,List<string> foldersToCreate,int? bufferSize)
        {

            this.taskDebugLog.WriteToLog("Backuping modified folder structure...", 5);
            FilesCorrect = new List<FileInfoObject>();
            FoldersCorrect = new List<FolderObject>();
            DirectoryInfo tmpDirectoryInfo;
            for (int i = 0; i < foldersToCreate.Count; i++)
            {
                tmpDirectoryInfo = new DirectoryInfo(destinationInfo.FullName + @"\" + foldersToCreate[i]);
                try
                {

                    tmpDirectoryInfo.Create();
                    this.FoldersCorrect.Add(new FolderObject() { RelativePath = tmpDirectoryInfo.FullName.Remove(0, sourceInfo.FullName.Length), CreationTimeUtc = tmpDirectoryInfo.CreationTimeUtc, LastWriteTimeUtc = tmpDirectoryInfo.LastWriteTimeUtc, Attributes = tmpDirectoryInfo.Attributes.ToString() });
                }
                catch (Exception ex)
                {
                    this.FoldersErrorCopy.Add(new CopyErrorObject() { FullPath = tmpDirectoryInfo.FullName, ExceptionMessage = ex.Message });
                }
            }
            this.taskDebugLog.WriteToLog("Backup of folder structure is done, " + this.FoldersCorrect.Count + " folders were successfully created, it was unable to create " + this.FoldersErrorCopy.Count + " folders", 5);







            this.taskDebugLog.WriteToLog("Backuping new or modified files...", 5);
            FileInfo tmpFileInfo;
            Int64 currentSizeSum = 0;

            for (int i = 0; i < filesToCopy.Count; i++)
            {
                tmpFileInfo = new FileInfo(source + filesToCopy[i]);
                try
                {
                    if (bufferSize != null)
                    {
                        ServiceKoFrMa.debugLog.WriteToLog("Buffer is set, comparing its size with temp folder. Buffer value is "+bufferSize+"MB", 9);
                        if (currentSizeSum < bufferSize * 1000000)
                        {
                            ServiceKoFrMa.debugLog.WriteToLog("There is no need to flush buffer, copying the file. There is still"+ (bufferSize - (currentSizeSum/1000000))+" MB left until flushing.", 9);
                        }
                        else
                        {
                            ServiceKoFrMa.debugLog.WriteToLog("Flushing the buffer because the limit was exceedid by " + (bufferSize - (currentSizeSum / 1000000)) + " MB", 6);
                            currentSizeSum = 0;
                            this.FinishBackup();
                        }
                        currentSizeSum += tmpFileInfo.Length;
                    }
                    Directory.CreateDirectory(Path.GetDirectoryName(destinationInfo.FullName + @"\" + filesToCopy[i]));
                    tmpFileInfo.CopyTo(destinationInfo.FullName + @"\" + filesToCopy[i]);
                    FilesCorrect.Add(new FileInfoObject { RelativePath = tmpFileInfo.FullName.Remove(0, sourceInfo.FullName.Length), Length = tmpFileInfo.Length, CreationTimeUtc = tmpFileInfo.CreationTimeUtc, LastWriteTimeUtc = tmpFileInfo.LastWriteTimeUtc, Attributes = tmpFileInfo.Attributes.ToString(), MD5 = this.CalculateMD5(tmpFileInfo.FullName) });

                }
                catch (Exception ex)
                {
                    this.FilesErrorCopy.Add(new CopyErrorObject() { FullPath = tmpFileInfo.FullName, ExceptionMessage = ex.Message });
                    this.taskDebugLog.WriteToLog("Unable to copy " + tmpFileInfo.FullName + " to " + destinationInfo.FullName + @"\" + filesToCopy[i] + " because of exception " + ex.Message + ". Path to destination folder: " + tmpFileInfo.Directory.FullName, 8);
                }

            }


            this.taskDebugLog.WriteToLog("File backup is done, " + this.FilesCorrect.Count + " files were successfully copied, it was unable to copy " + this.FilesErrorCopy.Count + " files", 5);


        }

        private void FinishBackup()
        {

            for (int i = 0; i < task.Destinations.Count; i++)
            {
                this.CreateDestinationFormat(this.destinationInfo.Parent.FullName, task.Destinations[i]);
                //ServiceKoFrMa.debugLog.WriteToLog("Compression done, deleting temporary files that were needed for compression", 6);
                //ServiceKoFrMa.debugLog.WriteToLog("Files successfully deleted, compression is now completed.", 6);
            }
            //Directory.Delete(destinationInfo.FullName, true);
            if (!atLeastOneDestinationIsLocalFolder)
            {
                ServiceKoFrMa.debugLog.WriteToLog("Backup done, deleting temporary files at " + Path.Combine(Path.GetTempPath(), "KoFrMaBackupTemp"), 7);
                try
                {
                    Directory.Delete(Path.Combine(Path.GetTempPath(), "KoFrMaBackupTemp"), true);
                }
                catch (Exception ex)
                {
                    ServiceKoFrMa.debugLog.WriteToLog("Cannot delete files in temporary directory because of "+ex.Message, 3);
                }
                try
                {
                    List<FileInfoObject> FileList = new List<FileInfoObject>();
                    List<FolderObject> FolderList = new List<FolderObject>();
                    this.ExploreDirectoryRecursively(new DirectoryInfo(Path.Combine(Path.GetTempPath(), "KoFrMaBackupTemp")), FileList, FolderList);
                    for (int i = 0; i < FileList.Count; i++)
                    {
                        File.Delete(Path.Combine(Path.GetTempPath(), "KoFrMaBackupTemp") + @"\" + FileList[i].RelativePath);
                    }
                    for (int i = 0; i < FolderList.Count; i++)
                    {
                        Directory.Delete(Path.Combine(Path.GetTempPath(), "KoFrMaBackupTemp") + @"\" + FolderList[i].RelativePath, true);
                    }
                }
                catch (Exception ex)
                {
                    ServiceKoFrMa.debugLog.WriteToLog("Cannot delete files in temporary directory because of " + ex.Message, 3);
                }

            }
        }

        private void CreateDestinationFormat(string backupPath,IDestination destination)
        {
            if (destination is DestinationZip)
            {
                ServiceKoFrMa.debugLog.WriteToLog("Starting backuping to archive, because the path to destination ends with .zip (" + destination + ')', 5);
                DestinationZip destinationZip = (DestinationZip)destination;
                Compression compression = new Compression();
                if (destination.Path is DestinationPathLocal)
                {
                    compression.CompressToZip(backupPath, destination.Path.Path + @"\" + destinationInfo.Parent.Name + ".zip", destinationZip.CompressionLevel,destinationZip.SplitAfter);
                }
                else
                {
                    compression.CompressToZip(backupPath, temporaryDestinationInfo.FullName + @"\" + destinationInfo.Parent.Name + ".zip", destinationZip.CompressionLevel, destinationZip.SplitAfter);
                    this.CreateDestination(temporaryDestinationInfo.FullName + @"\" + destinationInfo.Parent.Name + ".zip", destination.Path);
                }

            }
            else if (destination is Destination7z destination7z)
            {
                Compression compression = new Compression();
                if (destination.Path is DestinationPathLocal)
                {
                    ServiceKoFrMa.debugLog.WriteToLog("Starting backuping to archive, because the path to destination ends with .7z (" + destination + ')', 5);
                    ServiceKoFrMa.debugLog.WriteToLog("Archive will be made from this folder " + backupPath + @"\ and put into this location " + destination.Path.Path + @"\" + destinationInfo.Parent.Name + ".7z", 7);
                    compression.CompressTo7z(ServiceKoFrMa.daemonSettings.SevenZipPath, backupPath + @"\", destination.Path.Path + @"\" + destinationInfo.Parent.Name + ".7z", destination7z.CompressionLevel, destination7z.SplitAfter);
                }
                else
                {
                    compression.CompressTo7z(ServiceKoFrMa.daemonSettings.SevenZipPath, backupPath + @"\", temporaryDestinationInfo.FullName + @"\" + destinationInfo.Parent.Name + ".7z", destination7z.CompressionLevel,destination7z.SplitAfter);
                    this.CreateDestination(temporaryDestinationInfo.FullName + @"\" + destinationInfo.Parent.Name + ".7z", destination.Path);
                }

            }
            else if (destination is DestinationRar destinationRar)
            {
                Compression compression = new Compression();
                if (destination.Path is DestinationPathLocal)
                {
                    ServiceKoFrMa.debugLog.WriteToLog("Starting backuping to archive, because the path to destination ends with .rar (" + destination + ')', 5);
                    ServiceKoFrMa.debugLog.WriteToLog("Archive will be made from this folder " + backupPath + @"\ and put into this location " + destination.Path.Path + @"\" + destinationInfo.Parent.Name + ".rar", 7);
                    compression.CompressToRar(ServiceKoFrMa.daemonSettings.WinRARPath, backupPath + @"\", destination.Path.Path + @"\" + destinationInfo.Parent.Name + ".rar", destinationRar.CompressionLevel, destinationRar.SplitAfter);
                }
                else
                {
                    compression.CompressToRar(ServiceKoFrMa.daemonSettings.WinRARPath, backupPath + @"\", temporaryDestinationInfo.FullName + @"\" + destinationInfo.Parent.Name + ".rar", destinationRar.CompressionLevel, destinationRar.SplitAfter);
                    this.CreateDestination(temporaryDestinationInfo.FullName + @"\" + destinationInfo.Parent.Name + ".7z", destination.Path);
                }

            }


            else if (destination is DestinationPlain)
            {
                if (!(backupPath == destinationInfo.Parent.FullName))
                {
                    if (destination.Path is DestinationPathLocal)
                    {
                        ServiceKoFrMa.debugLog.WriteToLog("Copying files to another local folder...", 6);
                        ServiceKoFrMa.debugLog.WriteToLog("because " + backupPath + " is not " + destinationInfo.Parent.FullName, 6);
                        Directory.CreateDirectory(destination.Path.Path);
                        this.CopyDirectoryRecursivlyWithoutLog(new DirectoryInfo(backupPath), new DirectoryInfo(destination.Path.Path));
                    }
                    else
                    {
                        this.CreateDestination(backupPath, destination.Path);
                    }

                }
                else
                {
                    ServiceKoFrMa.debugLog.WriteToLog("Keeping the plain backup where it is.", 6);
                }



            }

        }

        private void CreateDestination(string backupPath,IDestinationPath destinationPath)
        {
            if (destinationPath is DestinationPathNetworkShare destinationPathNetworkShare)
            {
                ServiceKoFrMa.debugLog.WriteToLog("Starting uploading files to samba/shared server, because the path to destination starts with \\ (" + destinationPath.Path + ')', 5);

            }
            else if (destinationPath is DestinationPathFTP destinationPathFTP)
            {
                ServiceKoFrMa.debugLog.WriteToLog("Starting uploading files to ftp, because the path to destination starts with ftp:// (" + destinationPath.Path + ')', 5);
                FTPConnection fTPConnection = new FTPConnection(destinationPathFTP);
                fTPConnection.UploadToFTP(backupPath);
            }
            else if (destinationPath is DestinationPathSFTP destinationPathSFTP)
            {
                ServiceKoFrMa.debugLog.WriteToLog("Starting uploading files to ftp, because the path to destination starts with sftp:// (" + destinationPath.Path + ')', 5);
                SSHConnection sSHConnection = new SSHConnection(destinationPathSFTP);
                sSHConnection.UploadToSSH(backupPath);
            }
        }
            


        private void CopyDirectoryRecursivlyWithoutLog(DirectoryInfo from, DirectoryInfo to)
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
                    this.CopyDirectoryRecursivlyWithoutLog(item, to.CreateSubdirectory(item.Name));
                }
            }
            catch (Exception)
            {
            }
        }

        //private void CopyDirectoryRecursivly(DirectoryInfo from, DirectoryInfo to)
        //{
        //    if (!to.Exists)
        //    {
        //        to.Create();
        //    }

        //    foreach (FileInfo item in from.GetFiles())
        //    {
        //        try
        //        {
        //            item.CopyTo(to.FullName + @"\" + item.Name);
        //            FilesCorrect.Add(new FileInfoObject { RelativePath = item.FullName.Remove(0, sourceInfo.FullName.Length), Length = item.Length, CreationTimeUtc = item.CreationTimeUtc, LastWriteTimeUtc = item.LastWriteTimeUtc, Attributes = item.Attributes.ToString(), MD5 = this.CalculateMD5(item.FullName) });
        //        }
        //        catch (Exception ex)
        //        {
        //            this.FilesErrorCopy.Add(new CopyErrorObject() { FullPath = item.FullName, ExceptionMessage = ex.Message });
        //        }

        //    }

        //    foreach (DirectoryInfo item in from.GetDirectories())
        //    {
        //        try
        //        {
        //            this.CopyDirectoryRecursivly(item, to.CreateSubdirectory(item.Name));
        //            this.FoldersCorrect.Add(new FolderObject() { RelativePath = item.FullName.Remove(0, sourceInfo.FullName.Length), CreationTimeUtc = item.CreationTimeUtc, LastWriteTimeUtc = item.LastWriteTimeUtc, Attributes = item.Attributes.ToString() });
        //        }
        //        catch (Exception ex)
        //        {
        //            this.FoldersErrorCopy.Add(new CopyErrorObject() { FullPath = item.FullName, ExceptionMessage = ex.Message });
        //        }

        //    }
        //}

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
