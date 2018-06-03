using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace KoFrMaDaemon.Backup
{
    public class BackupJournalOperations
    {
        private StreamWriter w1;

        private StreamWriter w2;

        private StreamReader r;

        private List<string> tmpList;

        /// <summary>
        /// Fuction that writes backup journal (that can be later build on) to drive and also saves the backup journal to local cache named as ID of the task so it can be automatically selected by daemon when communicating with the server
        /// </summary>
        /// <param name="backupJournalObject">The backup journal you want to save to disk</param>
        /// <param name="pathToJournalFile">Path to location including the filename where the backup journal should by saved</param>
        /// <param name="TaskID">ID of the task that you are saving, if null not saving it</param>
        /// <param name="debugLog"><c>DebugLog</c> instance for logging performed actions</param>
        public void CreateBackupJournal(BackupJournalObject backupJournalObject, string pathToJournalFile, int TaskID, DebugLog debugLog)
        {
            try
            {
                tmpList = new List<string>();

                debugLog.WriteToLog("Writing relative paths to backup journal...",7);
                for (int i = 0; i < backupJournalObject.RelativePaths.Count; i++)
                {
                    tmpList.Add(backupJournalObject.RelativePaths[i]);
                }
                tmpList.Add("!");
                debugLog.WriteToLog("Writing list of files to backup journal...", 7);
                List<FileInfoObject> fileBackupJournalHash = this.ReturnHashCodesFiles(backupJournalObject.BackupJournalFiles);
                for (int i = 0; i < fileBackupJournalHash.Count; i++)
                {
                    tmpList.Add(fileBackupJournalHash[i].FullPath + '|' + fileBackupJournalHash[i].Length.ToString() + '|' + fileBackupJournalHash[i].CreationTimeUtc.ToBinary().ToString() + '|' + fileBackupJournalHash[i].LastWriteTimeUtc.ToBinary().ToString() + '|' + fileBackupJournalHash[i].Attributes + '|' + fileBackupJournalHash[i].MD5 + '|' + fileBackupJournalHash[i].HashRow.ToString());
                }
                tmpList.Add("?");
                if (backupJournalObject.BackupJournalFilesDelete !=null)
                {
                    debugLog.WriteToLog("Writing list of deleted files to backup journal...", 7);
                    for (int i = 0; i < backupJournalObject.BackupJournalFilesDelete.Count; i++)
                    {
                        tmpList.Add(backupJournalObject.BackupJournalFilesDelete[i]);
                    }
                }

                debugLog.WriteToLog("Writing list of folders to backup journal...", 7);
                List<FolderObject> folderBackupJournalHash = this.ReturnHashCodesFolders(backupJournalObject.BackupJournalFolders);
                tmpList.Add(":");
                for (int i = 0; i < folderBackupJournalHash.Count; i++)
                {
                    tmpList.Add(folderBackupJournalHash[i].FullPath + '|' + folderBackupJournalHash[i].CreationTimeUtc.ToBinary().ToString() + '|' +  folderBackupJournalHash[i].Attributes.ToString() + '|' + folderBackupJournalHash[i ].HashRow.ToString());
                }
                tmpList.Add("?");
                if (backupJournalObject.BackupJournalFoldersDelete != null)
                {
                    debugLog.WriteToLog("Writing list of deleted folders to backup journal...", 7);
                    for (int i = 0; i < backupJournalObject.BackupJournalFoldersDelete.Count; i++)
                    {
                        tmpList.Add(backupJournalObject.BackupJournalFoldersDelete[i]);
                    }
                }




                debugLog.WriteToLog("Writing backup journal to disk...", 7);

                w1 = new StreamWriter(pathToJournalFile);
                for (int i = 0; i < tmpList.Count; i++)
                {
                    w1.WriteLine(tmpList[i]);
                }
                w1.Close();
                w1.Dispose();

                if (TaskID >0)
                {
                    Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\KoFrMa\journalcache\");
                    File.Copy(pathToJournalFile, Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\KoFrMa\journalcache\" + TaskID.ToString() + ".dat", true);
                    //w2 = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\KoFrMa\journalcache\" + TaskID.ToString() + ".dat", false);
                    //for (int i = 0; i < tmpList.Count; i++)
                    //{
                    //    w2.WriteLine(tmpList[i]);
                    //}
                    //w2.Close();
                    //w2.Dispose();
                }
            }
            catch (Exception ex)
            {
                debugLog.WriteToLog("Fatal error when trying to create backup journal: " + ex, 2);
                throw;
                //zde už nelze pokračovat, nutno shodit celý proces zálohování, nekompletní log znamená vadnou zálohu!
            }

        }
        /// <summary>
        /// Function that loads saved backup journal from disk based on its path <paramref name="pathToJournal"/>
        /// </summary>
        /// <param name="pathToJournal">Path to the backup journal you want to load including the filename</param>
        /// <param name="debugLog"><c>DebugLog</c> instance for logging performed actions</param>
        /// <returns><c>BackupJournalObject that will be loaded</c></returns>

        public BackupJournalObject MergeJournalObjects(BackupJournalObject journalObjectOld, BackupJournalObject journalObjectNew)
        {
            BackupJournalObject result = new BackupJournalObject();

            result.RelativePaths = journalObjectOld.RelativePaths;
            result.BackupJournalFiles = new List<FileInfoObject>(journalObjectOld.BackupJournalFiles.Count+journalObjectNew.BackupJournalFiles.Count);
            result.BackupJournalFolders = new List<FolderObject>(journalObjectOld.BackupJournalFiles.Count + journalObjectNew.BackupJournalFiles.Count);
            result.BackupJournalFilesDelete = new List<string>(journalObjectOld.BackupJournalFiles.Count + journalObjectNew.BackupJournalFiles.Count);
            result.BackupJournalFoldersDelete = new List<string>(journalObjectOld.BackupJournalFiles.Count + journalObjectNew.BackupJournalFiles.Count);
            List<string> compareList;

            result.BackupJournalFiles.AddRange(journalObjectNew.BackupJournalFiles);
            compareList = new List<string>(journalObjectNew.BackupJournalFiles.Count);
            for (int i = 0; i < journalObjectNew.BackupJournalFiles.Count; i++)
            {
                compareList.Add(journalObjectNew.BackupJournalFiles[i].FullPath);
            }
            for (int i = 0; i < journalObjectOld.BackupJournalFiles.Count; i++)
            {
                if (!compareList.Contains(journalObjectOld.BackupJournalFiles[i].FullPath))
                {
                    result.BackupJournalFiles.Add(journalObjectOld.BackupJournalFiles[i]);
                }
            }

            result.BackupJournalFolders.AddRange(journalObjectNew.BackupJournalFolders);
            compareList = new List<string>(journalObjectNew.BackupJournalFolders.Count);
            for (int i = 0; i < journalObjectNew.BackupJournalFolders.Count; i++)
            {
                compareList.Add(journalObjectNew.BackupJournalFolders[i].FullPath);
            }
            for (int i = 0; i < journalObjectOld.BackupJournalFolders.Count; i++)
            {
                if (!compareList.Contains(journalObjectOld.BackupJournalFolders[i].FullPath))
                {
                    result.BackupJournalFolders.Add(journalObjectOld.BackupJournalFolders[i]);
                }
            }


            result.BackupJournalFilesDelete.AddRange(journalObjectNew.BackupJournalFilesDelete);
            for (int i = 0; i < journalObjectOld.BackupJournalFilesDelete.Count; i++)
            {
                if (!journalObjectNew.BackupJournalFilesDelete.Contains(journalObjectOld.BackupJournalFilesDelete[i]))
                {
                    result.BackupJournalFilesDelete.Add(journalObjectOld.BackupJournalFilesDelete[i]);
                }
            }


            result.BackupJournalFoldersDelete.AddRange(journalObjectNew.BackupJournalFoldersDelete);
            for (int i = 0; i < journalObjectOld.BackupJournalFoldersDelete.Count; i++)
            {
                if (!journalObjectNew.BackupJournalFoldersDelete.Contains(journalObjectOld.BackupJournalFoldersDelete[i]))
                {
                    result.BackupJournalFoldersDelete.Add(journalObjectOld.BackupJournalFoldersDelete[i]);
                }
            }



            return result;
        }


        public BackupJournalObject LoadBackupJournalObject(string pathToJournal, DebugLog debugLog)
        {
            BackupJournalObject backupJournalObject = new BackupJournalObject();

            try
            {
                r = new StreamReader(pathToJournal);
                debugLog.WriteToLog("Loading relative paths from backup journal...",7);
                backupJournalObject.RelativePaths = new List<string>(1);
                while (r.Peek() != '!')
                {
                    backupJournalObject.RelativePaths.Add(r.ReadLine());
                }
                r.ReadLine();
                debugLog.WriteToLog("Loading list of files from backup journal...", 7);
                List<FileInfoObject> tmpListFiles = new List<FileInfoObject>(100);
                string[] tmp;
                while (r.Peek() != '?')
                {
                    tmp = r.ReadLine().Split('|');
                    if (tmp.Length == 7)
                    {
                        tmpListFiles.Add(new FileInfoObject() { FullPath = tmp[0], Length = Convert.ToInt64(tmp[1]), CreationTimeUtc = DateTime.FromBinary(Convert.ToInt64(tmp[2])), LastWriteTimeUtc = DateTime.FromBinary(Convert.ToInt64(tmp[3])), Attributes = tmp[4], MD5 = tmp[5], HashRow = Convert.ToInt32(tmp[6]) });
                    }
                    else
                    {
                        string tmpRow = "";
                        for (int i = 0; i < tmp.Length; i++)
                        {
                            tmpRow += ('|' + tmp[i]);
                        }
                        debugLog.WriteToLog("Error when trying to load file row from backup journal, the row is: " + tmpRow, 3);
                    }
                }
                r.ReadLine();
                backupJournalObject.BackupJournalFiles = tmpListFiles;

                debugLog.WriteToLog("Loading list of deleted files from backup journal...", 7);
                List<string> tmpListDeletedFiles = new List<string>(100);
                while (r.Peek() != ':')
                {
                    tmpListDeletedFiles.Add(r.ReadLine());
                }
                r.ReadLine();
                backupJournalObject.BackupJournalFilesDelete = tmpListDeletedFiles;

                debugLog.WriteToLog("Loading list of folders from backup journal...", 7);
                List<FolderObject> tmpListFolders = new List<FolderObject>(100);
                while ( r.Peek() != '?')
                {
                    tmp = r.ReadLine().Split('|');
                    if (tmp.Length == 4)
                    {
                        tmpListFolders.Add(new FolderObject() { FullPath = tmp[0], CreationTimeUtc = DateTime.FromBinary(Convert.ToInt64(tmp[1])), Attributes = tmp[2], HashRow = Convert.ToInt32(tmp[3]) });
                    }
                    else
                    {
                        string tmpRow = "";
                        for (int i = 0; i < tmp.Length; i++)
                        {
                            tmpRow += ('|' + tmp[i]);
                        }
                        debugLog.WriteToLog("Error when trying to load folder row from backup journal, row has different values, number of values is" +tmp.Length+" and the row is: " + tmpRow, 3);
                    }
                }
                r.ReadLine();
                backupJournalObject.BackupJournalFolders = tmpListFolders;



                debugLog.WriteToLog("Loading list of deleted folders from backup journal...", 7);
                List<string> tmpListDeletedFolders = new List<string>(100);
                while (!r.EndOfStream)
                {
                    tmpListDeletedFolders.Add(r.ReadLine());
                }
                backupJournalObject.BackupJournalFoldersDelete = tmpListDeletedFolders;
            }
            catch (Exception ex)
            {
                debugLog.WriteToLog("Fatal error loading backup journal, backup cannot continue: " + ex.Message,2);
            }
            
            return backupJournalObject;

        }
        /// <summary>
        /// Function that adds hash column to every given <c>FileInfoObject</c> in a list (for quicker comparing if file was changed for differential/incremental backups)
        /// </summary>
        /// <param name="listWithoutHashCodes">List of <c>FileInfoObjects</c> that you want to add the hash row to</param>
        /// <returns>Returns same list as entered only with added hash properties to each <c>FileInfoObjects</c></returns>
        public List<FileInfoObject> ReturnHashCodesFiles(List<FileInfoObject> listWithoutHashCodes)
        {
            List<FileInfoObject> tmpList;

            tmpList = listWithoutHashCodes;
            string tmp;
            foreach (FileInfoObject item in tmpList)
            {
                tmp = item.FullPath + item.Length.ToString() + item.CreationTimeUtc.ToBinary().ToString() + item.LastWriteTimeUtc.ToBinary().ToString() + item.Attributes + item.MD5;
                item.HashRow = tmp.GetHashCode();
            }

            return tmpList;
        }
        /// <summary>
        /// Function that adds hash column to every given <c>FolderObject</c> in a list (for quicker comparing if file was changed for differential/incremental backups)
        /// </summary>
        /// <param name="listWithoutHashCodes">List of <c>FolderObjects</c> that you want to add the hash row to</param>
        /// <returns>Returns same list as entered only with added hash properties to each <c>FolderObject</c></returns>
        public List<FolderObject> ReturnHashCodesFolders(List<FolderObject> listWithoutHashCodes)
        {
            List<FolderObject> tmpList;
            tmpList = listWithoutHashCodes;
            string tmp;
            for (int i = 0; i < tmpList.Count; i++)
            {
                tmp = listWithoutHashCodes[i].FullPath + listWithoutHashCodes[i].CreationTimeUtc.ToBinary().ToString() + listWithoutHashCodes[i].Attributes;
                listWithoutHashCodes[i].HashRow = tmp.GetHashCode();
            }

            return tmpList;
        }
    }
}
