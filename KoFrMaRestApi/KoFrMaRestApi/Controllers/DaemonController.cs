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
        [Route("api/Daemon/GetInstructions/{DaemonId:int}")]
        [HttpGet]
        public Instructions GetInstructions(int PC_Unique, int OS,int Version, DateTime Time)
        {
            return new Instructions() { };
        }
    }
}