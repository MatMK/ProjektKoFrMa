using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.Daemon.Task.BackupJournal
{
    public class FileInfoObject
    {
        /// <summary>
        /// Full path to the file
        /// </summary>
        public string FullPath { get; set; }
        /// <summary>
        /// Relative path to the file to where to source folder is
        /// </summary>
        public string RelativePath { get; set; }
        /// <summary>
        /// Size of the file in bytes
        /// </summary>
        public long Length { get; set; }
        /// <summary>
        /// When the file was created (file attribute)
        /// </summary>
        public DateTime CreationTimeUtc { get; set; }
        /// <summary>
        /// When the file was edited for the last time (file attribute)
        /// </summary>
        public DateTime LastWriteTimeUtc { get; set; }
        /// <summary>
        /// Attributes of the file (ex. ReadOnly)
        /// </summary>
        public string Attributes { get; set; }
        /// <summary>
        /// MD5 hash of the file (content, not name)
        /// </summary>
        public string MD5 { get; set; }
        /// <summary>
        /// Hash of all the other properties for quick comparison
        /// </summary>
        public Int32 HashRow { get; set; }
        /// <summary>
        /// Defines if the file found a match when comparing backup journals, should be always set false unless experimenting, the value will be automatically assigned during the backup process
        /// </summary>
        public bool Paired { get; set; }
    }
}