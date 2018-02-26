using KoFrMaRestApi.Models.Daemon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace KoFrMaRestApi.Models
{
    public class Token
    {
        MySqlCom mySql = new MySqlCom();
        public bool Authorized(DaemonInfo daemon)
        {
            bool result;
            using (MySqlConnection connection = WebApiConfig.Connection())
            {
                connection.Open();

                result =  mySql.Authorized(daemon.PC_Unique, daemon.Token, connection);
                connection.Close();
            }
            return result;
        }
        public string CreateToken(int HashPassword)
        {
            string Token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            mySql.RegisterToken(HashPassword, Token);
            return Token;
        }
    }
}