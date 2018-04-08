using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoFrMaDaemon.Backup
{
    public interface IDestinationPath
    {
        /// <summary>
        /// Cesta cíli zálohy, může odkazovat na lokální disk, ftp server (ftp://...), sftp server (sftp://) nebo sdílené úložiště (//NASBackup/CilZalohy)
        /// </summary>
        string Path { get; set; }
    }
}
