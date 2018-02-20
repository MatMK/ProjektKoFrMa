using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.IO;
using System.Net;
using KoFrMaDaemon.ConnectionToServer;

namespace KoFrMaDaemon
{
    public partial class ServiceKoFrMa : ServiceBase
    {
        //private const string servicePrefixName = "KoFrMa";

        //private const byte version = 101;

        private bool inProgress;

        private Timer timer;

        private string logPath;

        private DebugLog debugLog;

        private Actions a = new Actions();

        private List<Tasks> ScheduledTasks;

        Connection connection = new Connection();
        DaemonInfo daemon = DaemonInfo.Instance;
        //Naplánované úlohy přijaté od serveru se budou přidávat do tohoto listu

        public ServiceKoFrMa()
        {
            InitializeComponent();
            ScheduledTasks = new List<Tasks>();
            timer = new Timer(5000);
            timer.Elapsed += new ElapsedEventHandler(OnTimerTick);
            inProgress = false;
            this.logPath = @"d:\Users\Matej\Desktop\KoFrMaBackup\DebugServiceLog.log";
            debugLog = new DebugLog(this.logPath, 8);
            ConnectionInfo.ServerURL = @"http://localhost:50576/api/Daemon/GetInstructions";
            daemon.OS = 1;
            daemon.PC_Unique = "1";
            daemon.Version = 1;
                
            timer.AutoReset = true;
            //this.serverURL = @"http://localhost:50576/";
        }

        protected override void OnStart(string[] args)
        {
            debugLog.WriteToLog("Service started", 4);
            timer.Start();
            
            //a.BackupFullFolder(@"d:\Users\Matej\Desktop\KoFrMaBackup\BackupThisFolder\", @"d:\Users\Matej\Desktop\KoFrMaBackup\BackupGoesHere\", debugLog);
            //a.BackupDifferential(@"d:\Users\Matej\Desktop\KoFrMaBackup\BackupGoesHere\", @"d:\Users\Matej\Desktop\KoFrMaBackup\BackupGoesHere\KoFrMaBackup_2018_02_18_20_34_42_Full\KoFrMaBackup.dat", debugLog);
            //a.BackupDifferential(@"d:\tmp\testBackup\BackupGoesHere\", @"d:\tmp\testBackup\BackupGoesHere\KoFrMaBackup_2018_02_18_13_58_48_Full\KoFrMaBackup.dat", debugLog);
        }

        protected override void OnStop()
        {
            this.inProgress = true;
            debugLog.WriteToLog("Service stopped", 4);
        }



