using KoFrMaRestApi.Models.Daemon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using KoFrMaRestApi.Models.AdminApp;
using KoFrMaRestApi.MySqlCom;

namespace KoFrMaRestApi.Models
{
    public class Token
    {
        MySqlDaemon SqlDaemon = new MySqlDaemon();
        MySqlAdmin SqlAdmin = new MySqlAdmin();
        public bool Authorized(DaemonInfo daemon)
        {
            bool result;
            using (MySqlConnection connection = WebApiConfig.Connection())
            {
                connection.Open();
                result = SqlDaemon.Authorized(daemon.PC_Unique, daemon.Token, connection);
                connection.Close();
            }
            return result;
        }
        public string CreateToken(Int64 HashPassword, DaemonInfo daemon)
        {
            string Token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            SqlDaemon.RegisterToken(daemon.PC_Unique, HashPassword, Token);
            return Token;
        }
        public string CreateToken(AdminLogin login)
        {
            string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            SqlAdmin.RegisterToken(login.UserName, login.Password, token);
            return token;
        }
        public bool Authorized(PostAdmin postAdmin)
        {
            return SqlAdmin.Authorized(postAdmin.adminInfo.UserName, postAdmin.adminInfo.Token);
        }
    }
}