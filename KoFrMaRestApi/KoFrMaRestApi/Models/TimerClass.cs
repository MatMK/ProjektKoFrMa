using KoFrMaRestApi.EmailSender;
using KoFrMaRestApi.Models.AdminApp.RepeatingTasks;
using KoFrMaRestApi.Models.Tables;
using KoFrMaRestApi.MySqlCom;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Web;

namespace KoFrMaRestApi.Models
{
    /// <summary>
    /// Contains a timer.
    /// </summary>
    public class TimerClass
    {
        private static TimerClass instance;
        private Timer timer;
        private Mail mail = new Mail();
        private MySqlAdmin mySql = new MySqlAdmin();
        /// <summary>
        /// Returns an instance of this class
        /// </summary>
        /// <returns>An instance of <see cref="TimerClass"/></returns>
        public static TimerClass GetInstance()
        {
            if (TimerClass.instance == null)
                TimerClass.instance = new TimerClass();
            return TimerClass.instance;
        }
        private TimerClass()
        {
        }
        /// <summary>
        /// Starts a timer, should only be called once;
        /// </summary>
        public void StartTimer()
        {
            /*
            CorrectTime(new TaskRepeating()
            {
                ExecutionTimes = new List<DateTime>() { new DateTime(2018, 5, 24, 16, 50, 0), new DateTime(2018, 5, 23, 16, 50, 0), new DateTime(2018, 5, 24, 16, 10, 0) },
                ExceptionDates = new List<ExceptionDate>()
                {
                    new ExceptionDate()
                    {
                        Start = new DateTime(2018, 5, 23, 16, 49, 0),
                        End = new DateTime(2018, 5, 23, 16, 51, 0)
                    }
                },
                Repeating = new TimeSpan(2, 0, 0, 0),
                RepeatTill = new DateTime(2018,5,25)
            },1, "tbEmailPreferences", "RepeatInJSON");*/
            if (timer == null)
            {
                timer = new Timer(10000);
                timer.Elapsed += OnTimedEvent;
                timer.Enabled = true;
            }
            else
            {
                throw new Exception("Timer already running");
            }
        }
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            using (MySqlConnection connection = WebApiConfig.Connection())
            using (MySqlCommand command = new MySqlCommand("SELECT * FROM `tbEmailPreferences`", connection))
            {
                connection.Open();
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["RepeatInJSON"] != DBNull.Value)
                        {
                            EmailSettings email = new EmailSettings() { EmailAddress = (string)reader["RecievingEmail"], SendOnlyFailed = Convert.ToBoolean(reader["SendOnlyFailed"]) };
                            if (this.CorrectTime(JsonSerializationUtility.Deserialize<TaskRepeating>((string)reader["RepeatInJSON"]), (int)reader["id"], "tbEmailPreferences", "RepeatInJSON"))
                            {
                                this.mail.SendEmail(new List<EmailSettings>() { email });
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Checks TaskRepeat, extends all dates to be higher than current date.
        /// </summary>
        /// <param name="taskRepeating">times to see if it's time</param>
        /// <param name="Id">Id of column to edit time in</param>
        /// <param name="TableName">Name of the table to edit time in</param>
        /// <param name="ColumnName">Name of the column to edit value in</param>
        /// <returns>True if it's time to execute a task</returns>
        public bool CorrectTime(TaskRepeating taskRepeating, int Id, string TableName, string ColumnName)
        {
            taskRepeating.ExecutionTimes.Sort();
            taskRepeating.ExceptionDates.Sort();
            bool eligible = false;
            foreach (DateTime item in taskRepeating.ExecutionTimes)
            {
                if (item <= DateTime.Now && eligible == false)
                {
                    eligible = true;
                    foreach (var value in taskRepeating.ExceptionDates)
                    {
                        if (item < value.End && item > value.Start)
                        {
                            eligible = false;
                            break;
                        }
                    }
                }
            }
            this.TaskExtend(taskRepeating, Id, TableName, ColumnName);
            return eligible;
        }
        /// <summary>
        /// Updates TaskRepeating in database.
        /// </summary>
        /// <param name="taskRepeating">times to edit</param>
        /// <param name="Id">Id of column to edit time in</param>
        /// <param name="TableName">Name of the table to edit time in</param>
        /// <param name="ColumnName">Name of the column to edit value in</param>
        public void TaskExtend(TaskRepeating taskRepeating, int Id, string TableName, string ColumnName)
        {
            TaskRepeating reference = taskRepeating;
            taskRepeating.ExecutionTimes.Sort();
            for (int i = taskRepeating.ExecutionTimes.Count-1; i >= 0; i--)
            {
                if (taskRepeating.ExecutionTimes[i] <= DateTime.Now)
                {
                    while (taskRepeating.ExecutionTimes[i] <= DateTime.Now)
                    {
                        taskRepeating.ExecutionTimes[i] = taskRepeating.ExecutionTimes[i] + taskRepeating.Repeating;
                    }
                    if (taskRepeating.ExecutionTimes[i] >= taskRepeating.RepeatTill)
                    {
                        taskRepeating.ExecutionTimes.RemoveAt(i);
                    }
                }
            }
            if (taskRepeating.ExecutionTimes.Count == 0)
            {
                mySql.AlterTable(new ChangeTable() { Id = Id, TableName = TableName, ColumnName = ColumnName, Value = DBNull.Value });
            }
            else if (taskRepeating.Equals(reference))    
            {
                mySql.AlterTable(new ChangeTable() {Id = Id, TableName = TableName, ColumnName = ColumnName, Value = JsonSerializationUtility.Serialize(taskRepeating) });
            }
        }
    }
}