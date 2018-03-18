using KoFrMaRestApi.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.AdminApp.GetList
{
    public class Data
    {
        public Data()
        {
            tbAdminAccounts = Table.GetAdminAccounts();
            tbDaemons = Table.GetDaemons();
            tbTasks = Table.GetTasks();
        }
        public List<tbAdminAccounts> tbAdminAccounts { get; set; }
        public List<tbDaemons> tbDaemons { get; set; }
        public List<tbTasks> tbTasks { get; set; }
    }
}