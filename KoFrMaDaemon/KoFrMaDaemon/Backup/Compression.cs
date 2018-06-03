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
        /// <summary>
        /// Compresses folder set by parameter <paramref name="source"/> into ZIP format with the ability to set desired compression level and split files into multiple archives
        /// </summary>
        /// <param name="source">Path to folder that will be compressed</param>
        /// <param name="destination">Path to name of the file where the zip will be placed</param>
        /// <param name="compressionLevel">Desired level of compression, see the DestinationZip for possible values <see cref = "KoFrMaDaemon.Backup.DestinationZip"/></param>
        /// <param name="splitAfter">Number of MiB by what the zip will be split (null if no splitting)</param>
        public void CompressToZip(string source, string destination, byte? compressionLevel, int? splitAfter)
        {
            KoFrMaDaemon.debugLog.WriteToLog("Inicializing compression to zip...", 6);

            if (!(compressionLevel == null))
            {
                if (splitAfter == null || splitAfter == 0)
                {
                    KoFrMaDaemon.debugLog.WriteToLog("Backup will be stored in one file.", 7);
                    if (!File.Exists(destination))
                    {
                        KoFrMaDaemon.debugLog.WriteToLog("Creating zip now...", 6);
                        ZipFile.CreateFromDirectory(source, destination, (CompressionLevel)compressionLevel, false);
                    }
                    else
                    {
                        KoFrMaDaemon.debugLog.WriteToLog("File exists -> adding files to existing archive", 6);
                        //nefunguje
                        ZipFile.CreateFromDirectory(source, destination, (CompressionLevel)compressionLevel, false);
                    }
                }
                else
                {
                    KoFrMaDaemon.debugLog.WriteToLog("Backup will be stored in multiple files split after "+splitAfter+"MiB ", 5);
                    Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile();
                    zip.AddDirectory(source);
                    zip.MaxOutputSegmentSize = 1048576 * (int)splitAfter;
                    zip.Save(destination);
                    KoFrMaDaemon.debugLog.WriteToLog("Multiple file compression done, source was devided in "+zip.NumberOfSegmentsForMostRecentSave + " files", 5);
                }
            }
            else
            {
                KoFrMaDaemon.debugLog.WriteToLog("Compression level is not set! Cannot continue!", 2);
            }
            
        }
        /// <summary>
        /// Compresses folder set by parameter <paramref name="source"/> into 7z format with the ability to set desired compression level and split files into multiple archives by using third party 7z.exe
        /// </summary>
        /// <param name="PathTo7zFolder">Path to folder where the third party 7-Zip programm is installed</param>
        /// <param name="source">Path to folder that will be compressed</param>
        /// <param name="destination">Path to name of the file where the 7z will be placed</param>
        /// <param name="compressionLevel">Desired level of compression, see the Destination7z for possible values <see cref="KoFrMaDaemon.Backup.Destination7z"/></param>
        /// <param name="splitAfter">Number of MiB by what the 7z will be split (null or 0 if no splitting)</param>
        public void CompressTo7z(string PathTo7zFolder, string source, string destination, byte? compressionLevel, int? splitAfter)
        {
            KoFrMaDaemon.debugLog.WriteToLog("Compressing now...", 6);
            if (PathTo7zFolder == null || PathTo7zFolder =="")
            {
                KoFrMaDaemon.debugLog.WriteToLog("7-zip is not installed or the path to it is not entered in the .ini file. Cannot compress.", 2);
            }
            else
            {
                if (!(compressionLevel == null))
                {
                    
                    Process p = new Process();
                    p.StartInfo.FileName = Path.Combine(PathTo7zFolder, "7z.exe");
                    if (splitAfter == null || splitAfter == 0)
                    {
                        KoFrMaDaemon.debugLog.WriteToLog("Running 7-zip as follows: " + Path.Combine(PathTo7zFolder, "7z.exe") + " a -t7z " + destination + ' ' + source + "* -mmt -mx" + compressionLevel, 8);
                        p.StartInfo.Arguments = "a -t7z " + destination + ' ' + source + "* -mmt -mx" + compressionLevel;
                    }
                    else
                    {
                        KoFrMaDaemon.debugLog.WriteToLog("Running 7-zip as follows: " + Path.Combine(PathTo7zFolder, "7z.exe") + " a -t7z " + destination + ' ' + source + "* -mmt -mx" + compressionLevel + "-v" + (int)splitAfter + 'm', 8);
                        p.StartInfo.Arguments = "a -t7z " + destination + ' ' + source + "* -mmt -mx" + compressionLevel+" -v"+(int)splitAfter+'m';
                    }
                    p.StartInfo.CreateNoWindow = true;
                    p.Start();
                    p.PriorityClass = ProcessPriorityClass.BelowNormal;
                    p.WaitForExit();
                }
                else
                {
                    KoFrMaDaemon.debugLog.WriteToLog("Compression level is not set! Cannot continue!", 2);
                }

            }
        }
        /// <summary>
        /// Compresses folder set by parameter <paramref name="source"/> into 7z format with the ability to set desired compression level and split files into multiple archives by using third party 7z.exe
        /// </summary>
        /// <param name="PathToRarFolder">Path to folder where the third party WinRAR programm is installed</param>
        /// <param name="source">Path to folder that will be compressed</param>
        /// <param name="destination">Path to name of the file where the rar will be placed</param>
        /// <param name="compressionLevel">Desired level of compression, see the Destination7z for possible values <see cref=">KoFrMaDaemon.Backup.DestinationRar"/></param>
        /// <param name="splitAfter">Number of MB by what the rar will be split (null or 0 if no splitting)</param>
        public void CompressToRar(string PathToRarFolder, string source, string destination, byte? compressionLevel, int? splitAfter)
        {
            KoFrMaDaemon.debugLog.WriteToLog("Compressing now...", 6);
            if (PathToRarFolder == null || PathToRarFolder == "")
            {
                KoFrMaDaemon.debugLog.WriteToLog("WinRar is not installed or the path to it is not entered in the .ini file. Cannot compress.", 2);
            }
            else
            {
                if (!(compressionLevel == null))
                {
                    Process p = new Process();
                    p.StartInfo.FileName = Path.Combine(PathToRarFolder, "Rar.exe");
                    p.StartInfo.CreateNoWindow = true;
                    if (splitAfter == null || splitAfter == 0)
                    {
                        if (!File.Exists(destination))
                        {
                            KoFrMaDaemon.debugLog.WriteToLog("Running WinRar as follows: " + Path.Combine(PathToRarFolder, "Rar.exe") + " a -r -s -ma5 -m" + compressionLevel + ' ' + destination + ' ' + source, 8);
                            p.StartInfo.Arguments = "a -r -s -ma5 -ep1 -m" + compressionLevel + ' ' + destination + ' ' + source;
                        }
                        else
                        {
                            KoFrMaDaemon.debugLog.WriteToLog("File exists -> adding files to existing archive", 6);

                            KoFrMaDaemon.debugLog.WriteToLog("Running WinRar as follows: " + Path.Combine(PathToRarFolder, "Rar.exe") + " u -r -ep1 " + destination + ' ' + source, 8);
                            p.StartInfo.Arguments = "u -r -ep1 " + destination + ' ' + source;
                        }
                    }
                    else
                    {
                        KoFrMaDaemon.debugLog.WriteToLog("Running WinRar as follows: " + Path.Combine(PathToRarFolder, "Rar.exe") + " a -r -s -ma5 -ep1 -m" + compressionLevel + "-v" + (int)splitAfter + ' ' + destination + ' ' + source, 8);
                        p.StartInfo.Arguments = "a -r -s -ma5 -ep1 -m" + compressionLevel + " -v"+splitAfter*1000+' ' + destination + ' ' + source;
                    }


                    p.Start();
                    p.PriorityClass = ProcessPriorityClass.BelowNormal;
                    p.WaitForExit();
                }
                else
                {
                    KoFrMaDaemon.debugLog.WriteToLog("Compression level is not set! Cannot continue!", 2);
                }

            }
        }
    }
}
