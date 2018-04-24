using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace KoFrMaDaemon.Backup
{
    class MSSQLBackup
    {
        public void BackupMSSQL(SourceMSSQL source,DirectoryInfo destination)
        {
            SqlConnection connect;
            string con1 = @"Data Source=" + source.ServerName + ";Initial Catalog=" + source.DatabaseName + ";Persist Security Info=True;User ID=" + source.NetworkCredential.UserName + ";Password=" + source.NetworkCredential.Password;
            connect = new SqlConnection(con1);
            connect.Open();
            SqlCommand command;
            command = new SqlCommand(@"backup database " + source.DatabaseName + " to disk ='" + destination.FullName + "\\" + source.DatabaseName +".bak" + "' with init,stats=10", connect);
            command.ExecuteNonQuery();
            connect.Close();
        }

    }
}
