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
        public void RegisterError(Exception exception)
        {
            using (MySqlConnection connection = WebApiConfig.Connection())
            using (MySqlCommand command = new MySqlCommand("INSERT INTO `tbRestApiExceptions`(`ExceptionInJson`, `TimeOfException`, `Severity`) VALUES (@Exception, @TimeOfException,@severity)", connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@Exception",JsonSerializationUtility.Serialize(exception));
                command.Parameters.AddWithValue("@TimeOfException", DateTime.Now);
                command.Parameters.AddWithValue("@severity", DBNull.Value);
                command.ExecuteNonQuery();
            }
        }
        public override void Log(ExceptionLoggerContext context)
        {
            this.RegisterError(context.Exception);
        }
    }
}