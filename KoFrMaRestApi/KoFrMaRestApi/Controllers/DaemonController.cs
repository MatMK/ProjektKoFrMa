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
using System.Diagnostics;

namespace KoFrMaRestApi.Controllers
{
    /// <summary>
    /// Used for comunication with daemon
    /// </summary>
    public class DaemonController : ApiController
    {
        //Token token = new Token();
        MySqlDaemon mySqlCom = new MySqlDaemon();
        MySqlAdmin mySqlAdmin = new MySqlAdmin();
        /// <summary>
        /// Submits daemons completed tasks and returns new tasks if there are any.
        /// AUTHENTICATION:
        /// Use valid token retrievable from <see cref="Register(Password)"/>
        /// </summary>
        /// <param name="request">Contains </param>
        /// <returns>Contains new Tasks to execute</returns>
        [HttpPost, Route(@"api/Daemon/GetInstructions")]
        public List<Task> GetInstructions(Request request)
        {
            request.daemon.Token = request.daemon.Token.Replace("\"", "");
            using (MySqlConnection connection = WebApiConfig.Connection())
            {
                connection.Open();
                if (mySqlCom.Authorized(request.daemon.PC_Unique, request.daemon.Token, connection))
                {
                    TasksCompleted(request.CompletedTasks);
                    int DaemonId = (int)mySqlCom.GetDaemonId(request.daemon);
                    mySqlCom.DaemonSeen(DaemonId, connection);
                    List<Task> tasks = mySqlCom.GetTasks(DaemonId, connection);
                    List<int> ToRemove = new List<int>();
                    List<int> BackupJournalNotNeeded = new List<int>();
                    for (int i = 0; i < tasks.Count - 1; i++)
                    {
                        foreach (var item in request.TasksVersions)
                        {
                            if ((tasks[i].IDTask == item.TaskID) && (tasks[i].GetHashCode() == item.TaskDataHash))
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
                        tasks[BackupJournalNotNeeded[i]].Sources = new SourceJournalLoadFromCache() { JournalID = BackupJournalNotNeeded[i] };
                    }
                    for (int i = ToRemove.Count - 1; i >= 0; i--)
                    {
                        tasks.RemoveAt(ToRemove[i]);
                    }
                    return tasks;
                }
                else
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// Registers completed tasks in SQL database. Creates new task if completed task should repeat
        /// AUTHENTICATION:
        /// Use valid token retrievable from <see cref="Register(Password)"/>
        /// </summary>
        /// <param name="tasksCompleted">List of completed tasks</param>
        private void TasksCompleted(List<TaskComplete> tasksCompleted)
        {
            using (MySqlConnection connection = WebApiConfig.Connection())
            {
                connection.Open();
                foreach (TaskComplete taskCompleted in tasksCompleted)
                {
                    if (taskCompleted.IsSuccessfull)
                    {
                        mySqlCom.TaskCompletionRecieved(taskCompleted, connection);
                    }
                    else
                    {
                        mySqlCom.TaskRemove(taskCompleted);
                    }
                }
                connection.Close();
            }
        }
        /// <summary>
        /// Returns token useable only in <see cref="DaemonController"/>
        /// </summary>
        /// <param name="password">Input with daemon's <see cref="DaemonInfo"/> and password in base64</param>
        /// <returns>Returns token</returns>
        [HttpPost, Route(@"api/Daemon/RegisterToken")]
        public RegisterData Register(Password password)
        {
            string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            Bcrypter encrypt = new Bcrypter();
            using (MySqlConnection connection = WebApiConfig.Connection())
            {
                connection.Open();
                mySqlCom.RegisterDaemonAndGetId(password.daemon, password.password);
                mySqlCom.DaemonSeen((int)mySqlCom.GetDaemonId(password.daemon ), connection);
                connection.Close();
            }
            mySqlCom.RegisterToken(password.daemon.PC_Unique,password.password,token);
            return new RegisterData() { Token = token, TimerTick = mySqlAdmin.GetTimerTick((int)mySqlCom.GetDaemonId(password.daemon))};
        }
    }
}