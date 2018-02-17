using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MySql.Data.MySqlClient;

namespace KoFrMaRestApi.Controllers
{
    public class ValuesController : ApiController
    {
        //https://www.youtube.com/watch?v=TcovfE8IsHs


        // Get api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
         // Get api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // Delete api/values/5
        public void Delete(int id)
        {

        }

        // Post api/values
        public void Post([FromBody]string value)
        {
        }

        // Put api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

     
    }
}