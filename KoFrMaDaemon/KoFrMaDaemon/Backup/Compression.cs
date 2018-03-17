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
    }
}
