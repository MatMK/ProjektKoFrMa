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
        string Sbody = "";

        string SemailTo = "machpetr@sssvt.cz";

        public void SendEmail()
        {
            MailMessage mail = new MailMessage();
            mail.From=new MailAddress(SemailFrom);
            mail.To.Add(SemailTo);
            mail.Subject =Ssubject;
            mail.Body = "< div style='border: medium solid grey; width: 500px; height: 266px;font-family: arial,sans-serif; font-size: 17px;'>";
            Sbody += "<h3 style='background-color: blueviolet; margin-top:0px;'>KOFRMA backup agency</h3>";
            Sbody += "<br />";
            Sbody += "Dear " + SemailFrom + ",";
            Sbody += "<br />";
            Sbody += "<p>";
            Sbody += "Thank you for using our backup </p>";
            Sbody += " <br />";
            Sbody += "Thanks,";
            Sbody += "<br />";
            Sbody += "<b>The Team</b>";
            Sbody += "</div>";
            SmtpClient smt = new SmtpClient();
            mail.IsBodyHtml = true;
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