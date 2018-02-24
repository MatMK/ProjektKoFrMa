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
        private StreamWriter w;

        private StreamReader r;

        public void CreateBackupJournal(BackupJournalObject backupJournalObject, string pathToJournal, DebugLog debugLog)
        {
            try
            {
                w = new StreamWriter(pathToJournal, true);
                debugLog.WriteToLog("Writing relative path to backup journal...",7);
                w.WriteLine(backupJournalObject.RelativePath);
                debugLog.WriteToLog("Writing list of files to backup journal...", 7);
                List<FileInfoObject> fileBackupJournalHash = this.ReturnHashCodesFiles(backupJournalObject.BackupJournalFiles);
                for (int i = 0; i < fileBackupJournalHash.Count; i++)
                {
                    w.WriteLine(fileBackupJournalHash[i].RelativePath + '|' + fileBackupJournalHash[i].Length.ToString() + '|' + fileBackupJournalHash[i].CreationTimeUtc.ToBinary().ToString() + '|' + fileBackupJournalHash[i].LastWriteTimeUtc.ToBinary().ToString() + '|' + fileBackupJournalHash[i].Attributes + '|' + fileBackupJournalHash[i].MD5 + '|' + fileBackupJournalHash[i].HashRow.ToString());
                }
                debugLog.WriteToLog("Writing list of folders to backup journal...", 7);
                List<FolderObject> folderBackupJournalHash = this.ReturnHashCodesFolders(backupJournalObject.BackupJournalFolders);
                w.WriteLine("?");
                for (int i = 0; i < folderBackupJournalHash.Count; i++)
                {
                    w.WriteLine(folderBackupJournalHash[i].RelativePath + '|' + folderBackupJournalHash[i].CreationTimeUtc.ToBinary().ToString() + '|' + folderBackupJournalHash[i].LastWriteTimeUtc.ToBinary().ToString() + '|' + folderBackupJournalHash[i].Attributes.ToString() + '|' + folderBackupJournalHash[i].HashRow.ToString());
                }
                w.Close();
                w.Dispose();
            }
            catch (Exception ex)
            {
                debugLog.WriteToLog("Fatal error when trying to create backup journal: " + ex.Message, 2);
                //zde už nelze pokračovat, nutno shodit celý proces zálohování, nekompletní log znamená vadnou zálohu!
            }

        }

        public BackupJournalObject LoadBackupJournalObject(string pathToJournal, DebugLog debugLog)
        {
            BackupJournalObject backupJournalObject = new BackupJournalObject();

            try
            {
                r = new StreamReader(pathToJournal);
                debugLog.WriteToLog("Loading relative path from backup journal...",7);
                backupJournalObject.RelativePath = r.ReadLine();
                debugLog.WriteToLog("Loading list of files from backup journal...", 7);
                List<FileInfoObject> tmpListFiles = new List<FileInfoObject>(100);
                string[] tmp;
                while (r.Peek() != '?')
                {
                    tmp = r.ReadLine().Split('|');
                    if (tmp.Length == 7)
                    {
                        tmpListFiles.Add(new FileInfoObject() { RelativePath = tmp[0], Length = Convert.ToInt64(tmp[1]), CreationTimeUtc = DateTime.FromBinary(Convert.ToInt64(tmp[2])), LastWriteTimeUtc = DateTime.FromBinary(Convert.ToInt64(tmp[3])), Attributes = tmp[4], MD5 = tmp[5], HashRow = Convert.ToInt32(tmp[6]) });
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

                backupJournalObject.BackupJournalFiles = tmpListFiles;
                debugLog.WriteToLog("Loading list of folders from backup journal...", 7);
                List<FolderObject> tmpListFolders = new List<FolderObject>(100);
                r.ReadLine();
                while (!r.EndOfStream)
                {
                    tmp = r.ReadLine().Split('|');
                    if (tmp.Length == 5)
                    {
                        tmpListFolders.Add(new FolderObject() { RelativePath = tmp[0], CreationTimeUtc = DateTime.FromBinary(Convert.ToInt64(tmp[1])), LastWriteTimeUtc = DateTime.FromBinary(Convert.ToInt64(tmp[2])), Attributes = tmp[3], HashRow = Convert.ToInt32(tmp[4]) });
                    }
                    else
                    {
                        string tmpRow = "";
                        for (int i = 0; i < tmp.Length; i++)
                        {
                            tmpRow += ('|' + tmp[i]);
                        }
                        debugLog.WriteToLog("Error when trying to load folder row from backup journal, the row is: " + tmpRow, 3);
                    }
                }
                backupJournalObject.BackupJournalFolders = tmpListFolders;

            }
            catch (Exception ex)
            {
                debugLog.WriteToLog("Fatal error loading backup journal, backup cannot continue: " + ex.Message,2);
            }
            
            return backupJournalObject;

        }
        public List<FileInfoObject> ReturnHashCodesFiles(List<FileInfoObject> listWithoutHashCodes)
        {
            List<FileInfoObject> tmpList;

            tmpList = listWithoutHashCodes;
            string tmp;
            foreach (FileInfoObject item in tmpList)
            {
                tmp = item.RelativePath + item.Length.ToString() + item.CreationTimeUtc.ToBinary().ToString() + item.LastWriteTimeUtc.ToBinary().ToString() + item.Attributes + item.MD5;
                item.HashRow = tmp.GetHashCode();
            }

            return tmpList;
        }

        public List<FolderObject> ReturnHashCodesFolders(List<FolderObject> listWithoutHashCodes)
        {
            List<FolderObject> tmpList;
            tmpList = listWithoutHashCodes;
            string tmp;
            for (int i = 0; i < tmpList.Count; i++)
            {
                tmp = listWithoutHashCodes[i].RelativePath + listWithoutHashCodes[i].CreationTimeUtc.ToBinary().ToString() + listWithoutHashCodes[i].LastWriteTimeUtc.ToBinary().ToString() + listWithoutHashCodes[i].Attributes;
                listWithoutHashCodes[i].HashRow = tmp.GetHashCode();
            }

            return tmpList;
        }
    }
}
