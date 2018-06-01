using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace KoFrMaRestApi.Models.Settings
{
    public class RestApiUserSetting
    {
        public string DatabaseAddress { get; set; }
        public NetworkCredential DatabaseCredential { get; set; }
    }
}