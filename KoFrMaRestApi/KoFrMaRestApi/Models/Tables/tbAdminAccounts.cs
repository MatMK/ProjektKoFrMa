﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.Tables
{
    public class tbAdminAccounts
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public Int64 Password { get; set; }
        public string Token { get; set; }
    }
}