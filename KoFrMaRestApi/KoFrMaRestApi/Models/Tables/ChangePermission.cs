using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.Tables
{
    public class ChangePermission
    {
        public int AdminId { get; set; }
        public int[] Permissions { get; set; }
    }
}