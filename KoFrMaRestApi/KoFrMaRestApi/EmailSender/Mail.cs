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
using System.IO;

namespace KoFrMaRestApi.EmailSender
{
    public class Mail
    {
        string smtpAddress = "smtp.gmail.com";
        int portNumber = 587;
        string SemailFrom = "kofrmabackup@gmail.com";
        string SemailFromName = "KoFrMa Report Daemon";
        string Spassword = "KoFrMa123456";
        string Sbody = "";

        public void SendEmail(EmailSettings emailSettings, int AdminId)
        {
            string Ssubject = "KoFrMaBackup report (" + DateTime.Now.ToShortDateString() + ")";
            using (StreamReader reader = new StreamReader(Path.Combine(AppContext.BaseDirectory, "EmailSender", "Email.html")))
            {
                Sbody = reader.ReadToEnd();
            }
            List<string> exceptions = new List<string>();
            List<TaskComplete> completedTasks = new List<TaskComplete>();
            using (MySqlConnection connection = WebApiConfig.Connection())
            using (MySqlCommand command = new MySqlCommand("SELECT * FROM `tbRestApiExceptions` inner join tbRestApiExceptionsAdminNOTNotified on tbRestApiExceptionsAdminNOTNotified.IdRestApiExceptions = tbRestApiExceptions.Id where `IdAdmin` = " + AdminId, connection))
            {
                connection.Open();
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        exceptions.Add((string)reader["ExceptionInJson"]);
                    }
                }
                command.CommandText = "SELECT * FROM tbTasksCompleted inner join tbTasksCompletedAdminNOTNotified on tbTasksCompleted.Id = tbTasksCompletedAdminNOTNotified.IdTaskCompleted where tbTasksCompletedAdminNOTNotified.IdAdmin = " + AdminId;
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        completedTasks.Add(new TaskComplete()
                        {
                            //DatFile = JsonSerializationUtility.Deserialize<BackupJournalObject>((string)reader["BackupJournal"]),
                            IDTask = (int)reader["Id"],
                            DaemonInfo = new DaemonInfo() { Id = (int)reader["Id"] },
                            TimeOfCompletition = (DateTime)reader["TimeOfCompletition"],
                            DebugLog = new List<string>() { (string)reader["DebugLog"] },
                            IsSuccessfull = Convert.ToBoolean(reader["IsSuccessfull"])
                        });
                    }
                }
                command.CommandText = "DELETE FROM `tbTasksCompletedAdminNOTNotified` WHERE IdAdmin = " + AdminId;
                command.ExecuteNonQuery();
                command.CommandText = "DELETE FROM `tbRestApiExceptionsAdminNOTNotified` WHERE `IdAdmin` = " + AdminId;
                command.ExecuteNonQuery();
            }
            int successful = 0;
            foreach (var item in completedTasks)
            {
                if (item.IsSuccessfull)
                {
                    successful++;
                }
            }
            Sbody = Sbody.Replace("{Date}", DateTime.Now.ToShortDateString());
            Sbody = Sbody.Replace("{AdminAppUrl}", WebApiConfig.WebServerURL);
            Sbody = Sbody.Replace("{ErrorCount}", exceptions.Count.ToString());
            Sbody = Sbody.Replace("{CompletedTasks}", completedTasks.Count.ToString());
            Sbody = Sbody.Replace("{Successful}", successful.ToString());
            string listCompletedTasks = "";
            foreach (var item in completedTasks)
            {
                listCompletedTasks += "<li style = \"margin:0 0 10px 30px;\" class=\"list-item-first\"><a href=" + WebApiConfig.WebServerURL + "/backup/completed-task-info/" + item.IDTask+">";
                listCompletedTasks += $"Task {item.IDTask} was ";
                listCompletedTasks += item.IsSuccessfull ? "successful" : "unsuccessful";
                listCompletedTasks += $". It was completed {item.TimeOfCompletition.ToShortDateString()} at {item.TimeOfCompletition.ToShortTimeString()}";
                listCompletedTasks += "</a></li>";

            }
            string listExceptions = "";
            foreach (var item in exceptions)
            {
                listExceptions += "<li style = \"margin:0 0 10px 30px;\" class=\"list-item-first\">";
                listExceptions += $"{item}";
                listExceptions += "</li>";
            }
            Sbody = Sbody.Replace("{ListOfTasks}", listCompletedTasks);
            Sbody = Sbody.Replace("{ListOfExceptions}", listExceptions);
            Sbody = Sbody.Replace("{TextAfterTasks}", "");
            Sbody = Sbody.Replace("{TextAfterExceptions}", "");
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(SemailFrom,SemailFromName,Encoding.UTF8);
            mail.Subject = Ssubject;
            mail.Body = Sbody;
            SmtpClient smt = new SmtpClient();
            mail.IsBodyHtml = true;
            smt.Host = smtpAddress;
            smt.Port = portNumber;
            smt.EnableSsl = true;
            NetworkCredential nc = new NetworkCredential(SemailFrom, Spassword);
            //smtp.UseDefaultCredentials = true;
            mail.Body = Sbody;
            smt.Credentials = nc;
            mail.To.Add(emailSettings.EmailAddress);
            smt.Send(mail);
        }
    }
}