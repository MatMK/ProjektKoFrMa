using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

namespace KoFrMaDaemon.Backup
{
    public class Compression
    {
        public void CompressToZip(string source, string destination, byte? compressionLevel)
        {
            ServiceKoFrMa.debugLog.WriteToLog("Compressing now...",6);
            if (compressionLevel==null)
            {
                ServiceKoFrMa.debugLog.WriteToLog("Compression level is not set! Cannot continue!",2);
            }
            ZipFile.CreateFromDirectory(source, destination, (CompressionLevel)compressionLevel,false);
        }

        public void CompressTo7z(string PathTo7zFolder, string source, string destination, byte? compressionLevel)
        {
            ServiceKoFrMa.debugLog.WriteToLog("Compressing now...", 6);
            if (PathTo7zFolder=="")
            {
                ServiceKoFrMa.debugLog.WriteToLog("7-zip is not installed or the path to it is not entered in the .ini file. Cannot compress.", 2);
            }
            else
            {
                System.Diagnostics.Process.Start(Path.Combine(PathTo7zFolder, "7z.exe"), "a -t7z " + destination + ' ' + source + @" -mmt -mx" + compressionLevel);
            }
        }
    }
}
