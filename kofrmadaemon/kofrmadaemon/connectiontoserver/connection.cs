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
        public List<Tasks> PostRequest()
        {
            DaemonInfo daemon = DaemonInfo.Instance;
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConnectionInfo.ServerURL + @"/api/Daemon/GetInstructions");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(daemon);

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            string result;
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }
            return JsonConvert.DeserializeObject<List<Tasks>>(result);
        }
        public void TaskCompleted(Tasks task, DebugLog debugLog, bool Successfull)
        {
            TaskComplete completedTask = new TaskComplete() { IDTask = task.IDTask, TimeOfCompletition = DateTime.Now, DebugLog = debugLog.logReport, DatFilePath = task.WhereToBackup, IsSuccessfull = Successfull };

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConnectionInfo.ServerURL + @"/api/Daemon/TaskCompleted");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(completedTask);

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }
        }

    }
}
