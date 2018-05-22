using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KoFrMaDaemon.Backup
{
    public class SourceMSSQL : ISource
    {
        /// <summary>
        /// Address of the MSSQL server from where the backup will be made
        /// </summary>
        public string ServerName { get; set; }
        /// <summary>
        /// Credential to the MSSQL server
        /// </summary>
        public NetworkCredential NetworkCredential { get; set; }
        /// <summary>
        /// Name of the database that will be backuped
        /// </summary>
        public string DatabaseName { get; set; }
    }
}
