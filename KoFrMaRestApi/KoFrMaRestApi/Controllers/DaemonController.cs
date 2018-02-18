using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using KoFrMaRestApi.Models;
using System.Data.SqlClient;

namespace KoFrMaRestApi.Controllers
{
    public class DaemonController : ApiController
    {
        [HttpPost]
        public Instructions GetInstructions(Instructions i)
        {
            //MySqlConnection connection = WebApiConfig.Connection();

            return i;
        }
    }
}