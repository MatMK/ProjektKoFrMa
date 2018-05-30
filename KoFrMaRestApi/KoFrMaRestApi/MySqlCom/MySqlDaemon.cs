using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using KoFrMaRestApi.Models.Daemon;
using Newtonsoft.Json;
using KoFrMaRestApi.Models.Daemon.Task;
using KoFrMaRestApi.Models.AdminApp.RepeatingTasks;
using KoFrMaRestApi.Models.Daemon.Task.BackupJournal;
using KoFrMaRestApi.Models;

namespace KoFrMaRestApi.MySqlCom
{
    public class MySqlDaemon
    {
        MySqlAdmin sqlAdmin = new MySqlAdmin();
        /// <summary>
        /// Returns daemons database id.
        /// </summary>
        /// <param name="daemon"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public int GetDaemonId(DaemonInfo daemon, MySqlConnection connection)
        {

            using (MySqlDataReader reader = SelectFromTableByPcId(connection, daemon))
            {
                int count = 0;
                int result = 0;
                while (reader.Read())
                {
                    count++;
                    result = (int)reader["id"];
                }
                if (count == 0)
                    throw new Exception("No daemon with given pc unique");
                if (count > 1)
                    throw new Exception("Multiple daemons with given pc unique");
                return result;
            }
        }
        public int RegisterDaemonAndGetId(DaemonInfo daemon, string Password, MySqlConnection connection)
        {
            using (MySqlDataReader reader = SelectFromTableByPcId(connection, daemon))
            {
                try
                {
                    return GetDaemonId(daemon, connection);
                }
                catch (Exception ex)
                {
                    if (ex.Message == "No daemon with given pc unique")
                    {
                        reader.Close();
                        string SqlInsert = @"insert into tbDaemons values(null, @version, @os, @pc_unique, 1, now(),@password,'')";
                        using (MySqlCommand command = new MySqlCommand(SqlInsert, connection))
                        {
                            command.Parameters.AddWithValue("@version", daemon.Version);
                            command.Parameters.AddWithValue("@os", daemon.OS);
                            command.Parameters.AddWithValue("@pc_unique", daemon.PC_Unique);
                            command.Parameters.AddWithValue("@password", Password);
                            command.ExecuteNonQuery();
                            return GetDaemonId(daemon, connection);
                        }
                    }
                    else
                    {
                        throw ex;
                    }
                }
            }

        }
        /// <summary>
        /// Gets a list of tasks for daemon to execute
        /// </summary>
        /// <param name="DaemonId">Id of daemon</param>
        /// <param name="connection">open sql connection</param>
        /// <returns>List of tasks for daemon to execute</returns>
        public List<Task> GetTasks(int DaemonId, MySqlConnection connection)
        {
            List<Task> result = new List<Task>();
            MySqlCommand sqlCommand = new MySqlCommand(@"SELECT Task, Id, RepeatInJSON FROM `tbTasks` WHERE `IdDaemon` = @Id and `Completed` = 0", connection);
            sqlCommand.Parameters.AddWithValue("@Id", DaemonId);
            MySqlDataReader reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                var t = JsonSerializationUtility.Deserialize<TaskRepeating>((string)reader["RepeatInJSON"]);
                if (t.ExecutionTimes != null || t.ExecutionTimes.Count() > 0)
                {
                    t.ExecutionTimes.Sort();
                    if (t.ExecutionTimes[0] <= DateTime.Now)
                    {
                        string json = (string)reader["Task"];
                        result.Add(JsonSerializationUtility.Deserialize<Task>(json));
                        result.Last().IDTask = (int)reader["Id"];
                    }
                }
            }
            sqlCommand.Dispose();
            reader.Dispose();
            return result;
        }
        public void TaskCompletionRecieved(TaskComplete task, MySqlConnection connection)
        {
            using (MySqlCommand command = new MySqlCommand(@"SELECT `RepeatInJSON`,`TimeOfExecution` FROM `tbTasks` WHERE Id = @Id", connection))
            {
                command.Parameters.AddWithValue("@Id", task.IDTask);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        if (reader["RepeatInJSON"] == DBNull.Value)
                        {
                            reader.Close();
                            TaskRemove(task, connection, true);
                        }
                        else
                        {
                            string json = (string)reader["RepeatInJSON"];
                            reader.Close();
                            TaskExtend(task, json, connection);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Sets task's state to completed and 
        /// </summary>
        /// <param name="taskComplete">Completed task</param>
        /// <param name="connection">Open MySQL connection</param>
        /// <param name="isSuccessful">Was task successful</param>
        private void TaskRemove(TaskComplete taskComplete, MySqlConnection connection, bool isSuccessful)
        {
            if (isSuccessful)
            {
                using (MySqlCommand command = new MySqlCommand(@"UPDATE `tbTasks` SET `Completed`= 1 WHERE `Id` = @IdTask", connection))
                {
                    command.Parameters.AddWithValue("@IdTask", taskComplete.IDTask);
                    command.ExecuteNonQuery();
                }
            }
            string debugLog = "";
            if (taskComplete.DebugLog != null)
            {
                foreach (string item in taskComplete.DebugLog)
                {
                    debugLog += item + "\n";
                }
            }
            using (MySqlCommand command = new MySqlCommand($"INSERT INTO `tbTasksCompleted`VALUES (null,{GetDaemonId(taskComplete.DaemonInfo, connection)},{taskComplete.IDTask},'{JsonSerializationUtility.Serialize(taskComplete.DatFile)}',@datetime,'{debugLog}',{taskComplete.IsSuccessfull})", connection))
            {
                command.Parameters.AddWithValue("@datetime", taskComplete.TimeOfCompletition);
                command.ExecuteNonQuery();
            }
            int Id = sqlAdmin.NextAutoIncrement("tbTasksCompleted") - 1;
            List<int> toInsert = new List<int>();
            using (MySqlCommand command = new MySqlCommand($"select Id from tbAdminAccounts where 1 order by Id", connection))
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        toInsert.Add((int)reader["Id"]);
                    }
                }
                foreach (int item in toInsert)
                {
                    command.CommandText = $"INSERT INTO `tbTasksCompletedAdminNOTNotified`(`IdTaskCompleted`, `IdAdmin`) VALUES({Id}, {item})";
                    command.ExecuteNonQuery();
                }
            }

        }
        /// <summary>
        /// If next execution time exists new task is created and current is marked as completed else task is removed with <see cref="TaskRemove(TaskComplete, MySqlConnection, bool)"/>
        /// </summary>
        /// <param name="taskComplete">Completed task</param>
        /// <param name="JsonTime">T<see cref="TaskRepeating"/> in json</param>
        /// <param name="connection">open MySqlConnection</param>
        private void TaskExtend(TaskComplete taskComplete, string JsonTime, MySqlConnection connection)
        {
            TaskRepeating repeat = JsonSerializationUtility.Deserialize<TaskRepeating>(JsonTime);
            DateTime nextDate = repeat.ExecutionTimes.Last();
            bool DateChanged = false;
            foreach (var item in repeat.ExecutionTimes)
            {
                if (item > DateTime.Now)
                {
                    bool NotException = HasDateException(item, repeat.ExceptionDates);
                    if (NotException)
                    {
                        nextDate = item;
                        DateChanged = true;
                        break;
                    }
                }
            }
            if (!DateChanged)
            {
                if (repeat.RepeatTill == null)
                {
                    bool DateOk = true;
                    while (repeat.ExecutionTimes[0] < DateTime.Now || !DateOk)
                    {
                        for (int i = 0; i < repeat.ExecutionTimes.Count; i++)
                        {
                            repeat.ExecutionTimes[i] += repeat.Repeating;
                        }
                        DateOk = DateAvailable(repeat.ExecutionTimes, repeat.ExceptionDates);
                    }
                }
                else
                {
                    bool DateOk = true;
                    while (repeat.ExecutionTimes[0] < DateTime.Now || !DateOk)
                    {
                        List<int> ToDelete = new List<int>();
                        for (int i = 0; i < repeat.ExecutionTimes.Count; i++)
                        {
                            if (repeat.ExecutionTimes[i] + repeat.Repeating < repeat.RepeatTill)
                                repeat.ExecutionTimes[i] += repeat.Repeating;
                            else
                                ToDelete.Add(i);
                        }
                        for (int i = ToDelete.Count - 1; i >= 0; i--)
                        {
                            repeat.ExecutionTimes.RemoveAt(ToDelete[i]);
                        }
                        if (repeat.ExecutionTimes.Count == 0)
                            break;
                        DateOk = DateAvailable(repeat.ExecutionTimes, repeat.ExceptionDates);
                    }
                }
                foreach (var item in repeat.ExecutionTimes)
                {
                    if (item > DateTime.Now)
                    {
                        nextDate = item;
                        break;
                    }
                }
            }
            if (repeat.ExecutionTimes.Count == 0)
            {
                TaskRemove(taskComplete, connection, true);
            }
            else
            {
                using (MySqlCommand command = new MySqlCommand(@"SELECT * FROM `tbTasks` WHERE Id = @Id", connection))
                {
                    int IdDaemon;
                    string Task;
                    DateTime TimeOfExecution;
                    string RepeatInJSON;
                    command.Parameters.AddWithValue("@Id", taskComplete.IDTask);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            IdDaemon = (int)reader["IdDaemon"];
                            Task = (string)reader["Task"];
                            TimeOfExecution = (DateTime)reader["TimeOfExecution"];
                            RepeatInJSON = (string)reader["RepeatInJSON"];
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                    Task TaskClass = JsonSerializationUtility.Deserialize<Task>(Task);
                    TaskRemove(taskComplete, connection, true);
                    command.CommandText = "SELECT AUTO_INCREMENT FROM information_schema.TABLES WHERE TABLE_SCHEMA = '3b1_kocourekmatej_db2' AND TABLE_NAME = 'tbTasks'";
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                            taskComplete.IDTask = Convert.ToInt32(reader["AUTO_INCREMENT"]);
                        else
                            throw new Exception();
                    }
                    TaskClass.IDTask = taskComplete.IDTask;
                    TaskClass.Sources = taskComplete.DatFile;
                    Task = JsonSerializationUtility.Serialize(TaskClass);
                    command.CommandText = "INSERT INTO `tbTasks` VALUES (null, @IdDaemon, @Task, @TimeOfExecution, @RepeatInJSON, @Completed)";
                    command.Parameters.AddWithValue("@IdDaemon", IdDaemon);
                    command.Parameters.AddWithValue("@Task", Task);
                    command.Parameters.AddWithValue("@TimeOfExecution", TimeOfExecution);
                    command.Parameters.AddWithValue("@RepeatInJSON", RepeatInJSON);
                    command.Parameters.AddWithValue("@Completed", 0);
                    command.ExecuteNonQuery();
                }
            }
        }
        private bool HasDateException(DateTime item, List<ExceptionDate> ExceptionDates)
        {
            bool result = true;
            if (ExceptionDates != null)
                foreach (var time in ExceptionDates)
                {
                    if (item > time.Start && item < time.End)
                    {
                        result = false;
                    }
                }
            return result;
        }
        private bool DateAvailable(List<DateTime> ExecutionTimes, List<ExceptionDate> ExceptionDates)
        {
            int Dates = 0;
            foreach (var item in ExecutionTimes)
            {
                if (!HasDateException(item, ExceptionDates))
                    Dates++;
            }
            if (Dates == ExecutionTimes.Count)
                return false;
            else
                return true;
        }
        private MySqlDataReader SelectFromTableByPcId(MySqlConnection connection, DaemonInfo daemon)
        {
            string SqlCommand = "SELECT id FROM `tbDaemons` WHERE PC_Unique = @PC_ID";
            MySqlCommand query = new MySqlCommand(SqlCommand, connection);
            query.Parameters.AddWithValue("@PC_ID", daemon.PC_Unique);
            return query.ExecuteReader();
        }
        /// <summary>
        /// Updates lastSeen column in database
        /// </summary>
        public void DaemonSeen(int DaemonId, MySqlConnection connection)
        {
            using (MySqlCommand command = new MySqlCommand(@"UPDATE `tbDaemons` SET `LastSeen`= Now() where Id = @Id", connection))
            {
                command.Parameters.AddWithValue("@Id", DaemonId);
                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// Checks if token is valid
        /// </summary>
        /// <param name="PC_Unique">unique pc identifier</param>
        /// <param name="Token">token</param>
        /// <param name="connection">open MySqlConnection</param>
        /// <returns></returns>
        public bool Authorized(string PC_Unique, string Token, MySqlConnection connection)
        {
            bool result;
            using (MySqlCommand command = new MySqlCommand(@"SELECT * FROM `tbDaemons` WHERE `PC_Unique` = @PC_Unique and `Token` = @Token and Allowed = 1", connection))
            {
                command.Parameters.AddWithValue("@PC_Unique", PC_Unique);
                command.Parameters.AddWithValue("@Token", Token);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                        result = true;
                    else
                        result = false;
                    reader.Close();
                }
            }
            return result;
        }
        /// <summary>
        /// Registers token to database 
        /// </summary>
        /// <param name="PC_Unique">unique pc identifier</param>
        /// <param name="Password">password</param>
        /// <param name="Token">token</param>
        public void RegisterToken(string PC_Unique, string Password, string Token)
        {
            using (MySqlConnection connection = WebApiConfig.Connection())
            using (MySqlCommand command = new MySqlCommand(@"SELECT `Password` FROM `tbDaemons` WHERE `PC_Unique` = @PC_Unique", connection))
            {
                string DatabasePassword = null;
                connection.Open();
                command.Parameters.AddWithValue("@PC_Unique", PC_Unique);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DatabasePassword = (string)reader["Password"];
                    }
                    reader.Close();
                }
                if (DatabasePassword == Password)
                {
                    command.CommandText = @"UPDATE `tbDaemons` SET `Token`= @Token WHERE `PC_Unique` = @PC_Unique";
                    command.Parameters.AddWithValue("@Token", Token);
                    command.ExecuteNonQuery();
                }
            }
        }
        public void TaskFailed(TaskComplete taskComplete)
        {
            using (MySqlConnection connection = WebApiConfig.Connection())
            {
                connection.Open();
                TaskRemove(taskComplete, connection, false);
            }
        }
    }
}