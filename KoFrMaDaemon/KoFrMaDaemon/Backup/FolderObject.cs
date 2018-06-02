using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoFrMaDaemon.Backup
{
    public class FolderObject
    {
        /// <summary>
        /// Full path to the folder
        /// </summary>
        public string FullPath { get; set; }
        /// <summary>
        /// Path to the folder relative to where the source foler is
        /// </summary>
        public string RelativePath { get; set; }
        /// <summary>
        /// When the folder was created (Folder attribute)
        /// </summary>
        public DateTime CreationTimeUtc { get; set; }
        /// <summary>
        /// When the folder changed its contains for the last time (Folder attribute)
        /// </summary>
        //public DateTime LastWriteTimeUtc { get; set; }
        /// <summary>
        /// Folder attributes (ex. ReadOnly)
        /// </summary>
        public string Attributes { get; set; }
        /// <summary>
        /// Hash of all the other properties for quick comparison
        /// </summary>
        public Int32 HashRow { get; set;}
        /// <summary>
        /// Defines if the file found a match when comparing backup journals, should be always set false unless experimenting, the value will be automatically assigned during the backup process
        /// </summary>
        public bool Paired { get; set; }
    }
}
