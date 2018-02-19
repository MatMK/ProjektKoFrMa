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

namespace KoFrMaDaemon
{
    public partial class ServiceKoFrMa : ServiceBase
    {
        //private const string servicePrefixName = "KoFrMa";

        //private const byte version = 101;

        private bool isStopping;

        private Timer timer;

        private int timerStep;

        private string logPath;

        private string serverURL;

        private DebugLog debugLog;

        private Actions a = new Actions();

        private List<Tasks> ScheduledTasks;
        //Naplánované úlohy přijaté od serveru se budou přidávat do tohoto listu

        public ServiceKoFrMa()
        {
            InitializeComponent();
            ScheduledTasks = new List<Tasks>();
            //timerStep = 5000;
            //timer = new Timer(this.timerStep);
            //timer.Elapsed += new ElapsedEventHandler(OnTimerTick);
            //isStopping = false;
            this.logPath = @"d:\Users\Matej\Desktop\KoFrMaBackup\DebugServiceLog.log";
            debugLog = new DebugLog(this.logPath,8);

            //timer.AutoReset = true;
            //this.serverURL = @"http://localhost:50576/";
        }

        protected override void OnStart(string[] args)
        {
            debugLog.WriteToLog("Service started",4);
            //timer.Start();

            //a.BackupFullFolder(@"d:\Users\Matej\Desktop\KoFrMaBackup\BackupThisFolder\", @"d:\Users\Matej\Desktop\KoFrMaBackup\BackupGoesHere\", debugLog);
            a.BackupDifferential(@"d:\Users\Matej\Desktop\KoFrMaBackup\BackupGoesHere\", @"d:\Users\Matej\Desktop\KoFrMaBackup\BackupGoesHere\KoFrMaBackup_2018_02_18_20_34_42_Full\KoFrMaBackup.dat", debugLog);
            //a.BackupDifferential(@"d:\tmp\testBackup\BackupGoesHere\", @"d:\tmp\testBackup\BackupGoesHere\KoFrMaBackup_2018_02_18_13_58_48_Full\KoFrMaBackup.dat", debugLog);
        }

        protected override void OnStop()
        {
            //this.isStopping = true;
            debugLog.WriteToLog("Service stopped",4);
        }



        private void OnTimerTick(object sender, ElapsedEventArgs e)
        {
            if (!this.isStopping)
            {
                //log.WriteToLog("tik");

                this.GetTasks();
                //log.WriteToLog("tikTasksGot");
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
            //log.WriteToLog("InGetTasks 1");

            WebRequest request = WebRequest.Create(this.serverURL);
            //request.Method = "POST";
            //request.ContentType = "multipart/form-data"; // ideální pro Upload souborů
            //request.ContentLength = 4;
            //log.WriteToLog("InGetTasks 2");

            WebResponse response = request.GetResponse();
            //log.WriteToLog("InGetTasks 3");

            string statusDescr = "StatusDescription = " + ((HttpWebResponse)response).StatusDescription;

            debugLog.WriteToLog(statusDescr,6);
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
