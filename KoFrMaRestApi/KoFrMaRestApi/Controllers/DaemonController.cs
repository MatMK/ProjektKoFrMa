using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using KoFrMaRestApi.Models;
using System.Data.SqlClient;
using KoFrMaRestApi.Models.Daemon;

namespace KoFrMaRestApi.Controllers
{
    /// <summary>
    /// Slouží k komunikaci s Daemony a Serverem
    /// </summary>
    public class DaemonController : ApiController
    {
        Token token = new Token();
        MySqlCom mySqlCom = new MySqlCom();
        /// <summary>
        /// Vrací instrukce pro daemon a registruje daemony do databáze.
        /// </summary>
        /// <param name="daemon"></param>
        /// <returns>Obsahuje informace o deamonu zasílajícím informaci.</returns>
        [HttpPost, Route(@"api/Daemon/GetInstructions")]
        public List<Tasks> GetInstructions(DaemonInfo daemon)
        {
            if (token.Authorized(daemon))
            {
                using (MySqlConnection connection = WebApiConfig.Connection())
                {
                    connection.Open();
                    //Zjistí zda je Daemon už zaregistrovaný, pokud ne, přidá ho do databáze
                    string DaemonId = mySqlCom.GetDaemonId(daemon, connection);

                    mySqlCom.DaemonSeen(DaemonId, connection);
                    // Vybere task určený pro daemona.
                    return mySqlCom.GetTasks(DaemonId, connection);
                }
            }
            else
            {
                return null;
            }
        }
        [HttpPost,Route(@"api/Daemon/TaskCompleted")]
        public void TaskCompleted(TaskComplete taskCompleted)
        {
            if (token.Authorized(taskCompleted.daemonInfo))
            {
                using (MySqlConnection connection = WebApiConfig.Connection())
                {
                    connection.Open();
                    if (taskCompleted.IsSuccessfull)
                    {
                        mySqlCom.TaskCompletionRecieved(taskCompleted, connection);
                        //pridat k povedenym taskum a odeslat emailem
                    }
                    else
                    {
                        //pridat k nepovedenym taskum a odeslat to emailem
                    }
                    mySqlCom.DaemonSeen(mySqlCom.GetDaemonId(taskCompleted.daemonInfo, connection), connection);
                    connection.Close();
                }
            }
        }
        [HttpPost, Route(@"api/Daemon/RegisterToken")]
        public string Register(Password password)
        {
            using (MySqlConnection connection = WebApiConfig.Connection())
            {
                connection.Open();
                mySqlCom.RegisterDaemonAndGetId(password.daemon,password.password, connection);
                connection.Close();
            }
            return token.CreateToken(password.password);
        }
    }
}