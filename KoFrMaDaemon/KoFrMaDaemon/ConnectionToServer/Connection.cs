using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System.Net;
using Newtonsoft.Json;


namespace KoFrMaDaemon.ConnectionToServer
{
    
    public class Connection
    {
        /// <summary>
        /// Zavolá server a dá mu informace o své verzi, operačním systému a unikátní klíč počítače
        /// </summary>
        /// <returns>Vrací nový Task ze serveru</returns>
        public Tasks PostRequest()
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:50576/api/Daemon/GetInstructions");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = @"{
	Version: 1,
	Os: 1,
	PC_Unique: '2'
}";

                streamWriter.Write(json);
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            string response;
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                response = streamReader.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<Tasks>(response);
        }
    }
}
