using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Text;
using System.Net;

namespace KoFrMaRestApi.EmailSender
{
    public class EmailSender
    {
        string smtpAddress = "smtp.gmail.com";
        int portNumber = 587;
        string SemailFrom = "kofrmabackup@gmail.com";
        string Spassword = "KoFrMa123456";
        string Ssubject = "Test";
        string Sbody = "Testing email";
        string SemailTo = "machpetr@sssvt.cz";

        public void SendEmail()
        {
            MailMessage mail = new MailMessage();
            mail.From=new MailAddress(SemailFrom);
            mail.To.Add(SemailTo);
            mail.Subject =Ssubject;
            mail.Body =Sbody;
            SmtpClient smt = new SmtpClient();
            mail.IsBodyHtml = false;
            smt.Host = smtpAddress;
            smt.Port = portNumber;
            smt.EnableSsl = true;
            NetworkCredential nc = new NetworkCredential(SemailFrom,Spassword);
            //smtp.UseDefaultCredentials = true;
            smt.Credentials = nc;
            smt.Send(mail);
        }
    }
}