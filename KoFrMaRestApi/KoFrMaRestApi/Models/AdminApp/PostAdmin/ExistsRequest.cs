using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.AdminApp.PostAdmin
{
    public class ExistsRequest : IRequest
    {
        public string TableName { get; set; }
        public string Value { get; set; }
        public string Column { get; set; }
    }
}