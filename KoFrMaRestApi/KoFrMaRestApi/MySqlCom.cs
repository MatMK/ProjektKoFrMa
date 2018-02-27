using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using KoFrMaRestApi.Models.Daemon;
using Newtonsoft.Json;

namespace KoFrMaRestApi
{
    public class MySqlCom
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
        /// <returns>Vrací list tasku pro daemona</returns>
        public List<Tasks> GetTasks(string DaemonId, MySqlConnection connection)
        {
            List<Tasks> result = new List<Tasks>();
            MySqlCommand sqlCommand = new MySqlCommand(@"SELECT Task, TimeOfExecution FROM `tbTasks` WHERE `IdDaemon` = @Id", connection);
            sqlCommand.Parameters.AddWithValue("@Id", DaemonId);
            MySqlDataReader reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                if (Convert.ToDateTime(reader["TimeOfExecution"]) <= DateTime.Now)
                {
                    string json = (string)reader["Task"];

                    result.Add(JsonConvert.DeserializeObject<Tasks>(json));
                    //Pouzit pokud v databazy budeme uchovavat listy tasku
                    //result.AddRange(JsonConvert.DeserializeObject<List<Tasks>>(json));

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
            using (MySqlCommand command = new MySqlCommand(@"SELECT `RepeatInMinutes`,`TimeOfExecution` FROM `tbTasks` WHERE Id = @Id", connection))
            {
                command.Parameters.AddWithValue("@Id", task.IDTask);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        if (reader["RepeatInMinutes"] == System.DBNull.Value)
                        {
                            reader.Close();
                            TaskRemove(task, connection);
                        }
                        else
                        {
                            int minutes = (int)reader["RepeatInMinutes"];
                            DateTime time = (DateTime)reader["TimeOfExecution"];
                            reader.Close();
                            TaskExtend(task, (int)minutes,time, connection);
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
        public void TaskRemove(TaskComplete task, MySqlConnection connection)
        {
            using (MySqlCommand command = new MySqlCommand(@"DELETE FROM `tbTasks` WHERE `Id` = @IdTask",connection))
            {
                command.Parameters.AddWithValue("@IdTask", task.IDTask);
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
        public void TaskExtend(TaskComplete task,int TimeInMinutes,DateTime time, MySqlConnection connection)
        {
            DateTime Repeat = time + TimeSpan.FromMinutes(TimeInMinutes);
            using (MySqlCommand command = new MySqlCommand(@"UPDATE `tbTasks` SET `TimeOfExecution`= @Time WHERE `Id` = @Id",connection))
            {
                command.Parameters.AddWithValue("@Id", task.IDTask);
                command.Parameters.AddWithValue("@Time", Repeat);
                command.ExecuteNonQuery();
            }
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
                        result =  false;
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
