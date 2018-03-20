using KoFrMaRestApi.Models;
using KoFrMaRestApi.Models.AdminApp;
using KoFrMaRestApi.Models.AdminApp.GetList;
using KoFrMaRestApi.Models.Tables;
using KoFrMaRestApi.MySqlCom;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

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
        [HttpPost, Route(@"api/AdminApp/SetTask")]
        public bool SetTask(PostAdmin postAdmin)
        {
            if (token.Authorized(postAdmin))
            {
                try
                {
                    mySqlCom.SetTasks(postAdmin.setTasks);
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
        [HttpPost, Route(@"api/AdminApp/GetList")]
        public Data GetList(PostAdmin postAdmin)
        {
            if (token.Authorized(postAdmin))
            {
                return new Data();
            }
            else
            {
                return null;
            }
        }
        [HttpPost, Route(@"api/AdminApp/AlterData")]
        public void AlterData(ChangeTable changeTable)
        {
            mySqlCom.AlterTable(changeTable);
        }
        [HttpGet, Route(@"api/AdminApp/test")]
        public string test()
        {
            return "cau ";
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