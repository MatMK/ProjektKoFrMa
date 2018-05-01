using KoFrMaRestApi.EmailSender;
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
        public DateTime timeToSend;
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
            if(DateTime.Now == timeToSend)
            {
                this.mail.SendEmail();
            }
        }
    }
}