        private void OnTimerTick(object sender, ElapsedEventArgs e)
        {
            debugLog.WriteToLog("Timer tick", 7);
            if (!this.inProgress) //Pokud se service zrovna nevypíná, třeba aby při vypínání Windows nezačala běžet nová úloha nebo pokud se zrovna neprohledává seznam úloh (běží asynchonně)
            {
                this.inProgress = true;
                debugLog.WriteToLog("Updating list of scheduled tasks from the server...", 5);
                this.GetTasks();
                debugLog.WriteToLog("List of scheduled tasks now contains " + this.ScheduledTasks.Count + " tasks", 6);
                this.inProgress = false;
                if (this.ScheduledTasks.Count>0)
                {
                    debugLog.WriteToLog("Tasks found, starting to check if the time has come for each of the tasks", 5);

                    foreach (Tasks item in ScheduledTasks)
                    {
                        debugLog.WriteToLog("Checking if the task should be started for task with ID " + item.IDTask, 7);
                        if (item.TimeToBackup.CompareTo(DateTime.Now) >= 0&&item.InProgress == false) //Pokud čas úlohy už uběhl nebo zrovna neběží
                        {
                            debugLog.WriteToLog("Task " + item.IDTask +" should be running because it was planned to run in " + item.TimeToBackup.ToString() + ", starting the inicialization now...", 6);
                            Actions action = new Actions();
                            if (item.SourceOfBackup.EndsWith(".dat")) //když bude jako zdroj úlohy nastaven path na soubor .dat provede se diferenciální, jinak pokud je to složka tak incrementální
                            {
                                debugLog.WriteToLog("Starting differential/incremental backup, because the path to source ends with .dat (" + item.SourceOfBackup + ')', 7);
                                if (item.WhereToBackup.EndsWith(".zip") || item.WhereToBackup.EndsWith(".rar") || item.WhereToBackup.EndsWith(".7z"))
                                {
                                    item.InProgress = true;
                                    debugLog.WriteToLog("Starting backuping to archive, because the path to destination ends with .zip, .rar or .7z (" + item.SourceOfBackup + ')', 7);
                                    //action.BackupDifferential(item.WhereToBackup, item.SourceOfBackup, debugLog);
                                    //udělat komprimaci
                                }
                                else
                                {
                                    item.InProgress = true;
                                    debugLog.WriteToLog("Starting plain copy backup, because the path to destination doesn't end with .zip, .rar or .7z (" + item.SourceOfBackup + ')', 7);
                                    action.BackupDifferential(item.WhereToBackup, item.SourceOfBackup, debugLog);
                                }

                            }
                            else
                            {
                                if (item.WhereToBackup.EndsWith(".zip") || item.WhereToBackup.EndsWith(".rar") || item.WhereToBackup.EndsWith(".7z"))
                                {
                                    item.InProgress = true;
                                    debugLog.WriteToLog("Starting backuping to archive, because the path to destination ends with .zip, .rar or .7z (" + item.SourceOfBackup + ')', 7);
                                    //action.BackupFullFolder(item.SourceOfBackup, item.WhereToBackup, debugLog);
                                    //udělat komprimaci
                                }
                                else
                                {
                                    item.InProgress = true;
                                    debugLog.WriteToLog("Starting plain copy backup, because the path to destination doesn't end with .zip, .rar or .7z (" + item.SourceOfBackup + ')', 8);
                                    action.BackupFullFolder(item.SourceOfBackup, item.WhereToBackup, debugLog);
                                }

                            }
                        }
                        else
                        {
                            debugLog.WriteToLog("Task " + item.IDTask + " is skipped because " + item.TimeToBackup.ToString() + " is in the future", 6);
                        }
                    }
                }
                else
                {
                    debugLog.WriteToLog("No other tasks planned, service will check again after " + timer.Interval / 1000 + 's', 5);
                }
                

            }
            else
            {
                debugLog.WriteToLog("Service is in the process of stopping, skipping regular timer action...", 5);
            }

        }



        /*
         Vstupní parametry GetTasks():
         ID
         IdentityOS
         Heslo
         Session
         OS demona
         Verze démona
         LokalniCas
         LastTasksResults: N-dim; ID_Result; ReturnCode; ID_Result; ReturnCode; ...0
         



         int32 i =15;
         CopyBin(ByteCile, byte[4], 4, i)


        Navrácené data:
        N-tasks; TypeOfTask; Par0..M;  


         */


        private void GetTasks()
        {
            ScheduledTasks = connection.PostRequest();

            //log.WriteToLog("InGetTasks 1");

            //WebRequest request = WebRequest.Create(this.serverURL);
            //request.Method = "POST";
            //request.ContentType = "multipart/form-data"; // ideální pro Upload souborů
            //request.ContentLength = 4;
            //log.WriteToLog("InGetTasks 2");

            //WebResponse response = request.GetResponse();
            //log.WriteToLog("InGetTasks 3");

            //string statusDescr = "StatusDescription = " + ((HttpWebResponse)response).StatusDescription;

            //debugLog.WriteToLog(statusDescr, 5);
            /*
             // Get the stream containing content returned by the server.
             dataStream = response.GetResponseStream();
             byte[] buffer = new byte[16000];
             int ReadCount = dataStream.Read(buffer, 0, buffer.Length);

             dataStream.Close();
             response.Close();
  */
        }
    }
}
