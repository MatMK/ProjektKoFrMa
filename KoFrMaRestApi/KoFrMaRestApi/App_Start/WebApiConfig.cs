using System;
using System.Collections.Generic;

using System.Linq;
using System.Net.Http.Headers;
using System.Web.Http;
using MySql.Data.MySqlClient;

namespace KoFrMaRestApi
{
    public static class WebApiConfig
    {
        //https://dev.mysql.com/doc/dev/connector-net/6.10/html/T_MySql_Data_MySqlClient_MySqlConnection.htm 

        public static MySqlConnection Connection()
        {
            string connectionS = "server=mysqlstudenti.litv.sssvt.cz;uid=kocourekmatej;pwd=123456;database=3b1_kocourekmatej_db2";

            MySqlConnection Connection = new MySqlConnection(connectionS);

            return Connection;

        }


        public static void Register(HttpConfiguration config)
        {
            //překládání xml na json
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            //Služby a konfigurace rozhraní Web API
            //config.Formatters.JsonFormatter
            //Trasy rozhraní Web API
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
               routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
           );
        }
    }
}
