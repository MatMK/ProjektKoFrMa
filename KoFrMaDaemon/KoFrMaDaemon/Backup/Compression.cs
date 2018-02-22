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
        DebugLog _debugLog;
        public Compression(DebugLog debugLog)
        {
            _debugLog = debugLog;
        }
        
        public void CompressToZip(string source, string destination, byte compressionLevel)
        {
            ZipFile.CreateFromDirectory(source, destination, (CompressionLevel)compressionLevel,true);
        }
    }
}
