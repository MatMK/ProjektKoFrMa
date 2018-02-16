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

        public void BackupFullFolder(string source, string destination, bool createLog)
        {
            DateTime timeOfBackup = DateTime.Now;

            DirectoryInfo sourceInfo = new DirectoryInfo(source);
            DirectoryInfo destinationInfo = new DirectoryInfo(destination).CreateSubdirectory("KoFrMaBackup_" + String.Format("{0:yyyy_MM_dd_HH_mm_ss}", timeOfBackup)).CreateSubdirectory("KoFrMaBackup");
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
            BackupLog.CreateBackupLog(FilesCorrect, destinationInfo.Parent.FullName + @"\KoFrMaBackup.dat");
            //BackupLog.CreateBackupLog(BackupLog.LoadBackupList(destinationInfo.Parent.FullName + @"\" + "KoFrMaBackup.dat"), destinationInfo.Parent.FullName + @"\" + "KoFrMaBackup2.dat");
            DebugLog.WriteToLog("Log byl úspěšně vytvořen");
            BackupLog = null;
            DebugLog.WriteToLog("Full backup " + timeOfBackup.ToString() + " byl dokončen");

        }


        public void BackupDifferential(string source, string destination, string OriginalBackupDatFilePath, bool createLog)
        {
            DateTime timeOfBackup = DateTime.Now;
            DirectoryInfo sourceInfo = new DirectoryInfo(source);
            DirectoryInfo destinationInfo = new DirectoryInfo(destination).CreateSubdirectory("KoFrMaBackup" + timeOfBackup.Year + timeOfBackup.Month + timeOfBackup.Day + timeOfBackup.Hour + timeOfBackup.Minute).CreateSubdirectory("KoFrMaBackup");
            BackupLog = new LogOperations(destinationInfo.Parent.FullName + @"\" + "KoFrMaBackup.dat");
            this.CopyDirectoryRecursivly(sourceInfo, null, false);
            List<FileInfoObject> CurrentFiles = FilesCorrect;
            List<FileInfoObject> OriginalFiles = BackupLog.LoadBackupList(OriginalBackupDatFilePath);
            List<string> FilesToCopy = new List<string>(500);
            List<string> FilesToDeleteFromOriginal = new List<string>(500);
            foreach (FileInfoObject itemCurrent in CurrentFiles)
            {
                foreach (FileInfoObject itemOriginal in OriginalFiles)
                {
                    bool sameFile = false;
                    if (itemCurrent.HashRow == itemOriginal.HashRow)
                    {
                        if (itemCurrent.FullName == itemOriginal.FullName
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
                        FilesToCopy.Add(itemCurrent.FullName);
                    }
                }
            }
            // M * delete:
            foreach (FileInfoObject itemOriginal in OriginalFiles)
            {
                if (!itemOriginal.Paired)
                {
                    FilesToDeleteFromOriginal.Add(itemOriginal.FullName);
                }
            }



        }
        
        private void CopyDirectoryRecursivly(DirectoryInfo from, DirectoryInfo to, bool Copy)
        {
            if (!to.Exists&&Copy)
            {
                to.Create();
            }
                
                foreach (FileInfo item in from.GetFiles())
                {
                    try
                    {
                        if (Copy)
                        item.CopyTo(to.FullName + @"\" + item.Name);
                        FilesCorrect.Add(new FileInfoObject { FullName = item.FullName, Length = item.Length, CreationTimeUtc = item.CreationTimeUtc, LastWriteTimeUtc = item.LastWriteTimeUtc, Attributes = item.Attributes.ToString(), MD5 = this.CalculateMD5(item.FullName) });
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
                        if (Copy)
                        this.CopyDirectoryRecursivly(item, to.CreateSubdirectory(item.Name), Copy);
                        this.FoldersCorrect.Add(item.FullName);
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
