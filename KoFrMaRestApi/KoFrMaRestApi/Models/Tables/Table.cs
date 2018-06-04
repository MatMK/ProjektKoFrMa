using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;

namespace KoFrMaRestApi.Models.Tables
{
    public class Table
    {
        public List<tbAdminAccounts> GetAdminAccounts()
        {
            List<tbAdminAccounts> tb = new List<tbAdminAccounts>();
            using (MySqlConnection connection = WebApiConfig.Connection())
            using (MySqlCommand command = new MySqlCommand(@"select * from tbAdminAccounts", connection))
            {
                connection.Open();

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        List<int> permissionsList = GetPermissions((int)reader["Id"]);
                        tb.Add(new tbAdminAccounts()
                        {
                            Id = (int)reader["Id"],
                            UserName = (string)reader["UserName"],
                            Email = (string)reader["Email"],
                            Enabled = Convert.ToBoolean(reader["Enabled"]),
                            Permission = permissionsList
                            /*
                            Password = (string)reader["Password"],
                            Token = (string)reader["Token"]*/
                        });
                    }
                }
            }
            return tb;
        }
        private List<int> GetPermissions(int AdminId)
        {
            List<int> permissionsList = new List<int>();
            using (MySqlConnection connection = WebApiConfig.Connection())
            {
                connection.Open();
                using (MySqlCommand getPermissions = new MySqlCommand($"SELECT Permission FROM `tbPermissions` WHERE `IdAdmin` = {AdminId}", connection))
                using (MySqlDataReader permissionsReader = getPermissions.ExecuteReader())
                {
                    while (permissionsReader.Read())
                    {
                        permissionsList.Add((int)permissionsReader["Permission"]);
                    }
                }
            }
            return permissionsList;
        }
        public List<tbDaemons> GetDaemons()
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
                                LastSeen = (object)reader["LastSeen"] == (object)DBNull.Value ? null : (DateTime?)reader["LastSeen"]/*,
                                Password = (Int64)reader["Password"],
                                Token = (string)reader["Token"]*/
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
        public List<tbTasks> GetTasks()
        {
            List<tbTasks> tb = new List<tbTasks>();
            try
            {
                using (MySqlConnection connection = WebApiConfig.Connection())
                using (MySqlCommand command = new MySqlCommand(@"select * from tbTasks where Completed = 0", connection))
                {
                    connection.Open();

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tb.Add(new tbTasks());
                            tb.Last().Id = (int)reader["Id"];
                            tb.Last().IdDaemon = (int)reader["IdDaemon"];
                            tb.Last().Task = (string)reader["Task"];
                            tb.Last().TimeOfExecution = (DateTime)reader["TimeOfExecution"];
                            tb.Last().RepeatInJSON = (object)reader["RepeatInJSON"] == (object)DBNull.Value ? null : (string)reader["RepeatInJSON"];
                            tb.Last().Completed = Convert.ToBoolean(reader["Completed"]);
                        }
                    }
                }
            }
            catch (MySqlException)
            {

            }
            return tb;
        }
        public List<tbTasksCompleted> GetTasksCompleted()
        {
            List<tbTasksCompleted> result = new List<tbTasksCompleted>();
            using (MySqlConnection connection = WebApiConfig.Connection())
            using (MySqlCommand command = new MySqlCommand("SELECT * FROM `tbTasksCompleted`", connection))
            {
                connection.Open();
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new tbTasksCompleted()
                        {
                            Id = (int)reader["Id"],
                            IdDaemon = (int)reader["IdDaemon"],
                            IdTask = (int)reader["IdTask"],
                            BackupJournal = (string)reader["BackupJournal"],
                            TimeOfCompetion = (DateTime)reader["TimeOfCompletition"],
                            DebugLog = (string)reader["DebugLog"],
                            IsSuccessful = Convert.ToBoolean(reader["IsSuccessfull"])
                        });
                    }
                }
            }
            return result;
        }
        public List<tbServerExceptions> GetServerExceptions()
        {
            List<tbServerExceptions> result = new List<tbServerExceptions>();
            using (MySqlConnection connection = WebApiConfig.Connection())
            using (MySqlCommand command = new MySqlCommand("SELECT * FROM `tbRestApiExceptions`", connection))
            {
                connection.Open();
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new tbServerExceptions()
                        {
                            Id = (int)reader["Id"],
                            ExceptionInJson = (string)reader["ExceptionInJson"],
                            TimeOfException = Convert.ToDateTime(reader["TimeOfException"]),
                            Severity = reader["Severity"] == DBNull.Value?null:(int?)reader["Severity"]

                        });
                    }
                }
            }
            return result;
        }
    }
}