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
        [HttpPost, Route(@"api/AdminApp/AlterData")]
        public void AlterData(PostAdmin postAdmin)
        {
            if(this.Authorized(postAdmin.adminInfo))
            {
                mySqlCom.AlterTable(((ChangeTableRequest)postAdmin.request).changeTable);
            }
        }
        [HttpPost, Route("api/AdminApp/AlterPermission")]
        public void AlterPermission(PostAdmin postAdmin)
        {
            if(this.Authorized(postAdmin.adminInfo))
            {
                //unfinished
            }
        }
        [HttpPost, Route(@"api/AdminApp/AddAdmin")]
        public void AddAdmin(PostAdmin postAdmin)
        {
            if (this.Authorized(postAdmin.adminInfo))
            {
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
            if(this.Authorized(postAdmin.adminInfo))
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
        [HttpGet, Route(@"api /AdminApp/test")]
        public int test()
        {
            return mySqlCom.NextAutoIncrement("tbTjjjjasks");
            /*
            mySqlCom.AlterTable(new ChangeTable()
            {
                ColumnName = "IdDaemon",
                Id = 2,
                TableName = "tbTasks",
                Value = 6
            });*/


            /*
            List<SetTasks> list = new List<SetTasks>();
            list.Add(new SetTasks()
            {
                ExecutionTimes = null,
                DaemonId = 1,
                TimeToBackup = new DateTime(2018, 3, 18, 18, 0, 0),
                SourceOfBackup = "this is a source of backup",
                WhereToBackup = "this is where to backup",
                TimerValue = 10,
                LogLevel = 0,
                CompressionLevel = 2,
                NetworkCredentials = null,
                InProgress = false
            }
            );
            mySqlCom.SetTasks(list);*/
        }
    }
}