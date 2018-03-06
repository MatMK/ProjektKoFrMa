using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using KoFrMaDaemon.Backup;

namespace KoFrMaDaemon.ConnectionToServer
{
    public class Connection
    {
        public List<Tasks> PostRequest(int[] TasksId, int[] BackupJournalNotNeeded)
        {
            ServiceKoFrMa.debugLog.WriteToLog("Creating request to server...", 7);
            Request request = new Request() {IdTasks = TasksId };
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConnectionInfo.ServerURL + @"/api/Daemon/GetInstructions");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            ServiceKoFrMa.debugLog.WriteToLog("Trying to send request to server at address " + ConnectionInfo.ServerURL + @"/api/Daemon/GetInstructions", 5);
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(request);
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }
            ServiceKoFrMa.debugLog.WriteToLog("Trying to receive response from server at address "+ ConnectionInfo.ServerURL + @"/api/Daemon/GetInstructions",5);
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            ServiceKoFrMa.debugLog.WriteToLog("Server returned code " + httpResponse.StatusCode + "which means " + httpResponse.StatusDescription, 5);
            string result;
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }
            ServiceKoFrMa.debugLog.WriteToLog("Performing deserialization of data that were received from the server...", 7);
            return JsonConvert.DeserializeObject<List<Tasks>>(result);
        }
        public void TaskCompleted(Tasks task, BackupJournalObject backupJournalNew, DebugLog debugLog, bool Successfull)
        {
            TaskComplete completedTask = new TaskComplete()
            {
                IDTask = task.IDTask,
                TimeOfCompletition = DateTime.Now,
                DebugLog = debugLog.logReport,
                DatFile = backupJournalNew,
                IsSuccessfull = Successfull
            };

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
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            string result;
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }
        }
        public string GetToken()
        {

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConnectionInfo.ServerURL + @"/api/Daemon/RegisterToken");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(Password.Instance);

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
            return result;
        }

    }
}
