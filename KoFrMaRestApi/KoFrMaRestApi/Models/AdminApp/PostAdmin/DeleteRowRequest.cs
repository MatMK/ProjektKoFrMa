using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.AdminApp.PostAdmin
{
    public class DeleteRowRequest : IRequest
    {
        public string TableName { get; set; }
        public int Id { get; set; }
    }
}