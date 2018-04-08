﻿using KoFrMaRestApi.Models.AdminApp;
using KoFrMaRestApi.Models.Daemon.Task;
using KoFrMaRestApi.Models.Tables;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.MySqlCom
{
    public class MySqlAdmin
    {
        public string RegisterToken(AdminLogin adminLogin)
        {
            string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            using (MySqlConnection connection = WebApiConfig.Connection())
            using (MySqlCommand command = new MySqlCommand(@"UPDATE `tbAdminAccounts` SET `Token`= @Token WHERE `Username` = @Username and `Password` = @Password", connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@Token", token);
                command.Parameters.AddWithValue("@Username", adminLogin.UserName);
                command.Parameters.AddWithValue("@Password", adminLogin.Password);
                int i = command.ExecuteNonQuery();
                if (i == 1)
                    return token;
                else if (i == 0)
                    return null;
                throw new Exception("Duplicate Admin account");
            }
        }
        public bool Authorized(string Username, string Token)
        {
            bool result;
            using (MySqlConnection connection = WebApiConfig.Connection())
            using (MySqlCommand command = new MySqlCommand(@"SELECT * FROM `tbAdminAccounts` WHERE `Username` = @Username and `Token` = @Token", connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@Username", Username);
                command.Parameters.AddWithValue("@Token", Token);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                        result = true;
                    else
                        result = false;
                    reader.Close();
                }
                connection.Close();
            }
            return result;

        }
        public bool HasPermission(int daemonId, int[] reqPermission)
        {
            using (MySqlConnection connection = WebApiConfig.Connection())
            using (MySqlCommand command = new MySqlCommand(@"SELECT `Permission` FROM `tbPermissions` WHERE `IdAdmin` = @IdAdmin", connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@IdAdmin", daemonId);
                int count = 0;
                foreach (int item in reqPermission)
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if ((int)reader["Permission"] == item)
                            {
                                count++;
                            }
                        }
                    }
                }
                if (reqPermission.Length == count)
                {
                    return true;
                }
                return false;
            }
        }
        public int? GetAdminId(string username)
        {
            int? result = null;
            using (MySqlConnection connection = WebApiConfig.Connection())
            using (MySqlCommand command = new MySqlCommand(@"SELECT `Id` FROM `tbAdminAccounts` WHERE `Username` = @username", connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@username", username);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    int count = 0;
                    while (reader.Read())
                    {
                        count++;
                        result = (int)reader["Id"];
                    }
                    if (count < 1)
                    {
                        throw new Exception("Multiple admin accounts with the same username");
                    }
                    return result;
                }
            }

        }
        /// <summary>
        /// Uploads task to mySql database
        /// </summary>
        /// <param name="tasks">Defines task</param>
        public void SetTasks(List<SetTasks> tasks)
        {
            using (MySqlConnection connection = WebApiConfig.Connection())
            {
                connection.Open();
                foreach (var item in tasks)
                {
                    using (MySqlCommand command = new MySqlCommand("INSERT INTO `tbTasks` VALUES (null, @DaemonId, @Task, @DateOfCompletion, @Repeating,0)", connection))
                    {
                        Task task = null;
                        using (MySqlCommand TaskId = new MySqlCommand("SELECT `auto_increment` FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'tbTasks'", connection))
                        using (MySqlDataReader reader = TaskId.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                task = new Task()
                                {
                                    IDTask = Convert.ToInt32(reader["auto_increment"]),
                                    TimeToBackup = item.TimeToBackup,
                                    SourceOfBackup = item.SourceOfBackup,
                                    Destinations = item.Destinations,
                                    LogLevel = item.LogLevel,
                                    ScriptBefore = item.ScriptBefore,
                                    ScriptAfter = item.ScriptAfter,
                                    TemporaryFolderMaxBuffer = item.TemporaryFolderMaxBuffer,
                                    InProgress = false,
                                    BackupJournalSource = null
                                };
                            }
                        }
                        dynamic Repeating;
                        if (item.ExecutionTimes != null)
                            Repeating = JsonConvert.SerializeObject(item.ExecutionTimes);
                        else
                            Repeating = DBNull.Value;
                        command.Parameters.AddWithValue("@DaemonId", item.DaemonId);
                        command.Parameters.AddWithValue("@Task", JsonConvert.SerializeObject(task));
                        command.Parameters.AddWithValue("@DateOfCompletion", item.TimeToBackup);
                        command.Parameters.AddWithValue("@Repeating", Repeating);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
        public void AlterTable(ChangeTable changeTable)
        {
            using (MySqlConnection connection = WebApiConfig.Connection())
            using (MySqlCommand command = new MySqlCommand($"UPDATE `{changeTable.TableName}` SET `{changeTable.ColumnName}` = @Value WHERE `Id` = {changeTable.Id};", connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@Value", changeTable.Value);
                command.ExecuteNonQuery();
            }
        }
    }
}