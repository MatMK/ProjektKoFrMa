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
        //string smtpAddress = "smtp.gmail.com";
        //int portNumber = 587;
        //bool enableSSL = true;
        //string emailFrom = "companyemail";
        //string password = "password";
        //string emailTo = "Your email";
        //string subject = "Hello!";
        //string body = "Hello, Mr.";
        //MailMessage mail = new MailMessage();
        //mail.From = new MailAddress(emailFrom);
        //mail.To.Add(emailTo);
        //mail.Subject = subject;
        //mail.Body = body;
        //mail.IsBodyHtml = true;
        //using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
        //{
        //smtp.Credentials = new NetworkCredential(emailFrom, password);
        //smtp.EnableSsl = enableSSL;
        //smtp.Send(mail);
        //}

        //MailMessage mm = new MailMessage(txtEmail.Text, txtTo.Text);
        //mm.Subject = txtSubject.Text;
        //mm.Body = txtBody.Text;
        //if (fuAttachment.HasFile)//file upload select or not
        //{
        //string FileName = Path.GetFileName(fuAttachment.PostedFile.FileName);
        //mm.Attachments.Add(new Attachment(fuAttachment.PostedFile.InputStream, FileName));
        //}
        //mm.IsBodyHtml = false;
        //SmtpClient smtp = new SmtpClient();
        //smtp.Host = "smtp.gmail.com";
        //smtp.EnableSsl = true;
        //NetworkCredential NetworkCred = new NetworkCredential(txtEmail.Text, txtPassword.Text);
        //smtp.UseDefaultCredentials = true;
        //smtp.Credentials = NetworkCred;
        //smtp.Port = 587;
        //smtp.Send(mm);

        public void SendEmail()
        {
            MailMessage mail = new MailMessage();
            mail.From=new MailAddress(SemailFrom);
            mail.To.Add(SemailTo);
            mail.Subject =Ssubject;
            mail.Body =Spassword;
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