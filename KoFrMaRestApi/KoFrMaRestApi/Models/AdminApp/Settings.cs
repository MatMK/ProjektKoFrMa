using KoFrMaRestApi.Models.Daemon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;

namespace KoFrMaRestApi.Models.AdminApp
{
    public class Settings
    {
        public void SetTasks(List<SetTasks> tasks)
        {
            using (MySqlConnection connection = WebApiConfig.Connection())
            {
                connection.Open();
                foreach (var item in tasks)
                {

                    using (MySqlCommand command = new MySqlCommand("INSERT INTO `tbTasks` VALUES (null, @DaemonId, @Task, @DateOfCompletion, @Repeating)", connection))
                    {
                        Tasks task = null;
                        using (MySqlCommand TaskId = new MySqlCommand("SELECT `auto_increment` FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'tbTasks'", connection))
                        using (MySqlDataReader reader = TaskId.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                task = new Tasks()
                                {
                                    IDTask = Convert.ToInt32(reader["auto_increment"]),
                                    TimeToBackup = item.TimeToBackup,
                                    SourceOfBackup = item.SourceOfBackup,
                                    WhereToBackup = item.WhereToBackup,
                                    TimerValue = item.TimerValue,
                                    LogLevel = item.LogLevel,
                                    CompressionLevel = item.CompressionLevel,
                                    NetworkCredentials = item.NetworkCredentials,
                                    InProgress = item.InProgress
                                };
                            }
                        }
                        command.Parameters.AddWithValue("@DaemonId", item.DaemonId);
                        command.Parameters.AddWithValue("@Task", JsonConvert.SerializeObject(task));
                        command.Parameters.AddWithValue("@DateOfCompletion", item.TimeToBackup);
                        command.Parameters.AddWithValue("@Repeating", item.RepeatingInMinutes);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}