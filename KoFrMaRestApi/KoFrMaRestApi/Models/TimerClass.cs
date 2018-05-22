using KoFrMaRestApi.EmailSender;
using KoFrMaRestApi.Models.AdminApp.RepeatingTasks;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Web;

namespace KoFrMaRestApi.Models
{
    public class TimerClass
    {
        public DateTime timeToSend { get; set; }
        private static TimerClass instance;
        private Timer timer;
        private Mail mail = new Mail();
        public static TimerClass GetInstance()
        {
            if (TimerClass.instance == null)
                TimerClass.instance = new TimerClass();
            return TimerClass.instance;
        }
        private TimerClass()
        {
        }
        public void StartTimer()
        {
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
            List<EmailSettings> emailSettings = new List<EmailSettings>();
            using (MySqlConnection connection = WebApiConfig.Connection())
            using (MySqlCommand command = new MySqlCommand("SELECT * FROM `tbEmailPreferences`", connection))
            {
                connection.Open();
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        emailSettings.Add(new EmailSettings() { EmailAddress = (string)reader["RecievingEmail"], SendOnlyFailed = Convert.ToBoolean(reader["SendOnlyFailed"]) });
                    }
                }
            }
            this.mail.SendEmail(emailSettings);
        }
    }
}