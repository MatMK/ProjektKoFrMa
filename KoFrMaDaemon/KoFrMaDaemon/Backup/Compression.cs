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
        public void CompressToZip(string source, string destination, byte? compressionLevel, int? splitAfter)
        {
            ServiceKoFrMa.debugLog.WriteToLog("Inicializing compression to zip...", 6);

            if (!(compressionLevel == null))
            {
                if (splitAfter != null && splitAfter != 0)
                {
                    ServiceKoFrMa.debugLog.WriteToLog("Backup will be stored in one file.", 7);
                    if (!File.Exists(destination))
                    {
                        ServiceKoFrMa.debugLog.WriteToLog("Creating zip now...", 6);
                        ZipFile.CreateFromDirectory(source, destination, (CompressionLevel)compressionLevel, false);
                    }
                    else
                    {
                        ServiceKoFrMa.debugLog.WriteToLog("File exists -> adding files to existing archive", 6);
                        //nefunguje
                        ZipFile.CreateFromDirectory(source, destination, (CompressionLevel)compressionLevel, false);
                    }
                }
                else
                {
                    ServiceKoFrMa.debugLog.WriteToLog("Backup will be stored in multiple files split after "+splitAfter+"MB ", 5);
                    Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile();
                    zip.AddDirectory(source);
                    zip.MaxOutputSegmentSize = 1048576 * (int)splitAfter;
                    zip.Save(destination);
                    ServiceKoFrMa.debugLog.WriteToLog("Multiple file compression done, source was devided in "+zip.NumberOfSegmentsForMostRecentSave + " files", 5);
                }
            }
            else
            {
                ServiceKoFrMa.debugLog.WriteToLog("Compression level is not set! Cannot continue!", 2);
            }
            
        }

        public void CompressTo7z(string PathTo7zFolder, string source, string destination, byte? compressionLevel, int? splitAfter)
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
                    
                    Process p = new Process();
                    p.StartInfo.FileName = Path.Combine(PathTo7zFolder, "7z.exe");
                    if (splitAfter!=null&&splitAfter!=0)
                    {
                        ServiceKoFrMa.debugLog.WriteToLog("Running 7-zip as follows: " + Path.Combine(PathTo7zFolder, "7z.exe") + " a -t7z " + destination + ' ' + source + "* -mmt -mx" + compressionLevel, 8);
                        p.StartInfo.Arguments = "a -t7z " + destination + ' ' + source + "* -mmt -mx" + compressionLevel;
                    }
                    else
                    {
                        ServiceKoFrMa.debugLog.WriteToLog("Running 7-zip as follows: " + Path.Combine(PathTo7zFolder, "7z.exe") + " a -t7z " + destination + ' ' + source + "* -mmt -mx" + compressionLevel + "-v" + (int)splitAfter + 'm', 8);
                        p.StartInfo.Arguments = "a -t7z " + destination + ' ' + source + "* -mmt -mx" + compressionLevel+"-v"+(int)splitAfter+'m';
                    }
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

        public void CompressToRar(string PathToRarFolder, string source, string destination, byte? compressionLevel, int? splitAfter)
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
                    Process p = new Process();
                    p.StartInfo.FileName = Path.Combine(PathToRarFolder, "Rar.exe");
                    p.StartInfo.CreateNoWindow = true;
                    if (splitAfter != null && splitAfter != 0)
                    {
                        if (!File.Exists(destination))
                        {
                            ServiceKoFrMa.debugLog.WriteToLog("Running WinRar as follows: " + Path.Combine(PathToRarFolder, "Rar.exe") + " a -r -s -ma5 -m" + compressionLevel + ' ' + destination + ' ' + source, 8);
                            p.StartInfo.Arguments = "a -r -s -ma5 -ep1 -m" + compressionLevel + ' ' + destination + ' ' + source;
                        }
                        else
                        {
                            ServiceKoFrMa.debugLog.WriteToLog("File exists -> adding files to existing archive", 6);

                            ServiceKoFrMa.debugLog.WriteToLog("Running WinRar as follows: " + Path.Combine(PathToRarFolder, "Rar.exe") + " u -r -ep1 " + destination + ' ' + source, 8);
                            p.StartInfo.Arguments = "u -r -ep1 " + destination + ' ' + source;
                        }
                    }
                    else
                    {
                        ServiceKoFrMa.debugLog.WriteToLog("Running WinRar as follows: " + Path.Combine(PathToRarFolder, "Rar.exe") + " a -r -s -ma5 -ep1 -m" + compressionLevel + "-v" + (int)splitAfter + ' ' + destination + ' ' + source, 8);
                        p.StartInfo.Arguments = "a -r -s -ma5 -ep1 -m" + compressionLevel + "-v"+(int)splitAfter+' ' + destination + ' ' + source;
                    }


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
