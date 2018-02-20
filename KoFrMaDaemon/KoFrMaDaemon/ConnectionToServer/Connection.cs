using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System.Net;


namespace KoFrMaDaemon.ConnectionToServer
{
    public class Connection
    {
        public void PostRequest()
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
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            string reslut;
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                reslut = streamReader.ReadToEnd();
            }
        }
    }
}
