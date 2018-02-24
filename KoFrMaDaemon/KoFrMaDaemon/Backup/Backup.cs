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

        public void Backup(string source, string destination,  byte compressionLevel, DebugLog debugLog)
        {
            if (source.EndsWith(".dat")) //když bude jako zdroj úlohy nastaven path na soubor .dat provede se diferenciální, jinak pokud je to složka tak plná
            {
                debugLog.WriteToLog("Starting differential/incremental backup, because the path to source ends with .dat (" + source + ')', 7);
                BackupDifferential backupDifferential = new BackupDifferential();
                backupDifferential.BackupDifferentialProcess(source, destination, debugLog);
            }
            else
            {
                debugLog.WriteToLog("Starting full backup, because the path to source doesn't end with .dat (" + source + ')', 7);
                BackupFull backupFull = new BackupFull();
                backupFull.BackupFullProcess(source, destination, debugLog);
            }

            if (destination.EndsWith(".zip") || destination.EndsWith(".rar") || destination.EndsWith(".7z"))
            {
                debugLog.WriteToLog("Starting backuping to archive, because the path to destination ends with .zip, .rar or .7z (" + source + ')', 7);
                Compression compression = new Compression(debugLog);
                compression.CompressToZip(destinationInfo.FullName, destinationInfo.FullName + @"\" + ".zip",compressionLevel);

                Directory.Delete(destinationInfo.FullName);
            }
            else
            {
                debugLog.WriteToLog("Keeping the plain backup because the task doesn't want to archive", 7);
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
