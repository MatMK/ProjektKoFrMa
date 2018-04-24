using KoFrMaRestApi.Models;
using KoFrMaRestApi.Models.AdminApp;
using KoFrMaRestApi.Models.AdminApp.GetList;
using KoFrMaRestApi.Models.AdminApp.PostAdmin;
using KoFrMaRestApi.Models.Tables;
using KoFrMaRestApi.MySqlCom;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using Newtonsoft.Json;
using KoFrMaRestApi.Models.Daemon.Task;
using System.Net;
using BCrypt.Net;

namespace KoFrMaRestApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*", exposedHeaders: "X-My-Header")]
    public class AdminAppController : ApiController
    {
        Token token = new Token();
        MySqlAdmin mySqlCom = new MySqlAdmin();
        [HttpPost, Route(@"api/AdminApp/RegisterToken")]
        public string RegisterToken(AdminLogin adminLogin)
        {
            return mySqlCom.RegisterToken(adminLogin);
        }
        [HttpPost, Route(@"api/AdminApp/Permitted")]
        public bool Permitted(AdminInfo adminInfo)
        {
            int? AdminId = mySqlCom.GetAdminId(adminInfo.UserName);
            if (AdminId == null)
            {
                return false;
            }
            return mySqlCom.HasPermission((int)AdminId,adminInfo.Permission);
        }
        private bool Permitted(string Username, int[] Permission)
        {
            int? AdminId = mySqlCom.GetAdminId(Username);
            if (AdminId == null)
            {
                return false;
            }
            return mySqlCom.HasPermission((int)AdminId, Permission);
        }
        [HttpPost, Route(@"api/AdminApp/Authorized")]
        public bool Authorized(AdminInfo adminInfo)
        {
            return mySqlCom.Authorized(adminInfo.UserName, adminInfo.Token);
        }
        [HttpPost, Route(@"api/AdminApp/SetTask")]
        public bool SetTask(PostAdmin postAdmin)
        {
            if (token.Authorized(postAdmin))
            {
                mySqlCom.SetTasks(((SetTasksRequest)postAdmin.request).setTasks);
                return true;
            }
            else
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }
        }
        [HttpPost, Route(@"api/AdminApp/GetSqlData")]
        public Data GetSqlData(PostAdmin postAdmin)
        {
            if (token.Authorized(postAdmin))
            {
                return new Data(((GetDataRequest)postAdmin.request).getData);
            }
            else
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }
        }
        [HttpPost, Route(@"api/AdminApp/AlterDataUsername")]
        public string AlterDataUsername(PostAdmin postAdmin)
        {
            if (this.Authorized(postAdmin.adminInfo))
            {
                if (Permitted(postAdmin.adminInfo.UserName, new int[] { 3 }))
                {
                    mySqlCom.AlterTable(((ChangeTableRequest)postAdmin.request).changeTable, "tbAdminAccounts", "Username");
                    return null;
                }
                else
                    throw new HttpResponseException(HttpStatusCode.Forbidden);
            }
            else
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }
        [HttpPost, Route(@"api/AdminApp/AlterDataEmail")]
        public string AlterDataEmail(PostAdmin postAdmin)
        {
            if (this.Authorized(postAdmin.adminInfo))
            {
                if (Permitted(postAdmin.adminInfo.UserName, new int[] { 3 }))
                {
                    mySqlCom.AlterTable(((ChangeTableRequest)postAdmin.request).changeTable, "tbAdminAccounts", "Email");
                    return null;
                }
                else
                    throw new HttpResponseException(HttpStatusCode.Forbidden);
            }
            else
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }
        [HttpPost, Route(@"api/AdminApp/AlterDataEnabled")]
        public string AlterDataEnabled(PostAdmin postAdmin)
        {
            if (this.Authorized(postAdmin.adminInfo))
            {
                if (Permitted(postAdmin.adminInfo.UserName, new int[] { 3 }))
                {
                    mySqlCom.AlterTable(((ChangeTableRequest)postAdmin.request).changeTable, "tbAdminAccounts", "Enabled");
                    return null;
                }
                else
                    throw new HttpResponseException(HttpStatusCode.Forbidden);
            }
            else
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }
        [HttpPost, Route(@"api/AdminApp/AlterDataPermissions")]
        public string AlterDataPermissions(PostAdmin postAdmin)
        {
            if (this.Authorized(postAdmin.adminInfo))
            {
                if (Permitted(postAdmin.adminInfo.UserName, new int[] { 3, 4 }))
                {
                    mySqlCom.AlterPermissions(((ChangePermissionRequest)postAdmin.request).changePermission);
                    return null;
                }
                else
                    throw new HttpResponseException(HttpStatusCode.Forbidden);
            }
            else
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }
        [HttpPost, Route(@"api/AdminApp/AlterDataIdDaemon")]
        public string AlterDataIdDaemon(PostAdmin postAdmin)
        {
            if (this.Authorized(postAdmin.adminInfo))
            {
                if (Permitted(postAdmin.adminInfo.UserName, new int[] { 3 }))
                {
                    mySqlCom.AlterTable(((ChangeTableRequest)postAdmin.request).changeTable, "tbTasks", "IdDaemon");
                    return null;
                }
                else
                    throw new HttpResponseException(HttpStatusCode.Forbidden);
            }
            else
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }
        [HttpPost, Route(@"api/AdminApp/AlterDataAllowed")]
        public string AlterDataAllowed(PostAdmin postAdmin)
        {
            if (this.Authorized(postAdmin.adminInfo))
            {
                if (Permitted(postAdmin.adminInfo.UserName, new int[] { 3 }))
                {
                    mySqlCom.AlterTable(((ChangeTableRequest)postAdmin.request).changeTable, "tbDaemons", "Allowed");
                    return null;
                }
                else
                    throw new HttpResponseException(HttpStatusCode.Forbidden);
            }
            else
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }
        [HttpPost, Route(@"api/AdminApp/AddAdmin")]
        public void AddAdmin(PostAdmin postAdmin)
        {
            if (this.Authorized(postAdmin.adminInfo))
            {
                if (Permitted(postAdmin.adminInfo.UserName, new int[] { 1 }))
                {
                    if (postAdmin.request is AddAdminRequest)
                        mySqlCom.AddAdmin(((AddAdminRequest)postAdmin.request).addAdmin);
                }
                else
                    throw new HttpResponseException(HttpStatusCode.Forbidden);
            }
            else
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }
        [HttpPost, Route(@"api/AdminApp/LogOut")]
        public void LogOut(AdminInfo admin)
        {
            if (this.Authorized(admin))
            {
                int? id = mySqlCom.GetAdminId(admin.UserName);
                if (id != null)
                {
                    mySqlCom.LogOut((int)id);
                }
                else
                {
                    throw new Exception("No admin with such name");
                }
            }
        }

        [HttpPost, Route(@"api/AdminApp/DeleteRow")]
        public string DeleteRow(PostAdmin postAdmin)
        {
            if(this.Authorized(postAdmin.adminInfo) && (((DeleteRowRequest)postAdmin.request).TableName == "tbAdminAccounts"|| ((DeleteRowRequest)postAdmin.request).TableName == "tbDaemons" || ((DeleteRowRequest)postAdmin.request).TableName == "tbTasks"))
            {
                if (Permitted(postAdmin.adminInfo.UserName, new int[] { 3 }))
                {
                    this.mySqlCom.DeleteRow((DeleteRowRequest)postAdmin.request);
                    return null;
                }
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }
            else
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }
        [HttpPost, Route(@"api/AdminApp/Exists")]
        public bool Exists(PostAdmin postAdmin)
        {
            if (this.Authorized(postAdmin.adminInfo))
            {
                return this.mySqlCom.Exists((ExistsRequest)postAdmin.request);
            }
            else
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }
        [HttpGet, Route(@"api/AdminApp/test")]
        public void test()
        {

            /*
            MySqlDaemon d = new MySqlDaemon();
            MySqlConnection connection = WebApiConfig.Connection();
            connection.Open();
            d.TaskCompletionRecieved(new Models.Daemon.TaskComplete() {
                DaemonInfo = new Models.Daemon.DaemonInfo()
                {
                    OS = "",
                    Version = 0,
                    PC_Unique = "160984079301212",
                    Token = ""
                },
                IDTask = 2,
                TimeOfCompletition = DateTime.Now,
                DatFile = new Models.Daemon.Task.BackupJournal.BackupJournalObject()
                {
                    BackupJournalFiles = null,
                    BackupJournalFolders = null,
                    RelativePath = ""
                },
                DebugLog = new List<string>(),
                IsSuccessfull = true
            }, connection);
            /*
            return new Task()
            {
                IDTask = 2,
                TimeToBackup = DateTime.Now,
                Sources = new SourceFolders()
                {
                    Paths = new List<string>()
                    {
                        "C:\\Data",
                        "C:\\Data2"
                    }
                },
                Destinations = new List<IDestination>() {
                    new DestinationPlain()
                    {
                        Path = new DestinationPathLocal()
                        {
                            Path = "C:\\Data_backup"
                        }
                    }
                },
                LogLevel = 7,
                InProgress = false,
                ScriptBefore = null,
                ScriptAfter = null,
                TemporaryFolderMaxBuffer = null*/
        }
    }
}
