﻿using KoFrMaRestApi.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.AdminApp.GetList
{
    public class Data
    {
        public Data(List<int> tables)
        {
            Table table = new Table();
            foreach (int item in tables)
            {
                //Data 1 = tbAdminAccounts
                if (item == 1)
                    tbAdminAccounts = table.GetAdminAccounts();
                //Data 2 = tbDaemons
                if (item == 2)
                    tbDaemons = table.GetDaemons();
                //Data 3 = TbTasks
                if (item == 3)
                    tbTasks = table.GetTasks();
                //Data 4 = tbCompletedTasks
                if (item == 4)
                    tbTasksCompleted = table.GetTasksCompleted();
                //Data 5 = tbServerExceptions
                if (item == 5)
                    tbServerExceptions = table.GetServerExceptions();
            }
        }
        public List<tbAdminAccounts> tbAdminAccounts { get; set; }
        public List<tbDaemons> tbDaemons { get; set; }
        public List<tbTasks> tbTasks { get; set; }
        public List<tbServerExceptions> tbServerExceptions { get; set; }
        public List<tbTasksCompleted> tbTasksCompleted { get; set; }
    }
}