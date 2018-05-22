using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models
{
    public static class ErrorObserver
    {
        public static void RegisterError(Exception exception)
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
    }
}