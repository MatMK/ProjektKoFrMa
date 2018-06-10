using KoFrMaRestApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.EmailSender
{
    public class ExecuteMail
    {
        private EmailSettings _email;
        private int _AdminId;
        private Mail mail = new Mail();
        public ExecuteMail(EmailSettings email, int AdminId)
        {
            this._email = email;
            this._AdminId = AdminId;
        }
        public void Execute()
        {
            mail.SendEmail(this._email, this._AdminId);
        }
    }
}