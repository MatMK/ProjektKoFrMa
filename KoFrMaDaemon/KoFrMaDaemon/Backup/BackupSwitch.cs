using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace KoFrMaDaemon.Backup
{
    public class BackupSwitch
    {
        protected DirectoryInfo sourceInfo;
        protected DirectoryInfo destinationInfo;

        public void Backup(string source, string destination,  byte? compressionLevel, DebugLog debugLog)
        {
            if (source.EndsWith(".dat")) //když bude jako zdroj úlohy nastaven path na soubor .dat provede se diferenciální, jinak pokud je to složka tak plná
            {
                debugLog.WriteToLog("Starting differential/incremental backup, because the path to source ends with .dat (" + source + ')', 5);
                BackupDifferential backupDifferential = new BackupDifferential();
                backupDifferential.BackupDifferentialProcess(source, Path.GetDirectoryName(destination), debugLog);
                this.sourceInfo = backupDifferential.sourceInfo;
                this.destinationInfo = backupDifferential.destinationInfo;
            }
            else
            {
                debugLog.WriteToLog("Starting full backup, because the path to source doesn't end with .dat (" + source + ')', 5);
                BackupFull backupFull = new BackupFull();
                backupFull.BackupFullProcess(source, Path.GetDirectoryName(destination), debugLog);
                this.sourceInfo = backupFull.sourceInfo;
                this.destinationInfo = backupFull.destinationInfo;
            }

            if (destination.EndsWith(".zip") || destination.EndsWith(".rar") || destination.EndsWith(".7z"))
            {
                debugLog.WriteToLog("Starting backuping to archive, because the path to destination ends with .zip, .rar or .7z (" + destination + ')', 5);
                Compression compression = new Compression(debugLog);
                compression.CompressToZip(destinationInfo.FullName, destinationInfo.Parent.FullName + @"\"+this.destinationInfo.Name+".zip",compressionLevel);
                debugLog.WriteToLog("Compression done, deleting temporary files that were needed for compression", 6);
                Directory.Delete(destinationInfo.FullName);
                debugLog.WriteToLog("Files successfully deleted, compression is now completed.", 6);
            }
            else
            {
                debugLog.WriteToLog("Keeping the plain backup because the task doesn't want to archive", 5);
            }
            debugLog.WriteToLog("Backup done, ending the backup instatnce", 7);
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
