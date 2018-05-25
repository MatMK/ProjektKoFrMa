using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoFrMaRestApi.Models.Daemon.Task
{
    public interface IDestinationPath
    {
        /// <summary>
        /// Path to the destination of backup, can be path to local folder, FTP server (ftp://...), SFTP server (sftp://) or shared folder (//NASBackup/BackupDestination)
        /// </summary>
        string Path { get; set; }
    }
}
