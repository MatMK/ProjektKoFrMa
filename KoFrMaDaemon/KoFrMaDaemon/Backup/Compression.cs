using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;

namespace KoFrMaDaemon.Backup
{
    public class Compression
    {
        public void CompressToZip(string source, string destination, byte? compressionLevel)
        {
            ServiceKoFrMa.debugLog.WriteToLog("Compressing now...",6);
            if (!(compressionLevel==null))
            {
                ZipFile.CreateFromDirectory(source, destination, (CompressionLevel)compressionLevel, false);
            }
            else
            {
                ServiceKoFrMa.debugLog.WriteToLog("Compression level is not set! Cannot continue!", 2);
            }
            
        }

        public void CompressTo7z(string PathTo7zFolder, string source, string destination, byte? compressionLevel)
        {
            ServiceKoFrMa.debugLog.WriteToLog("Compressing now...", 6);
            if (PathTo7zFolder == null || PathTo7zFolder =="")
            {
                ServiceKoFrMa.debugLog.WriteToLog("7-zip is not installed or the path to it is not entered in the .ini file. Cannot compress.", 2);
            }
            else
            {
                if (!(compressionLevel == null))
                {
                    ServiceKoFrMa.debugLog.WriteToLog("Running 7-zip as follows: "+ Path.Combine(PathTo7zFolder, "7z.exe") + " a -t7z " + destination + ' ' + source + "* -mmt -mx" + compressionLevel, 8);
                    Process p = new Process();
                    p.StartInfo.FileName = Path.Combine(PathTo7zFolder, "7z.exe");
                    p.StartInfo.Arguments = "a -t7z " + destination + ' ' + source + "* -mmt -mx" + compressionLevel;
                    p.StartInfo.CreateNoWindow = true;
                    p.Start();
                    p.PriorityClass = ProcessPriorityClass.BelowNormal;
                    p.WaitForExit();
                }
                else
                {
                    ServiceKoFrMa.debugLog.WriteToLog("Compression level is not set! Cannot continue!", 2);
                }

            }
        }

        public void CompressToRar(string PathToRarFolder, string source, string destination, byte? compressionLevel)
        {
            ServiceKoFrMa.debugLog.WriteToLog("Compressing now...", 6);
            if (PathToRarFolder == null || PathToRarFolder == "")
            {
                ServiceKoFrMa.debugLog.WriteToLog("WinRar is not installed or the path to it is not entered in the .ini file. Cannot compress.", 2);
            }
            else
            {
                if (!(compressionLevel == null))
                {
                    ServiceKoFrMa.debugLog.WriteToLog("Running WinRar as follows: " + Path.Combine(PathToRarFolder, "Rar.exe") + " a -r -s -ma5 -m"+compressionLevel + ' ' + destination + ' ' + source, 8);
                    Process p = new Process();
                    p.StartInfo.FileName = Path.Combine(PathToRarFolder, "Rar.exe");
                    p.StartInfo.Arguments = "a -r -s -ma5 -ep1 -m" + compressionLevel + ' ' + destination + ' ' + source;
                    p.StartInfo.CreateNoWindow = true;
                    p.Start();
                    p.PriorityClass = ProcessPriorityClass.BelowNormal;
                    p.WaitForExit();
                }
                else
                {
                    ServiceKoFrMa.debugLog.WriteToLog("Compression level is not set! Cannot continue!", 2);
                }

            }
        }
    }
}
