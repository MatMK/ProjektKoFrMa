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

namespace KoFrMaRestApi.Controllers
{
    public class DaemonController : ApiController
    {
        [Route("api/Daemon/Register/{daemon}")]
        [HttpPost]
        public void DaemonRegister(DaemonInfo daemon)
        {

        }
        [Route("api/Daemon/GetInstructions/{DaemonId:int}")]
        [HttpGet]
        public Instructions GetInstructions(int DaemonId)
        {
            throw new Exception();
        }
    }
}