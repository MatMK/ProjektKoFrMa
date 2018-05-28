using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Text;
using System.Net;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using KoFrMaRestApi.Models.Daemon;
using KoFrMaRestApi.Models;
using KoFrMaRestApi.Models.Daemon.Task.BackupJournal;

namespace KoFrMaRestApi.EmailSender
{
    public class Mail
    {
        string smtpAddress = "smtp.gmail.com";
        int portNumber = 587;
        string SemailFrom = "kofrmabackup@gmail.com";
        string SemailFromName = "KoFrMa Report Daemon";
        string Spassword = "KoFrMa123456";
        string Ssubject = "KoFrMa report";
        string Sbody = "";

        public void SendEmail(List<EmailSettings> emailSettings)
        {
            List<Exception> exceptions = new List<Exception>();
            List<TaskComplete> completedTasks = new List<TaskComplete>();
            using (MySqlConnection connection = WebApiConfig.Connection())
            using (MySqlCommand command = new MySqlCommand("select * from tbRestApiExceptions where AdminNotified = 0", connection))
            {
                connection.Open();
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        exceptions.Add(JsonConvert.DeserializeObject<Exception>((string)reader["ExceptionInJson"]));
                    }
                }
                command.CommandText = "SELECT * FROM `tbTasksCompleted` WHERE `AdminNotified` = 0";
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        completedTasks.Add(new TaskComplete()
                        {
                            DatFile = JsonSerializationUtility.Deserialize<BackupJournalObject>((string)reader["BackupJournal"]),
                            IDTask = (int)reader["Id"],
                            DaemonInfo = new DaemonInfo() { Id = (int)reader["Id"] },
                            TimeOfCompletition = (DateTime)reader["TimeOfCompletition"],
                            DebugLog = new List<string>() { (string)reader["DebugLog"] },
                            IsSuccessfull = Convert.ToBoolean(reader["IsSuccessfull"])
                        });
                    }
                }
                command.CommandText = "UPDATE `tbRestApiExceptions` SET `AdminNotified`=1 WHERE `AdminNotified`= 0";
                command.ExecuteNonQuery();
            }
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(SemailFrom,SemailFromName,Encoding.UTF8);
            mail.Subject = Ssubject;
            //Sbody += "< div style='border: medium solid grey; width: 500px; height: 266px;font-family: arial,sans-serif; font-size: 17px;'>";
            Sbody += "<h3 style='background-color: blueviolet; margin-top:0px;'>KOFRMA backup agency</h3>";
            Sbody += "<br/>";
            Sbody += "Dear user,";
            Sbody += "<br/>";
            if (exceptions.Count > 0)
            {
                Sbody += $"Since last time, there has been {exceptions.Count} errors on KoFrMaRestApi server, here are the messages:";
                Sbody += "<br/>";
                foreach (var item in exceptions)
                {
                    Sbody += item.Message;
                    Sbody += "<br/>";
                }
            }
            if (completedTasks.Count > 0)
            {
                Sbody += $"Since last time, {completedTasks.Count} tasks were completed, here is more info:";
                Sbody += "<br/>";
                foreach (var item in completedTasks)
                {
                    Sbody += item.IDTask;
                    Sbody += $"<a href=\"{WebApiConfig.WebServerURL}\">More info</a>";
                    Sbody += "<br/>";
                }
            }
            Sbody += "<p>";
            Sbody += "Thank you for using our backup</p>";
            Sbody += "<br/>";
            Sbody += "Thanks,";
            Sbody += "<br/>";
            Sbody += "<b>The KoFrMa Team</b>";
            Sbody += "</div>";
            SmtpClient smt = new SmtpClient();
            mail.IsBodyHtml = true;
            smt.Host = smtpAddress;
            smt.Port = portNumber;
            smt.EnableSsl = true;
            NetworkCredential nc = new NetworkCredential(SemailFrom, Spassword);
            //smtp.UseDefaultCredentials = true;
            mail.Body = Sbody;
            smt.Credentials = nc;
            foreach (var item in emailSettings)
            {
                mail.To.Add(item.EmailAddress);
            }
            smt.Send(mail);
        }
    }
}