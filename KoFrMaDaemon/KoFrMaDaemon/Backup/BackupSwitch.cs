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

        private List<string> destinations;
        private byte? compressionLevel;
        private NetworkCredential networkCredential;
        private bool atLeastOneDestinationIsLocalFolder = false;



        public List<FileInfoObject> FilesCorrect;
        public List<FolderObject> FoldersCorrect;
        public List<CopyErrorObject> FilesErrorCopy;
        public List<CopyErrorObject> FoldersErrorCopy;

        public List<CopyErrorObject> FilesErrorLoad;
        public List<CopyErrorObject> FoldersErrorLoad;

        public void Backup(string source, BackupJournalObject backupJournalSource, List<string> destinations, byte? compressionLevel,NetworkCredential networkCredential, int TaskID,int? bufferSize, DebugLog debugLog)
        {
            this.networkCredential = networkCredential;
            this.compressionLevel = compressionLevel;
            this.destinations = destinations;
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
            this.BackupToFolder(source, backupJournalSource, this.temporaryDestinationInfo.FullName, TaskID, bufferSize,debugLog);
            ServiceKoFrMa.debugLog.WriteToLog("Creating desired destination outputs...", 7);
            this.FinishBackup();
            ServiceKoFrMa.debugLog.WriteToLog("Backup done, ending the backup instance.", 7);
        }
        private void BackupToFolder(string source, BackupJournalObject backupJournalSource, string destination, int TaskID,int? bufferSize,DebugLog debugLog)
        {
            ServiceKoFrMa.debugLog.WriteToLog("Deciding what type of backup it is...", 7);
            if (backupJournalSource != null)
            {
                debugLog.WriteToLog("Starting differential/incremental backup, because journal was received from the server", 5);
                this.BackupDifferentialProcess(backupJournalSource, destination,TaskID,bufferSize, debugLog);
            }
            else
            {
                debugLog.WriteToLog("Starting full backup, because the there is no info about backup journal", 5);
                this.BackupFullProcess(source, destination, TaskID,bufferSize, debugLog);
                //this.sourceInfo = backupFull.sourceInfo.Parent;
                //this.destinationInfo = backupFull.destinationInfo.Parent;
                //BackupJournalNew = backupFull.BackupJournalNew;
            }

        }
        public void BackupFullProcess(string source, string destination, int TaskID, int? bufferSize, DebugLog serviceDebugLog)
        {
            FilesCorrect = new List<FileInfoObject>(100);
            FoldersCorrect = new List<FolderObject>(100);
            FilesErrorCopy = new List<CopyErrorObject>(100);
            FoldersErrorCopy = new List<CopyErrorObject>(100);

            DateTime timeOfBackup = DateTime.Now;

            string temporaryDebugInfo = "";
            if (serviceDebugLog._logLevel >= 4)
                temporaryDebugInfo = "Full backup started at  " + timeOfBackup.ToString();
            Directory.CreateDirectory(destination + @"\KoFrMaBackup_" + String.Format("{0:yyyy_MM_dd_HH_mm_ss}", timeOfBackup) + @"_Full\KoFrMaBackup");
            destinationInfo = new DirectoryInfo(destination + @"\KoFrMaBackup_" + String.Format("{0:yyyy_MM_dd_HH_mm_ss}", timeOfBackup) + @"_Full\KoFrMaBackup");

            serviceDebugLog.WriteToLog("Log of including operations is located in " + destinationInfo.Parent.FullName + @"\KoFrMaDebug.log", 4);

            DebugLog DebugLog = new DebugLog(destinationInfo.Parent.FullName + @"\KoFrMaDebug.log", serviceDebugLog.writeToWindowsEventLog, serviceDebugLog._logLevel);

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
                this.BackupFileCopy(sourceInfo.FullName, FileListString, FolderListString, bufferSize,DebugLog);
                DebugLog.WriteToLog("Backup done, " + FilesCorrect.Count.ToString() + " files and " + FoldersCorrect.Count.ToString() + " folders successfully backuped, it was unable to backup " + FilesErrorCopy.Count + " files and " + FoldersErrorCopy.Count + " folders", 5);
            }
            catch (Exception x)
            {
                DebugLog.WriteToLog("Error " + x.Message + " occured and backup couldn't be fully done", 3);
            }

            DebugLog.WriteToLog("Creating transaction jounal of successfully backuped files and folders...", 5);
            BackupJournalOperations BackupJournal = new BackupJournalOperations();
            BackupJournalNew = new BackupJournalObject() { RelativePath = source, BackupJournalFiles = FilesCorrect, BackupJournalFolders = FoldersCorrect };
            BackupJournal.CreateBackupJournal(BackupJournalNew, destinationInfo.Parent.FullName + @"\KoFrMaBackup.dat", TaskID, DebugLog);
            DebugLog.WriteToLog("Journal successfully created", 5);
            TimeSpan backupTook = DateTime.Now - timeOfBackup;
            DebugLog.WriteToLog("Full backup was completed in " + backupTook.TotalSeconds + " s", 4);
        }


        public void BackupDifferentialProcess(BackupJournalObject backupJournalSource, string destination, int TaskID, int? bufferSize, DebugLog serviceDebugLog)
        {
            FilesCorrect = new List<FileInfoObject>(100);
            FoldersCorrect = new List<FolderObject>(100);
            FilesErrorLoad = new List<CopyErrorObject>(100);
            FilesErrorCopy = new List<CopyErrorObject>(100);
            FoldersErrorLoad = new List<CopyErrorObject>(100);
            FoldersErrorCopy = new List<CopyErrorObject>(100);

            DateTime timeOfBackup = DateTime.Now;

            string temporaryDebugInfo = "";
            if (serviceDebugLog._logLevel >= 4)
                temporaryDebugInfo = "Starting the differential/incremental backup in " + timeOfBackup.ToString();

            destinationInfo = new DirectoryInfo(destination).CreateSubdirectory("KoFrMaBackup_" + String.Format("{0:yyyy_MM_dd_HH_mm_ss}", timeOfBackup)).CreateSubdirectory("KoFrMaBackup");

            serviceDebugLog.WriteToLog("Log of including operations is located in " + destinationInfo.Parent.FullName + @"\" + "KoFrMaDebug.log", 4);

            DebugLog DebugLog = new DebugLog(destinationInfo.Parent.FullName + @"\" + "KoFrMaDebug.log", serviceDebugLog.writeToWindowsEventLog, serviceDebugLog._logLevel);

            DebugLog.WriteToLog("Subdirectory for the backup was created at " + destinationInfo.FullName, 5);

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
            sourceInfo = new DirectoryInfo(source);
            BackupJournalObject currentJournalObject = this.JournalCurrent(sourceInfo);
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






            DebugLog.WriteToLog("Starting to backup files and folders to " + destinationInfo.FullName + @"\", 4);

            this.BackupFileCopy(source, FilesToCopy, FoldersToCreate, bufferSize, DebugLog);


            DebugLog.WriteToLog("Creating transaction log of successfully copied files and folders...", 5);
            BackupJournalNew = new BackupJournalObject()
            {
                RelativePath = source,
                BackupJournalFiles = FilesCorrect,
                BackupJournalFolders = FoldersCorrect
            };
            BackupJournal.CreateBackupJournal(BackupJournalNew, destinationInfo.Parent.FullName + @"\KoFrMaBackup.dat", TaskID, DebugLog);
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

        private void BackupFileCopy(string source, List<string> filesToCopy,List<string> foldersToCreate,int? bufferSize,DebugLog DebugLog)
        {

            DebugLog.WriteToLog("Backuping modified folder structure...", 5);
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
            DebugLog.WriteToLog("Backup of folder structure is done, " + this.FoldersCorrect.Count + " folders were successfully created, it was unable to create " + this.FoldersErrorCopy.Count + " folders", 5);







            DebugLog.WriteToLog("Backuping new or modified files...", 5);
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
                    DebugLog.WriteToLog("Unable to copy " + tmpFileInfo.FullName + " to " + destinationInfo.FullName + @"\" + filesToCopy[i] + " because of exception " + ex.Message + ". Path to destination folder: " + tmpFileInfo.Directory.FullName, 8);
                }

            }


            DebugLog.WriteToLog("File backup is done, " + this.FilesCorrect.Count + " files were successfully copied, it was unable to copy " + this.FilesErrorCopy.Count + " files", 5);


        }

        private void FinishBackup()
        {

            for (int i = 0; i < destinations.Count; i++)
            {
                this.CreateDestination(this.destinationInfo.Parent.FullName, destinations[i], compressionLevel, networkCredential);
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

        private void CreateDestination(string backupPath,string destination, byte? compressionLevel, NetworkCredential networkCredential)
        {
            if (destination.EndsWith(".zip"))
            {
                ServiceKoFrMa.debugLog.WriteToLog("Starting backuping to archive, because the path to destination ends with .zip (" + destination + ')', 5);
                Compression compression = new Compression();
                compression.CompressToZip(backupPath, Path.GetDirectoryName(destination) + @"\" + destinationInfo.Parent.Name + ".zip", compressionLevel);
            }
            if (destination.EndsWith(".7z"))
            {
                ServiceKoFrMa.debugLog.WriteToLog("Starting backuping to archive, because the path to destination ends with .7z (" + destination + ')', 5);
                Compression compression = new Compression();
                ServiceKoFrMa.debugLog.WriteToLog("Archive will be made from this folder "+backupPath +@"\ and put into this location "+ Path.GetDirectoryName(destination)+@"\"+destinationInfo.Parent.Name+".7z", 7);
                compression.CompressTo7z(ServiceKoFrMa.daemonSettings.SevenZipPath,backupPath + @"\", Path.GetDirectoryName(destination) + @"\" + destinationInfo.Parent.Name + ".7z", compressionLevel);
            }
            if (destination.EndsWith(".rar"))
            {
                ServiceKoFrMa.debugLog.WriteToLog("Starting backuping to archive, because the path to destination ends with .rar (" + destination + ')', 5);
                Compression compression = new Compression();
                ServiceKoFrMa.debugLog.WriteToLog("Archive will be made from this folder " + backupPath + @"\ and put into this location " + Path.GetDirectoryName(destination) + @"\" + destinationInfo.Parent.Name + ".rar", 7);
                compression.CompressToRar(ServiceKoFrMa.daemonSettings.WinRARPath, backupPath + @"\", Path.GetDirectoryName(destination) + @"\" + destinationInfo.Parent.Name + ".rar", compressionLevel);
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
                if (!(backupPath==destinationInfo.Parent.FullName))
                {
                    ServiceKoFrMa.debugLog.WriteToLog("Copying files to another local folder...", 6);
                    ServiceKoFrMa.debugLog.WriteToLog("because "+backupPath +" is not "+destinationInfo.Parent.FullName, 6);
                    Directory.CreateDirectory(destination);
                    this.CopyDirectoryRecursivlyWithoutLog(new DirectoryInfo(backupPath), new DirectoryInfo(destination));
                }
                else
                {
                    ServiceKoFrMa.debugLog.WriteToLog("Keeping the plain backup where it is.", 6);
                }

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
                    FilesCorrect.Add(new FileInfoObject { RelativePath = item.FullName.Remove(0, sourceInfo.FullName.Length), Length = item.Length, CreationTimeUtc = item.CreationTimeUtc, LastWriteTimeUtc = item.LastWriteTimeUtc, Attributes = item.Attributes.ToString(), MD5 = this.CalculateMD5(item.FullName) });
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
                    this.FoldersCorrect.Add(new FolderObject() { RelativePath = item.FullName.Remove(0, sourceInfo.FullName.Length), CreationTimeUtc = item.CreationTimeUtc, LastWriteTimeUtc = item.LastWriteTimeUtc, Attributes = item.Attributes.ToString() });
                }
                catch (Exception ex)
                {
                    this.FoldersErrorCopy.Add(new CopyErrorObject() { FullPath = item.FullName, ExceptionMessage = ex.Message });
                }

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
