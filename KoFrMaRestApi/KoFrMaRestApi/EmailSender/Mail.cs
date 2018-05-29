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
        string Ssubject = "KoFrMaBackup report (" + DateTime.Now.ToShortDateString() + ")";
        string Sbody = "";

        public void SendEmail(List<EmailSettings> emailSettings)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Path.Combine(AppContext.BaseDirectory, "EmailSender", "Email.html")))
            {
                Sbody = reader.ReadToEnd();
            }
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
            foreach (var item in emailSettings)
            {
                mail.To.Add(item.EmailAddress);
            }
            smt.Send(mail);
        }
    }
}