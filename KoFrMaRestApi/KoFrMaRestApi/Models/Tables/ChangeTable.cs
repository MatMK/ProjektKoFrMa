using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.Tables
{
    public class ChangeTable
    {
        //public string TableName { get; set; }
        public int Id { get; set; }
        //public string ColumnName{ get; set; }
        public dynamic Value { get; set; }
    }
}