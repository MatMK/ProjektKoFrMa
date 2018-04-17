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
using KoFrMaRestApi.MySqlCom;
using KoFrMaRestApi.Models.Daemon.Task;

namespace KoFrMaRestApi.Controllers
{
    /// <summary>
    /// Slouží k komunikaci s Daemony a Serverem
    /// </summary>
    public class DaemonController : ApiController
    {
        Token token = new Token();
        MySqlDaemon mySqlCom = new MySqlDaemon();
        /// <summary>
        /// Vraci instrukce pro daemon a registruje daemony do databaze.
        /// </summary>
        /// <param name="daemon"></param>
        /// <returns>Obsahuje informace o deamonu zasílajicim informaci.</returns>
        [HttpPost, Route(@"api/Daemon/GetInstructions")]
        public List<Task> GetInstructions(Request request)
        {
            if (token.Authorized(request.daemon))
            {
                using (MySqlConnection connection = WebApiConfig.Connection())
                {
                    connection.Open();
                    //Zjistí zda je Daemon už zaregistrovaný, pokud ne, přidá ho do databáze
                    string DaemonId = mySqlCom.GetDaemonId(request.daemon, connection);
                    mySqlCom.DaemonSeen(DaemonId, connection);
                    // Vybere task určený pro daemona.
                    List<Task> tasks = mySqlCom.GetTasks(DaemonId, connection);
                    List<int> ToRemove = new List<int>();
                    List<int> BackupJournalNotNeeded = new List<int>();
                    for (int i = 0; i < tasks.Count - 1; i++)
                    {
                        foreach (var item in request.TasksVersions)
                        {
                            if ((tasks[i].IDTask == item.TaskID)&&(tasks[i].GetHashCode()==item.TaskDataHash))
                            {
                                ToRemove.Add(i);
                            }
                        }
                        foreach (var item in request.BackupJournalNotNeeded)
                        {
                            if (tasks[i].IDTask == item)
                            {
                                BackupJournalNotNeeded.Add(i);
                            }
                        }
                    }
                    for (int i = BackupJournalNotNeeded.Count - 1; i >= 0; i--)
                    {
                        tasks[BackupJournalNotNeeded[i]].Sources = new SourceJournalLoadFromCache() { JournalID = };
                    }
                    for (int i = ToRemove.Count - 1; i >= 0; i--)
                    {
                        tasks.RemoveAt(ToRemove[i]);
                    }
                    return tasks;
                }
            }
            else
            {
                return null;
            }
        }
        [HttpPost, Route(@"api/Daemon/TaskCompleted")]
        public void TaskCompleted(TaskComplete taskCompleted)
        {
            if (token.Authorized(taskCompleted.DaemonInfo))
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
                    mySqlCom.DaemonSeen(mySqlCom.GetDaemonId(taskCompleted.DaemonInfo, connection), connection);
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
                mySqlCom.DaemonSeen(mySqlCom.GetDaemonId(password.daemon, connection), connection);
                mySqlCom.RegisterDaemonAndGetId(password.daemon, password.password, connection);
                connection.Close();
            }
            return token.CreateToken(password.password, password.daemon);
        }
    }
}