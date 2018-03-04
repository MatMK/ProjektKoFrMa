using KoFrMaRestApi.Models;
using KoFrMaRestApi.Models.AdminApp;
using KoFrMaRestApi.MySqlCom;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace KoFrMaRestApi.Controllers
{
    public class AdminAppController : ApiController
    {
        Token token = new Token();
        MySqlDaemon sqlDaemon = new MySqlDaemon();
        Settings settings = new Settings();
        [HttpPost, Route(@"api/AdminApp/RegisterToken")]
        public string RegisterToken(AdminLogin adminLogin)
        {
            return token.CreateToken(adminLogin);
        }
        [HttpGet, Route(@"api/AdminApp/test")]
        public void test()
        {
            using (MySqlConnection connection = WebApiConfig.Connection())
            {
                connection.Open();
                sqlDaemon.TaskCompletionRecieved(11,connection);
            }
            
        }
    }
}