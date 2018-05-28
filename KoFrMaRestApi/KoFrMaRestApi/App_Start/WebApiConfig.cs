using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Cors;
using KoFrMaRestApi.Models;
using MySql.Data.MySqlClient;
using System.Web.Http.ExceptionHandling;

namespace KoFrMaRestApi
{
    /// <summary>
    /// RestApi main configuration
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// Url of AdminApp. 
        /// Used for Email links
        /// </summary>
        public static string WebServerURL = "http://localhost:4200/";
        /// <summary>
        /// returns MySqlConnection
        /// </summary>
        /// <returns>MySqlConnection</returns>
        public static MySqlConnection Connection()
        {
            string connectionS = "server=mysqlstudenti.litv.sssvt.cz;uid=kocourekmatej;pwd=KoFrMa123456;database=3b1_kocourekmatej_db2";

            MySqlConnection Connection = new MySqlConnection(connectionS);

            return Connection;

        }
        /// <summary>
        /// Sets up APIs
        /// </summary>
        /// <param name="config"></param>
        public static void  Register(HttpConfiguration config)
        {
            config.Services.Add(typeof(IExceptionLogger), new ErrorObserver());
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            config.Formatters.JsonFormatter.SerializerSettings.SerializationBinder = JsonSerializationUtility.jsonSettings.SerializationBinder;
            config.Formatters.JsonFormatter.SerializerSettings.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto;
            //config.Formatters.JsonFormatter
            //Trasy rozhraní Web API
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
               routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
           );
            EnableCorsAttribute cors = new EnableCorsAttribute(WebServerURL, "*", "GET,POST");
            config.EnableCors(cors);
        }
    }
}
