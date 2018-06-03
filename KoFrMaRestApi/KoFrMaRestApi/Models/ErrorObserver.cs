using KoFrMaRestApi.MySqlCom;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Results;

namespace KoFrMaRestApi.Models
{
    public class ErrorObserver : ExceptionLogger
    {
        MySqlAdmin sqlAdmin = new MySqlAdmin();
        /// <summary>
        /// Registers an exception to mysql database
        /// </summary>
        /// <param name="exception">Exception to register to mysql database</param>
        public void RegisterError(Exception exception)
        {
            using (MySqlConnection connection = WebApiConfig.Connection())
            using (MySqlCommand command = new MySqlCommand("INSERT INTO `tbRestApiExceptions`(`ExceptionInJson`, `TimeOfException`, `Severity`) VALUES (@Exception, @TimeOfException,@severity)", connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@Exception",exception.Message);
                command.Parameters.AddWithValue("@TimeOfException", DateTime.Now);
                command.Parameters.AddWithValue("@severity", DBNull.Value);
                command.ExecuteNonQuery();
                int Id = sqlAdmin.NextAutoIncrement("tbRestApiExceptions") - 1;
                List<int> toInsert = new List<int>();
                command.CommandText = "select Id from tbAdminAccounts where 1 order by Id";
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        toInsert.Add((int)reader["Id"]);
                    }
                }
                foreach (int item in toInsert)
                {
                    command.CommandText = $"INSERT INTO `tbRestApiExceptionsAdminNOTNotified`(IdRestApiExceptions, IdAdmin) VALUES({Id},@Item)";
                    command.Parameters.AddWithValue("@Item", item);
                    command.ExecuteNonQuery();
                }
            }
        }
        /// <summary>
        /// Automatically logs exceptions from controllers
        /// </summary>
        /// <param name="context">Exception context</param>
        public override void Log(ExceptionLoggerContext context)
        {
            this.RegisterError(context.Exception);
        }
    }
}