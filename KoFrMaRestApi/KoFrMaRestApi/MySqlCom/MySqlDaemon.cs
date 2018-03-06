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

namespace KoFrMaRestApi.MySqlCom
{
    public class MySqlDaemon
    {
        /// <summary>
        /// Zjistí zda je Daemon už zaregistrovaný, pokud ne, přidá ho do databáze
        /// </summary>
        /// <param name="daemon"></param>
        /// <param name="connection"></param>
        /// <returns>Vrací ID daemona v databázi</returns>
        public string GetDaemonId(DaemonInfo daemon, MySqlConnection connection)
        {

            using (MySqlDataReader reader = SelectFromTableByPcId(connection, daemon))
            {
                if (reader.Read())
                {
                    return reader.GetString(0);
                }
                else
                {/*
                    reader.Close();
                    string SqlInsert = "insert into tbDaemons values(null, @version, @os, @pc_unique, 1, now())";
                    using (MySqlCommand command = new MySqlCommand(SqlInsert, connection))
                    {
                        command.Parameters.AddWithValue("@version", daemon.Version);
                        command.Parameters.AddWithValue("@os", daemon.OS);
                        command.Parameters.AddWithValue("@pc_unique", daemon.PC_Unique);
                        command.ExecuteNonQuery();
                        return GetDaemonId(daemon,connection);
                    }*/
                    return null;
                }
            }

        }
        public string RegisterDaemonAndGetId(DaemonInfo daemon, Int64 Password, MySqlConnection connection)
        {

            using (MySqlDataReader reader = SelectFromTableByPcId(connection, daemon))
            {
                if (reader.Read())
                {
                    return reader.GetString(0);
                }
                else
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
                        return GetDaemonId(daemon,connection);
                    }
                }
            }

        }

        /// <summary>
        /// Vybere tasky pro daemona z databáze
        /// </summary>
        /// <param name="DaemonId">ID daemona v databázy, lze použí GetDaemonId()</param>
        /// <param name="connection"></param>
        /// <returns>Vrací repeat.ExecutionTimes tasku pro daemona</returns>
        public List<Tasks> GetTasks(string DaemonId, MySqlConnection connection)
        {
            List<Tasks> result = new List<Tasks>();
            MySqlCommand sqlCommand = new MySqlCommand(@"SELECT Task, TimeOfExecution, Id FROM `tbTasks` WHERE `IdDaemon` = @Id and `Completed` = 0", connection);
            sqlCommand.Parameters.AddWithValue("@Id", DaemonId);
            MySqlDataReader reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                if (Convert.ToDateTime(reader["TimeOfExecution"]) <= DateTime.Now)
                {
                    string json = (string)reader["Task"];
                    result.Add(JsonConvert.DeserializeObject<Tasks>(json));
                    result.Last().IDTask = (int)reader["Id"];
                }
            }
            sqlCommand.Dispose();
            reader.Dispose();
            return result;
        }
        /// <summary>
        /// Rozhodne zda task z databáze smazat nebo změnit jeho provedení na později
        /// </summary>
        /// <param name="task"></param>
        /// <param name="connection"></param>
        public void TaskCompletionRecieved(TaskComplete task, MySqlConnection connection)
        {
            UpdateBackupJournal(task.IDTask, task.DatFile, connection);
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
                            TaskRemove(task.IDTask, connection);
                        }
                        else
                        {
                            string json = (string)reader["RepeatInJSON"];
                            reader.Close();
                            TaskExtend(task.IDTask, json, connection);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Odstraní task z databáze
        /// </summary>
        /// <param name="task"></param>
        /// <param name="connection"></param>
        private void TaskRemove(int IDTask, MySqlConnection connection)
        {
            using (MySqlCommand command = new MySqlCommand(@"UPDATE `tbTasks` SET `Completed`= 1 WHERE `Id` = @IdTask", connection))
            {
                command.Parameters.AddWithValue("@IdTask", IDTask);
                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// Prodlouží task o dany počet minut
        /// </summary>
        /// <param name="task"></param>
        /// <param name="TimeInMinutes"></param>
        /// <param name="time"></param>
        /// <param name="connection"></param>
        private void TaskExtend(int IdTask, string JsonTime, MySqlConnection connection)
        {
            TaskRepeating repeat = JsonConvert.DeserializeObject<TaskRepeating>(JsonTime);
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
                //pridat check pokud to neni potreba pro dokonceni neake zalohy
                TaskRemove(IdTask, connection);
            }
            else
            {
                using (MySqlCommand command = new MySqlCommand(@"UPDATE `tbTasks` SET `TimeOfExecution`= @Time,`RepeatInJSON` = @NewJson WHERE `Id` = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", IdTask);
                    command.Parameters.AddWithValue("@Time", nextDate);
                    command.Parameters.AddWithValue("@NewJson", JsonConvert.SerializeObject(repeat));
                    command.ExecuteNonQuery();
                }
            }
        }
        private void UpdateBackupJournal(int IdTask, BackupJournalObject backupJournal, MySqlConnection connection)
        {
            using (MySqlCommand command = new MySqlCommand("SELECT `Task` FROM `tbTasks` WHERE `Id` = @IdTask", connection))
            {
                Tasks task = null;
                command.Parameters.AddWithValue("@IdTask", IdTask);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                        task = JsonConvert.DeserializeObject<Tasks>((string)reader["Task"]);
                    reader.Close();
                }
                task.BackupJournalSource = backupJournal;
                command.CommandText = "UPDATE `tbTasks` SET `Task`= @NewTask where Id = @IdTask";
                command.Parameters.AddWithValue("@NewTask", task);
                command.ExecuteNonQuery();
            }
        }
        private bool HasDateException(DateTime item, List<ExceptionDate> ExceptionDates)
        {
            bool result = true;
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
        /// Zapíše do databáze čas kdy byla funkce zavolána
        /// </summary>
        public void DaemonSeen(string DaemonId, MySqlConnection connection)
        {
            using (MySqlCommand command = new MySqlCommand(@"UPDATE `tbDaemons` SET `LastSeen`= Now() where ID = @Id", connection))
            {
                command.Parameters.AddWithValue("@Id", DaemonId);
                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// Kontroluje zda token odpovídá počítači
        /// </summary>
        public bool Authorized(string PC_Unique, string Token, MySqlConnection connection)
        {
            bool result;
            string _token = JsonConvert.DeserializeObject<string>(Token);
            using (MySqlCommand command = new MySqlCommand(@"SELECT * FROM `tbDaemons` WHERE `PC_Unique` = @PC_Unique and `Token` = @Token and Allowed = 1", connection))
            {
                command.Parameters.AddWithValue("@PC_Unique", PC_Unique);
                command.Parameters.AddWithValue("@Token", _token);
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
        public void RegisterToken(string PC_Unique, Int64 Password, string Token)
        {
            using (MySqlConnection connection = WebApiConfig.Connection())
            using (MySqlCommand command = new MySqlCommand(@"SELECT `Password` FROM `tbDaemons` WHERE `PC_Unique` = @PC_Unique", connection))
            {
                int? DatabasePassword = null;
                connection.Open();
                command.Parameters.AddWithValue("@PC_Unique", PC_Unique);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DatabasePassword = (int)reader["Password"];
                    }
                    reader.Close();
                }
                if (DatabasePassword == Password)
                {
                    command.CommandText = @"UPDATE `tbDaemons` SET `Token`= @Token WHERE `PC_Unique` = @PC_Unique";
                    command.Parameters.AddWithValue("@Token", Token);
                    command.Parameters.AddWithValue("@PC_Unique", PC_Unique);
                    command.ExecuteNonQuery();

                }
            }
        }
        public bool RegisterToken(Int64 Password, string Token)
        {
            using (MySqlConnection connection = WebApiConfig.Connection())
            using (MySqlCommand command = new MySqlCommand(@"UPDATE `tbDaemons` SET `Token`= @Token WHERE `Password` = @Password", connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@Token", Token);
                command.Parameters.AddWithValue("@Password", Password);
                if (command.ExecuteNonQuery() == 0)
                    return false;
                return true;
            }
        }
    }
}