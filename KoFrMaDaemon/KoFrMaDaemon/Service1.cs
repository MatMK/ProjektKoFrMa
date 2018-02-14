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
        private const string servicePrefixName = "KoFrMa";

        private const byte version = 101;

        private bool isStopping;

        private Timer timer;

        private int timerStep;

        private string logPath;

        private string serverURL;

        public ServiceKoFrMa()
        {
            InitializeComponent();
            timerStep = 5000;
            timer = new Timer(this.timerStep);
            timer.Elapsed += new ElapsedEventHandler(OnTimerTick);
            isStopping = false;
            this.logPath = @"d:\tmp\tmp.txt";
            timer.AutoReset = true;
            this.serverURL = @"http://www.e64.cz/kofrma/ns1.aspx";
        }

        protected override void OnStart(string[] args)
        {
            WriteToLog("Service started");
            timer.Start();

        }

        protected override void OnStop()
        {
            this.isStopping = true;
            WriteToLog("Service stopped");
        }



        private void OnTimerTick(object sender, ElapsedEventArgs e)
        {
            if (!this.isStopping)
            {
                this.WriteToLog("tik");

                this.GetTasks();
                this.WriteToLog("tikTasksGot");
            }

        }

        private void WriteToLog(string text)
        {
            StreamWriter w = new StreamWriter(this.logPath, true);
            w.WriteLine(DateTime.Now.ToString() + ' ' + text);
            w.Close();
            w.Dispose();
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
            WriteToLog("InGetTasks 1");

            WebRequest request = WebRequest.Create(this.serverURL);
            //request.Method = "POST";
            //request.ContentType = "multipart/form-data"; // ideální pro Upload souborů
            //request.ContentLength = 4;
            WriteToLog("InGetTasks 2");

            WebResponse response = request.GetResponse();
            WriteToLog("InGetTasks 3");

            string statusDescr = "StatusDescription = " + ((HttpWebResponse)response).StatusDescription;

            WriteToLog(statusDescr);
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
