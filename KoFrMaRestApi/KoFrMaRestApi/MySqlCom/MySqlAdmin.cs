using KoFrMaRestApi.Models;
using KoFrMaRestApi.Models.AdminApp;
using KoFrMaRestApi.Models.AdminApp.GetList;
using KoFrMaRestApi.Models.AdminApp.PostAdmin;
using KoFrMaRestApi.Models.AdminApp.RepeatingTasks;
using KoFrMaRestApi.Models.Daemon.Task;
using KoFrMaRestApi.Models.Daemon.Task.BackupJournal;
using KoFrMaRestApi.Models.Tables;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.MySqlCom
{
    /// <summary>
    /// Used for comunnication with SQL
    /// </summary>
    public class MySqlAdmin
    {
        Bcrypter Verification = new Bcrypter();
        Token TokenService = new Token();
        /// <summary>
        /// Returns new token
        /// </summary>
        /// <param name="adminLogin">Admin login credentials</param>
        /// <returns>new token</returns>
        public string RegisterToken(AdminLogin adminLogin)
        {
            string DatabasePassword = "";
            string token = TokenService.GenerateToken();
            using (MySqlConnection connection = WebApiConfig.Connection())
            using (MySqlCommand command = new MySqlCommand(@"SELECT `Password` FROM `tbAdminAccounts` WHERE `Username` = @username", connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@Username", adminLogin.UserName);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    int NumberOfAdmins = 0;
                    while (reader.Read())
                    {
                        NumberOfAdmins++;
                        DatabasePassword = (string)reader["Password"];
                    }
                    if (NumberOfAdmins == 0)
                    {
                        throw new Exception("No admin with this username");
                    }
                    if (NumberOfAdmins > 1)
                    {
                        throw new Exception("Multiple admin accounts with the same username");
                    }
                }
            }
            using (MySqlConnection connection = WebApiConfig.Connection())
            using (MySqlCommand command = new MySqlCommand(@"SELECT `Password` FROM `tbAdminAccounts` WHERE `Username` = @username", connection))
            {
                connection.Open();
                if (Verification.PasswordMatches(adminLogin.Password, DatabasePassword))
                {
                    command.CommandText = $"UPDATE `tbAdminAccounts` SET `Token`= @Token WHERE `Username` = @Username and `Enabled` = 1";
                    command.Parameters.AddWithValue("@Token", token);
                    command.Parameters.AddWithValue("@Username", adminLogin.UserName);
                    command.ExecuteNonQuery();
                    return token;
                }
                else
                    return null;
            }
        }
        /// <summary>
        /// Checks if admin is authorized
        /// </summary>
        /// <param name="Username">admin's username</param>
        /// <param name="Token">admin's token</param>
        /// <returns>True if admin is authorized</returns>
        public bool Authorized(string Username, string Token)
        {
            bool result;
            if (!TokenService.IsValid(Token))
            {
                return false;
            }
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
        /// <summary>
        /// Checks if admin has valid permission
        /// </summary>
        /// <param name="adminid">Id of the admin in SQL database, use <see cref="GetAdminId(string)"/> to get Id</param>
        /// <param name="reqPermission">Array of permissions</param>
        /// <returns>True if admin has all permissions</returns>
        public bool HasPermission(int adminid, int[] reqPermission)
        {
            using (MySqlConnection connection = WebApiConfig.Connection())
            using (MySqlCommand command = new MySqlCommand(@"SELECT `Permission` FROM `tbPermissions` WHERE `IdAdmin` = @IdAdmin", connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@IdAdmin", adminid);
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
        /// <summary>
        /// Gets admin id from database
        /// </summary>
        /// <param name="username">Admin's usernmae</param>
        /// <returns>Integer id of admin in database or null if there is no admin</returns>
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
                    using (MySqlCommand command = new MySqlCommand("INSERT INTO `tbTasks` VALUES (null, @DaemonId, @Task, @DateOfCompletion,@IdPreviousTask, @BackupTypePlan,@Repeating,0)", connection))
                    {
                        TaskRepeating taskRepeating = new TaskRepeating()
                        {
                            ExceptionDates = item.ExecutionTimes.ExceptionDates,
                            ExecutionTimes = item.ExecutionTimes.ExecutionTimes,
                            RepeatTill = item.ExecutionTimes.RepeatTill,
                            Repeating = new TimeSpan(0, 0, item.ExecutionTimes.Repeating)
                        };
                        taskRepeating.ExecutionTimes.Sort();
                        Task task = new Task()
                        {
                            IDTask = NextAutoIncrement("tbTasks"),
                            Destinations = item.Destinations,
                            LogLevel = item.LogLevel,
                            ScriptBefore = item.ScriptBefore,
                            ScriptAfter = item.ScriptAfter,
                            TemporaryFolderMaxBuffer = item.TemporaryFolderMaxBuffer,
                            InProgress = false,
                            TimeToBackup = taskRepeating.ExecutionTimes[0]
                        };
                        if (item.FollowupTo == 0)
                        {
                            task.Sources = item.Sources;
                        }
                        else
                        {
                            command.CommandText = "SELECT * FROM `tbTasksCompleted` WHERE `Id` = " + item.FollowupTo;
                            using (MySqlDataReader reader = command.ExecuteReader())
                            {
                                int i = 0;
                                while (reader.Read())
                                {
                                    if ((string)reader["BackupJournal"] != "null" && reader["BackupJournal"] != DBNull.Value)
                                    {
                                        i++;
                                        task.Sources = JsonSerializationUtility.Deserialize<BackupJournalObject>(Verification.Base64Decode((string)reader["BackupJournal"]));
                                        break;
                                    }
                                }
                                if (i == 0)
                                {
                                    throw new Exception("Cannot follow up to task with no backup journal");
                                }
                            }
                        }
                        dynamic Repeating;
                        command.CommandText = "INSERT INTO `tbTasks` VALUES (null, @DaemonId, @Task, @DateOfCompletion,@IdPreviousTask, @BackupTypePlan,@Repeating,0)";
                        if (item.ExecutionTimes != null)
                            Repeating = JsonSerializationUtility.Serialize(taskRepeating);
                        else
                            throw new Exception("Task repeating cannot be null");
                        command.Parameters.AddWithValue("@DaemonId", item.DaemonId);
                        command.Parameters.AddWithValue("@Task", JsonSerializationUtility.Serialize(task));
                        command.Parameters.AddWithValue("@DateOfCompletion", taskRepeating.ExecutionTimes[0]);
                        command.Parameters.AddWithValue("@Repeating", Repeating);
                        command.Parameters.AddWithValue("@IdPreviousTask", item.FollowupTo);
                        command.Parameters.AddWithValue("@BackupTypePlan", item.FullAfterBackup);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
        /// <summary>
        /// Adds admin to database
        /// </summary>
        /// <param name="addAdmin">New admin data</param>
        public void AddAdmin(AddAdmin addAdmin)
        {
            using (MySqlConnection connection = WebApiConfig.Connection())
            using (MySqlCommand command = new MySqlCommand($"INSERT INTO `tbAdminAccounts`() VALUES (null, @Username, @Email, @Enable, @Password, null)", connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@Email", addAdmin.Email);
                command.Parameters.AddWithValue("@Username", addAdmin.Username);
                command.Parameters.AddWithValue("@Enable", addAdmin.Enabled);
                command.Parameters.AddWithValue("@Password", Verification.BcryptPasswordInBase64(addAdmin.Password));
                command.ExecuteNonQuery();
                int AdminId = NextAutoIncrement("tbAdminAccounts") - 1;
                foreach (var item in addAdmin.Permissions)
                {
                    command.CommandText = $"INSERT INTO `tbPermissions`() VALUES (null,{item},{AdminId})";
                    command.ExecuteNonQuery();
                }
            }
        }
        /// <summary>
        /// Changes table value
        /// </summary>
        /// <param name="changeTable">Data to change value</param>
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
        public void AlterTable(ChangeTable changeTable, string IdColumnName)
        {
            using (MySqlConnection connection = WebApiConfig.Connection())
            using (MySqlCommand command = new MySqlCommand($"UPDATE `{changeTable.TableName}` SET `{changeTable.ColumnName}` = @Value WHERE {IdColumnName} = {changeTable.Id};", connection))
            {

                connection.Open();
                command.Parameters.AddWithValue("@Value", changeTable.Value);
                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// returns next autoincrement of id
        /// </summary>
        /// <param name="table">Table name</param>
        /// <returns></returns>
        public int NextAutoIncrement(string table)
        {
            using (MySqlConnection connection = WebApiConfig.Connection())
            using (MySqlCommand command = new MySqlCommand($"SELECT `auto_increment` FROM INFORMATION_SCHEMA.TABLES WHERE table_name = '{table}'", connection))
            {
                connection.Open();
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return Convert.ToInt32(reader["auto_increment"]);
                    }
                    else
                    {
                        throw new Exception("Table does not exist");
                    }
                }
            }
        }
        /// <summary>
        /// Deletes admin's token from database
        /// </summary>
        /// <param name="AdminId">Id of the admin in SQL database, use <see cref="GetAdminId(string)"/> to get Id</param>
        public void LogOut(int AdminId)
        {
            using (MySqlConnection connection = WebApiConfig.Connection())
            using (MySqlCommand command = new MySqlCommand($"UPDATE `tbAdminAccounts` SET  `Token` = null WHERE `Id` = {AdminId}", connection))
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// Deletes a row in table
        /// </summary>
        /// <param name="deleteRow">Data for deleting table</param>
        public void DeleteRow(DeleteRowRequest deleteRow)
        {
            using (MySqlConnection connection = WebApiConfig.Connection())
            using (MySqlCommand command = new MySqlCommand($"DELETE FROM `{deleteRow.TableName}` WHERE Id = {deleteRow.Id}", connection))
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// Checks if username exists
        /// </summary>
        /// <param name="exists"><see cref="ExistsRequest"/></param>
        /// <returns>True if username already exists</returns>
        public bool Exists(ExistsRequest exists)
        {
            using (MySqlConnection connection = WebApiConfig.Connection())
            using (MySqlCommand command = new MySqlCommand($"SELECT * FROM `{exists.TableName}` WHERE {exists.Column} = \"{exists.Value}\"", connection))
            {
                connection.Open();
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return true;
                    }
                    return false;
                }
            }
        }
        /// <summary>
        /// Changes permission
        /// </summary>
        /// <param name="changePermission"><see cref="ChangePermission"/></param>
        public void AlterPermissions(ChangePermission changePermission)
        {
            using (MySqlConnection connection = WebApiConfig.Connection())
            using (MySqlCommand command = new MySqlCommand($"DELETE FROM `tbPermissions` WHERE `IdAdmin` = {changePermission.AdminId}", connection))
            {
                connection.Open();
                command.ExecuteNonQuery();
                foreach (int item in changePermission.Permissions)
                {
                    command.CommandText = $"INSERT INTO `tbPermissions`(`Id`, `Permission`, `IdAdmin`) VALUES (null,{item},{changePermission.AdminId})";
                    command.ExecuteNonQuery();
                }
            }
        }
        /// <summary>
        /// Updates password
        /// </summary>
        /// <param name="password">Admin' username</param>
        /// <param name="username">New password</param>
        public void UpdatePassword(string password, string username)
        {
            using (MySqlConnection connection = WebApiConfig.Connection())
            using (MySqlCommand command = new MySqlCommand("UPDATE `tbAdminAccounts` SET `Password`= @password WHERE `Username` = @username", connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", Verification.BcryptPasswordInBase64(password));
                int i = command.ExecuteNonQuery();
                if (i == 0)
                {
                    throw new Exception("No admin with this username");
                }
                if (i < 1)
                {
                    throw new Exception("Multiple admin accounts with the same username");
                }
            }
        }
        public string SelectToken(string password, string username)
        {
            using (MySqlConnection connection = WebApiConfig.Connection())
            using (MySqlCommand command = new MySqlCommand("SELECT * FROM `tbAdminAccounts` WHERE `Username` = @username and `Password` = @password", connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    int count = 0;
                    string result = null;
                    while (reader.Read())
                    {
                        count++;
                        result = (string)reader["Token"];

                    }
                    if (result == null)
                    {
                        result = this.RegisterToken(new AdminLogin() { Password = password, UserName = username });
                    }
                    return result;
                }
            }
        }
        public void UpdateEmail(int AdminId, EditEmailRequest email)
        {
            using (MySqlConnection connection = WebApiConfig.Connection())
            using (MySqlCommand command = new MySqlCommand("DELETE FROM `tbEmailPreferences` WHERE `IdAdmin` = " + AdminId, connection))
            {
                connection.Open();
                command.ExecuteNonQuery();
                if (email.RecieveMail)
                {
                    TaskRepeating rep = new TaskRepeating()
                    {
                        ExceptionDates = email.Repeating.ExceptionDates,
                        ExecutionTimes = email.Repeating.ExecutionTimes,
                        Repeating = new TimeSpan(0,0,email.Repeating.Repeating),
                        RepeatTill = email.Repeating.RepeatTill
                    };
                    command.CommandText = $"INSERT INTO `tbEmailPreferences`(`IdAdmin`, `RepeatInJSON`, `RecievingEmail`) VALUES ({AdminId},@repeating,@email)";
                    command.Parameters.AddWithValue("@repeating", JsonSerializationUtility.Serialize(rep));
                    command.Parameters.AddWithValue("@email", GetAdminEmail(AdminId));
                    command.ExecuteNonQuery();
                }
            }
        }
        private string GetAdminEmail(int Id)
        {
            using (MySqlConnection connection = WebApiConfig.Connection())
            using (MySqlCommand command = new MySqlCommand("SELECT * FROM `tbAdminAccounts` WHERE `Id` = " + Id, connection))
            {
                connection.Open();
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return (string)reader["Email"];
                    }
                    throw new Exception("No admin with such id");
                }
            }
        }
        public EditEmailRequest GetEmailData(int AdminId)
        {
            using (MySqlConnection connection = WebApiConfig.Connection())
            using (MySqlCommand command = new MySqlCommand("SELECT * FROM `tbEmailPreferences` WHERE Id = " + AdminId, connection))
            {
                connection.Open();
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var i = new EditEmailRequest();
                        i.RecieveMail = true;
                        i.Repeating = JsonSerializationUtility.Deserialize<TaskRepeatingNoTimespan>((string)reader["RepeatInJSON"]);
                        return i;
                    }
                    return new EditEmailRequest() { RecieveMail = false };
                }
            }
        }
        public TimerTicks GetTimerTick(int DaemonId)
        {
            using (MySqlConnection connection = WebApiConfig.Connection())
            using (MySqlCommand command = new MySqlCommand("SELECT * FROM `tbDaemons` WHERE `Id` = " + DaemonId, connection))
            {
                TimerTicks timers = new TimerTicks();
                connection.Open();
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        timers.TimerTick = (int)reader["TimerTick"];
                        timers.AfterStart = (int)reader["TimerOnStart"];
                        timers.AfterFailed = (int)reader["TimerAfterFail"];
                        return timers;
                    }
                    return null;
                }
            }
        }
    }
}