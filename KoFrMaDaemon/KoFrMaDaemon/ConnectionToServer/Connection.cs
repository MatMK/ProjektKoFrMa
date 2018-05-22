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
using System.Security.Cryptography;

namespace KoFrMaDaemon.ConnectionToServer
{
    public class Connection
    {
        /// <summary>
        /// Connects to the RestAPI server and tries to obtain list of new backup tasks, while sending already completed tasks back
        /// </summary>
        /// <param name="currentTasks">List of the tasks that the daemon already received before and hash indicating if they changed to the server</param>
        /// <param name="journalNotNeeded">List of tasks that if would be needed doesn't require the server to send the backup journal, bacause a copy of it is already cached offline</param>
        /// <param name="completedTasks">List of completed tasks with all details</param>
        /// <returns></returns>
        public List<Task> PostRequest(List<TaskVersion> currentTasks, List<int> journalNotNeeded,List<TaskComplete> completedTasks)
        {
            ServiceKoFrMa.debugLog.WriteToLog("Creating request to server...", 7);
            Request request = new Request() {TasksVersions=currentTasks,BackupJournalNotNeeded = journalNotNeeded,CompletedTasks = completedTasks};
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConnectionInfo.ServerURL + @"/api/Daemon/GetInstructions");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            ///Timeout aby se aplikace nesekla když se nepřipojí, potom předělat na nastevní hodnoty ze serveru
            httpWebRequest.Timeout = 5000;
            httpWebRequest.ReadWriteTimeout = 32000;

            ServiceKoFrMa.debugLog.WriteToLog("Trying to send request to server at address " + ConnectionInfo.ServerURL + @"/api/Daemon/GetInstructions", 5);
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {

                string json = JsonSerializationUtility.Serialize(request);
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }
            ServiceKoFrMa.debugLog.WriteToLog("Trying to receive response from server at address "+ ConnectionInfo.ServerURL + @"/api/Daemon/GetInstructions",5);
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            ServiceKoFrMa.debugLog.WriteToLog("Server returned code " + httpResponse.StatusCode + " which means " + httpResponse.StatusDescription, 5);
            string result;
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }
            ServiceKoFrMa.debugLog.WriteToLog("Performing deserialization of data that were received from the server...", 7);
            //result = this.DecodeBase64(System.Text.Encoding.UTF8, resultBase64);
            return JsonSerializationUtility.Deserialize<List<Task>>(result);
        }
        //public void TaskCompleted(Task task, BackupJournalObject backupJournalNew, DebugLog debugLog, bool Successfull)
        //{
        //    TaskComplete completedTask = new TaskComplete()
        //    {
        //        IDTask = task.IDTask,
        //        TimeOfCompletition = DateTime.Now,
        //        DebugLog = debugLog.logReport,
        //        DatFile = backupJournalNew,
        //        IsSuccessfull = Successfull
        //    };

        //    var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConnectionInfo.ServerURL + @"/api/Daemon/TaskCompleted");
        //    httpWebRequest.ContentType = "application/json";
        //    httpWebRequest.Method = "POST";
        //    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
        //    {
        //        string json = JsonConvert.SerializeObject(completedTask);

        //        streamWriter.Write(json);
        //        streamWriter.Flush();
        //        streamWriter.Close();
        //    }
        //    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        //    string result;
        //    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        //    {
        //        result = streamReader.ReadToEnd();
        //    }
        //}
        /// <summary>
        /// Connects to the RestAPI server and tries to obtain a token that authorizes the daemon to receives tasks
        /// </summary>
        /// <returns></returns>
        public string GetToken()
        {
            ServiceKoFrMa.debugLog.WriteToLog("Creating token request...", 7);
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConnectionInfo.ServerURL + @"/api/Daemon/RegisterToken");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            ServiceKoFrMa.debugLog.WriteToLog("Trying to send token request to server at address " + ConnectionInfo.ServerURL + @"/api/Daemon/RegisterToken", 6);
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(Password.Instance);
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }
            ServiceKoFrMa.debugLog.WriteToLog("Trying to receive response from server...", 7);
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            ServiceKoFrMa.debugLog.WriteToLog("Server returned code " + httpResponse.StatusCode + " which means " + httpResponse.StatusDescription, 6);
            string result;
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }
            return result;
        }


        private string EncodeBase64(System.Text.Encoding encoding, string text)
        {
            if (text == null)
            {
                return null;
            }

            byte[] textAsBytes = encoding.GetBytes(text);
            return System.Convert.ToBase64String(textAsBytes);
        }

        private string DecodeBase64(System.Text.Encoding encoding, string encodedText)
        {
            if (encodedText == null)
            {
                return null;
            }

            byte[] textAsBytes = System.Convert.FromBase64String(encodedText);
            return encoding.GetString(textAsBytes);
        }
        private string CipherString(string source, string key)
        {
            string tmp="";
            //Aes.Create();
            return tmp;
        }

    }
}
