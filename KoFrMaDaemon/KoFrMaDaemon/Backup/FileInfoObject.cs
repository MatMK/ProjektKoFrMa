using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace KoFrMaDaemon.Backup
{
    public class FileInfoObject
    {
        public string FullPath { get; set; }
        public string RelativePath { get; set; }
        public long Length { get; set; }
        public DateTime CreationTimeUtc { get; set; }
        public DateTime LastWriteTimeUtc { get; set; }
        public string Attributes { get; set; }
        public string MD5 { get; set; }
        public Int32 HashRow { get; set; }
        public bool Paired { get; set; }
    }
}
