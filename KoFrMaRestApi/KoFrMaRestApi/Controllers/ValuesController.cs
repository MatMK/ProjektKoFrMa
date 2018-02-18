using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MySql.Data.MySqlClient;

namespace KoFrMaRestApi.Controllers
{
    public class results
    {
        public string name { get; set; }
        public string password { get; set; }

        public results(string name, string password)
        {
            this.name = name;
            this.password = password;
        }
    }

    public class ValuesController : ApiController
    {

        //https://www.youtube.com/watch?v=TcovfE8IsHs
        //


        // Get api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
         // Get api/values/5
        public string Get(int id)
        {
            MySqlConnection conn = WebApiConfig.Connection();
            MySqlCommand query = conn.CreateCommand();

            query.CommandText = "SELECT id, PC_Unique from tbDaemons";
            var results = new List<results>();


            try
            {
                conn.Open();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
            return "failure: " + ex;
            }

            MySqlDataReader fetch_query = query.ExecuteReader();
            while (fetch_query.Read())
            {
                return results.Add(new results(fetch_query["id"].ToString(), fetch_query["PC_Unique"].ToString()));
            }
            return "Done";
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