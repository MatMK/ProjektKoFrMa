using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoFrMaDaemon.Backup
{
    public class FolderObject
    {
        public string FullPath { get; set; }
        public DateTime CreationTimeUtc { get; set; }
        public DateTime LastWriteTimeUtc { get; set; }
        public string Attributes { get; set; }
        public Int32 HashRow { get; set; }
        public bool Paired { get; set; }
    }
}
