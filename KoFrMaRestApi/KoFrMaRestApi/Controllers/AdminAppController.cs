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
                try
                {
                    mySqlCom.SetTasks(((SetTasksRequest)postAdmin.request).setTasks);
                }
                catch
                {
                    return false;
                }
                return true;
            }
            else
            {
                return false;
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
                return null;
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
                    return "Insuficient permissions";
            }
            return "Unauthorized";
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
                    return "Insuficient permissions";
            }
            return "Unauthorized";
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
                    return "Insuficient permissions";
            }
            return "Unauthorized";
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
                    return "Insuficient permissions";
            }
            return "Unauthorized";
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
                    return "Insuficient permissions";
            }
            return "Unauthorized";
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
                    return "Insuficient permissions";
            }
            return "Unauthorized";
        }
        [HttpPost, Route(@"api/AdminApp/AddAdmin")]
        public void AddAdmin(PostAdmin postAdmin)
        {
            if (this.Authorized(postAdmin.adminInfo))
            {
                if(Permitted(postAdmin.adminInfo.UserName,new int[] {1}))
                    if (postAdmin.request is AddAdminRequest)
                        mySqlCom.AddAdmin(((AddAdminRequest)postAdmin.request).addAdmin);
            }
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
        public void DeleteRow(PostAdmin postAdmin)
        {
            if(this.Authorized(postAdmin.adminInfo) && (((DeleteRowRequest)postAdmin.request).TableName == "tbAdminAccounts"|| ((DeleteRowRequest)postAdmin.request).TableName == "tbDaemons" || ((DeleteRowRequest)postAdmin.request).TableName == "tbTasks"))
            {
                this.mySqlCom.DeleteRow((DeleteRowRequest)postAdmin.request);
            }
        }
        [HttpPost, Route(@"api/AdminApp/Exists")]
        public bool? Exists(PostAdmin postAdmin)
        {
            if (this.Authorized(postAdmin.adminInfo))
            {
                return this.mySqlCom.Exists((ExistsRequest)postAdmin.request);
            }
            return null;
        }
        [HttpGet, Route(@"api/AdminApp/test")]
        public Task test()
        {
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
                TemporaryFolderMaxBuffer = null
            };
        }
    }
}