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
        private ErrorObserver observer = new ErrorObserver();
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
            if (timer == null)
            {
                timer = new Timer(30000);
                timer.Elapsed += OnTimedEvent;
                timer.Enabled = true;
            }
            else
            {
                throw new Exception("Timer already running");
            }
        }
        private async void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            List<ExecuteMail> exec = new List<ExecuteMail>();
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
                            if (this.CorrectTime(JsonSerializationUtility.Deserialize<TaskRepeating>((string)reader["RepeatInJSON"]), 0))
                            {
                                exec.Add(new ExecuteMail(email, (int)reader["IdAdmin"]));
                            }
                            TaskExtendDatabase(JsonSerializationUtility.Deserialize<TaskRepeating>((string)reader["RepeatInJSON"]), (int)reader["id"], "tbEmailPreferences", "RepeatInJSON");
                        }
                    }
                }
                connection.Close();
                connection.Dispose();
            }
            foreach (var item in exec)
            {
                item.Execute();
            }
        }
        /// <summary>
        /// Checks TaskRepeat
        /// </summary>
        /// <param name="taskRepeating">times to see if it's time</param>
        /// <param name="sendBefore">Time to tell how much in future should the task be sent</param>
        /// <returns>True if it's time to execute a task</returns>
        public bool CorrectTime(TaskRepeating taskRepeating, int sendBefore)
        {
            taskRepeating.ExecutionTimes.Sort();
            if (taskRepeating.ExceptionDates != null)
            {
                taskRepeating.ExceptionDates.Sort();
            }
            else
            {
                taskRepeating.ExceptionDates = new List<ExceptionDate>();
            }
            bool eligible = false;
            foreach (DateTime item in taskRepeating.ExecutionTimes)
            {
                if ((item - DateTime.Now).TotalMilliseconds< sendBefore && eligible == false)
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
            return eligible;
        }
        /// <summary>
        /// Updates TaskRepeating in database.
        /// </summary>
        /// <param name="taskRepeating">times to edit</param>
        /// <param name="Id">Id of column to edit time in</param>
        /// <param name="TableName">Name of the table to edit time in</param>
        /// <param name="ColumnName">Name of the column to edit value in</param>
        public TaskRepeating TaskExtendDatabase(TaskRepeating taskRepeating, int Id, string TableName, string ColumnName)
        {
            TaskRepeating reference = taskRepeating;
            taskRepeating.ExecutionTimes.Sort();
            for (int i = taskRepeating.ExecutionTimes.Count - 1; i >= 0; i--)
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
                return null;
            }
            else if (taskRepeating.Equals(reference))
            {
                    mySql.AlterTable(new ChangeTable() { Id = Id, TableName = TableName, ColumnName = ColumnName, Value = JsonSerializationUtility.Serialize(taskRepeating) });
            }
            return taskRepeating;
        }
        /// <summary>
        /// Extends a <see cref="TaskRepeating"/>
        /// </summary>
        /// <param name="taskRepeating">Taskrepeating to extend</param>
        /// <returns>new Taskrepeating</returns>
        public TaskRepeating TaskExtend(TaskRepeating taskRepeating)
        {
            TaskRepeating reference = taskRepeating;
            taskRepeating.ExecutionTimes.Sort();
            for (int i = taskRepeating.ExecutionTimes.Count - 1; i >= 0; i--)
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
                return null;
            }
            return taskRepeating;
        }
    }
}