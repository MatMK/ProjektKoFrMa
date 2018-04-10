using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.AdminApp.PostAdmin
{
    public class GetDataRequest : IRequest
    {
        public List<int> getData { get; set; }
    }
}