using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.Tables
{
    public static class Table
    {
        public static List<tbAdminAccounts> GetAdminAccounts()
        {
            List<tbAdminAccounts> tb = new List<tbAdminAccounts>();
            try
            {
                using (MySqlConnection connection = WebApiConfig.Connection())
                using (MySqlCommand command = new MySqlCommand(@"select * from tbAdminAccounts", connection))
                {
                    connection.Open();

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tb.Add(new tbAdminAccounts()
                            {
                                Id = (int)reader["Id"],
                                Password = (int)reader["Password"],
                                UserName = (string)reader["UserName"],
                                Token = (string)reader["Token"]
                            });
                        }
                    }
                }
            }
            catch (MySqlException)
            {

            }
            return tb;
        }
        public static List<tbDaemons> GetDaemons()
        {
            List<tbDaemons> tb = new List<tbDaemons>();
            try
            {
                using (MySqlConnection connection = WebApiConfig.Connection())
                using (MySqlCommand command = new MySqlCommand(@"select * from tbDaemons", connection))
                {
                    connection.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tb.Add(new tbDaemons()
                            {
                                Id = (int)reader["Id"],
                                Version = (int)reader["Version"],
                                OS = (string)reader["OS"],
                                PC_Unique = (string)reader["PC_Unique"],
                                Allowed = Convert.ToBoolean(reader["Allowed"]),
                                LastSeen = (object)reader["LastSeen"] == (object)DBNull.Value ? null: (DateTime?)reader["LastSeen"],
                                Password = (Int64)reader["Password"],
                                Token = (string)reader["Token"]
                            });
                        }
                    }
                }
            }
            catch (MySqlException)
            {

            }
            return tb;
        }
        public static List<tbTasks> GetTasks()
        {
            List<tbTasks> tb = new List<tbTasks>();
            try
            {
                using (MySqlConnection connection = WebApiConfig.Connection())
                using (MySqlCommand command = new MySqlCommand(@"select * from tbTasks", connection))
                {
                    connection.Open();

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tb.Add(new tbTasks()
                            {
                                Id = (int)reader["Id"],
                                IdDaemon = (int)reader["IdDaemon"],
                                Task = (string)reader["Task"],
                                TimeOfExecution = (DateTime)reader["TimeOfExecution"],
                                RepeatInJSON = (object)reader["RepeatInJSON"] == (object)DBNull.Value?null:(string)reader["RepeatInJSON"],
                                Completed = Convert.ToBoolean(reader["Completed"])
                            });
                        }
                    }
                }
            }
            catch (MySqlException)
            {

            }
            return tb;
        }

    }
}