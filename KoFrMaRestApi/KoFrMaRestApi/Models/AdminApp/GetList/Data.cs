using KoFrMaRestApi.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.AdminApp.GetList
{
    public class Data
    {
        public Data(List<int> getData)
        {
            foreach (int item in getData)
            {

                //Data 1
                if (item == 1)
                    tbAdminAccounts = Table.GetAdminAccounts();
                //Data 2
                if (item == 2)
                    tbDaemons = Table.GetDaemons();
                //Data 3
                if (item == 3)
                    tbTasks = Table.GetTasks();
            }
        }
        public List<tbAdminAccounts> tbAdminAccounts { get; set; }
        public List<tbDaemons> tbDaemons { get; set; }
        public List<tbTasks> tbTasks { get; set; }
    }
}