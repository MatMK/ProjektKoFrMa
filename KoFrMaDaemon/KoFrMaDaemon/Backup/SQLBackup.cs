using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using MySql.Data.MySqlClient;

namespace KoFrMaDaemon.Backup
{
    class SQLBackup
    {
        /// <summary>
        /// Backups the Microsoft SQL database into file to desired location
        /// </summary>
        /// <param name="source"><c>SourceMSSQL</c> containing all parameters needed for backup</param>
        /// <param name="destination"><c>DirectoryInfo</c> where the backup file will be stored</param>
        public void BackupMSSQL(SourceMSSQL source,DirectoryInfo destination,DebugLog debugLog)
        {
            SqlConnection connect;
            string con1 = @"Data Source=" + source.ServerName + ";Initial Catalog=" + source.DatabaseName + ";Persist Security Info=True;User ID=" + source.NetworkCredential.UserName + ";Password=" + source.NetworkCredential.Password;
            debugLog.WriteToLog("Connecting to database with this SQL command: "+con1, 6);
            connect = new SqlConnection(con1);
            connect.Open();
            SqlCommand command;
            command = new SqlCommand(@"backup database " + source.DatabaseName + " to disk ='" + destination.FullName + "\\" + source.DatabaseName +".bak" + "' with init,stats=10", connect);
            debugLog.WriteToLog("Backuping databse with this SQL command: " + command, 6);
            command.ExecuteNonQuery();
            connect.Close();
        }
        /// <summary>
        /// Backups the MySQL database into file to desired location
        /// </summary>
        /// <param name="source"><c>SourceMySQL</c> containing all parameters needed for backup</param>
        /// <param name="destination"><c>DirectoryInfo</c> where the backup file will be stored</param>
        public void BackupMySQL(SourceMySQL source,DirectoryInfo destination)
        {
            MySqlConnection conn = new MySqlConnection("server=" + source.ServerName + ";user=" + source.NetworkCredential.UserName + ";pwd=" + source.NetworkCredential.Password + ";database=" + source.DatabaseName + ';');
            MySqlCommand cmd = new MySqlCommand();
            MySqlBackup mb = new MySqlBackup(cmd);
            cmd.Connection = conn;
            conn.Open();
            mb.ExportToFile(destination.FullName + @"\" + source.DatabaseName + ".sql");
            conn.Close();
        }

    }
}
