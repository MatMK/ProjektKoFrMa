using System;
using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Diagnostics;
//using System.Linq;
using System.ServiceProcess;
//using System.Text;
//using System.Threading.Tasks;
using System.Timers;
//using System.IO;
//using System.Net;
using KoFrMaDaemon.ConnectionToServer;
using System.Management;
using KoFrMaDaemon.Backup;
using System.IO;

namespace KoFrMaDaemon
{
    public partial class ServiceKoFrMa : ServiceBase
    {
        private bool inProgress;
        private Timer timerTasks;
        private Timer timerConnection;
        public static DebugLog debugLog;
        Connection connection;
        DaemonInfo daemon;
        Password daemonPassword;
        TimerValues timerValues;
        public static DaemonSettings daemonSettings;

        /// <summary>
        /// Naplánované úlohy přijaté od serveru se budou přidávat do tohoto listu
        /// </summary>
        private List<Task> ScheduledTasks;
        /// <summary>
        /// Dokončené úlohy se přidávají do tohoto listu, při připojení se odešlou na server
        /// </summary>
        private List<TaskComplete> CompletedTasksYetToSend;

        public ServiceKoFrMa()
        {
            InitializeComponent();


            ScheduledTasks = new List<Task>();
            CompletedTasksYetToSend = new List<TaskComplete>();

            inProgress = false;

            timerValues = new TimerValues();
            this.timerValues.OnStart = 5000;
            this.timerValues.ConnectionSuccess = 5000;
            this.timerValues.ConnectionFailed = 5000;


            timerConnection = new Timer(timerValues.OnStart);
            timerConnection.Elapsed += new ElapsedEventHandler(OnTimerConnectionTick);
            timerConnection.AutoReset = false;

            timerTasks = new Timer(1);
            timerTasks.Elapsed += new ElapsedEventHandler(OnTimerTasksTick);
            timerTasks.AutoReset = false;


            Password password = Password.Instance;
            daemonSettings = new DaemonSettings();


            debugLog = new DebugLog(daemonSettings.LocalLogPath,daemonSettings.WindowsLog, 9);

            /// Předávání informací o daemonovi a systému
            daemon = DaemonInfo.Instance;
            daemonPassword = Password.Instance;
            daemon.Version = 101;
            daemon.OS = System.Environment.OSVersion.VersionString;
            daemon.PC_Unique = this.GetSerNumBIOS();
            connection = new Connection();
            daemonPassword.SetPassword(daemonSettings.Password);
            ConnectionInfo.ServerURL = daemonSettings.ServerIP;
            password.daemon = daemon;

        }

        protected override void OnStart(string[] args)
        {
            try
            {
                debugLog.WriteToLog("Service started", 4);
                debugLog.WriteToLog("Daemon version is "+daemon.Version.ToString()+" daemon OS is "+daemon.OS+" and daemon unique motherboard ID is " +daemon.PC_Unique, 7);

                //timerConnection.Start();

                this.CheatTasks();

                //try
                //{
                //    FTPConnection fTPConnection = new FTPConnection(@"ftp://e64.cz/WWWRoot/DRead/", "v013823a", "3wZ1ySRlLY8c7k6", debugLog);
                //    fTPConnection.UploadToFTP(@"d:\KoFrMa\BackupThisFolder\");
                //}
                //catch (Exception ex)
                //{
                //    debugLog.WriteToLog(ex.Message, 2);
                //    throw;
                //}
            }
            catch (Exception ex)
            {
                debugLog.WriteToLog("Cannot start service because of error: "+ex.Message + ex, 1);
                throw;
            }

        }

        protected override void OnStop()
        {
            this.inProgress = true;
            debugLog.WriteToLog("Service stopped", 4);
        }

        private void CheatTasks()
        {
            BackupJournalOperations cheatBackupJournalOperations = new BackupJournalOperations();
            DateTime timeToBackup = DateTime.Now;

            ScheduledTasks.Add(new Task {
                SourceOfBackup = @"D:\KoFrMa\BackupThisFolder\",
                //BackupJournalSource = cheatBackupJournalOperations.LoadBackupJournalObject(@"d:\KoFrMa\BackupGoesHere\KoFrMaBackup_2018_02_18_20_34_42_Full\KoFrMaBackup.dat", debugLog),
                IDTask = 1,
                LogLevel = 8,
                CompressionLevel = 0,
                WhereToBackup = new List<string> { (@"d:\KoFrMa\BackupGoesHere\.7z") },
                TimeToBackup = timeToBackup.AddSeconds(1)

            });

            debugLog.WriteToLog("List of scheduled tasks now contains " + this.ScheduledTasks.Count + " tasks", 6);
            if (this.ScheduledTasks.Count > 0)
            {
                debugLog.WriteToLog("Starting scheduled tasks check", 6);
                this.OnTimerTasksTick(null, null);
            }
        }

        private void OnTimerTasksTick(object sender, ElapsedEventArgs e)
        {
            debugLog.WriteToLog("Task timer tick", 8);
            this.timerTasks.Stop();
            this.timerTasks.Interval = 2147483647;
            if (this.ScheduledTasks.Count > 0)
            {
                debugLog.WriteToLog("Tasks found, starting to check if the time has come for each of the tasks", 5);
                bool successfull=false;
                foreach (Task item in ScheduledTasks)
                {
                    if (!item.InProgress)
                    {
                        item.InProgress = true;
                        debugLog.WriteToLog("Checking if the task should be started for task with ID " + item.IDTask, 7);

                        if (item.TimeToBackup.CompareTo(DateTime.Now) <= 0) //Pokud čas úlohy už uběhl nebo zrovna neběží
                        {
                            debugLog.WriteToLog("Task " + item.IDTask + " should be running because it was planned to run in " + item.TimeToBackup.ToString() + ", starting the inicialization now...", 6);
                            BackupSwitch backupInstance = new BackupSwitch();
                            try
                            {
                                debugLog.WriteToLog("Task locked, starting the backup...", 6);
                                debugLog.WriteToLog("Destination of the backup is " + item.WhereToBackup[0], 8);
                                backupInstance.Backup(item.SourceOfBackup, item.BackupJournalSource, item.WhereToBackup, item.CompressionLevel,item.NetworkCredentials, item.IDTask, debugLog);
                                debugLog.WriteToLog("Task completed, setting task as successfully completed...", 6);
                                successfull = true;
                                //connection.TaskCompleted(item, backupInstance.BackupJournalNew, debugLog, true);
                            }
                            catch (Exception ex)
                            {
                                debugLog.WriteToLog("Task failed with fatal error " + ex.Message, 2);
                                //connection.TaskCompleted(item, backupInstance.BackupJournalNew, debugLog, false);
                            }
                            finally
                            {
                                debugLog.WriteToLog("Task " + item.IDTask + " ended. Information about the completed task will be send with the rest to the server on next occasion.", 6);
                                CompletedTasksYetToSend.Add(new TaskComplete { TimeOfCompletition = DateTime.Now, IDTask = item.IDTask, DatFile = backupInstance.BackupJournalNew, IsSuccessfull = successfull });
                                //, DebugLog = backupInstance.taskDebugLog.logReport
                            }
                        }
                        else
                        {
                            TimeSpan tmp = item.TimeToBackup - DateTime.Now;
                            if (tmp.TotalMilliseconds< 2147483647)
                            {
                                if (timerTasks.Interval > tmp.TotalMilliseconds)
                                {
                                    timerTasks.Stop();
                                    timerTasks.Interval = tmp.TotalMilliseconds;
                                    timerTasks.Start();
                                    debugLog.WriteToLog("Timer value set to this task.", 7);
                                }
                                else
                                {
                                    debugLog.WriteToLog("There is another task planned earlier than this one, not changing the timer.", 7);
                                }
                            }
                            else
                            {
                                debugLog.WriteToLog("Task is planned too far in the future, timer not set.", 7);
                            }
                            debugLog.WriteToLog("Task " + item.IDTask + " was skipped because " + item.TimeToBackup.ToString() + " is in future.", 6);

                        }
                        item.InProgress = false;
                        //ScheduledTasks.Remove(item);
                    }
                }
                if (timerTasks.Interval== 2147483647)
                {
                    debugLog.WriteToLog("No other tasks planned", 5);
                }
                else
                {
                    debugLog.WriteToLog("No other tasks started, service will check again after " + timerTasks.Interval / 1000 + 's', 5);
                }

            }
            else
            {
                debugLog.WriteToLog("No tasks planned.", 5);
            }
        }

        private void OnTimerConnectionTick(object sender, ElapsedEventArgs e)
        {
            debugLog.WriteToLog("Connection timer tick", 8);
            if (!this.inProgress) ///Pokud se service zrovna nevypíná, třeba aby při vypínání nezačala běžet nová úloha
            {
                if (daemon.Token == null)
                {
                    debugLog.WriteToLog("Trying to obtain token from the server...", 5);
                    try
                    {
                        daemon.Token = connection.GetToken();
                        debugLog.WriteToLog("Token obtained.", 5);
                        this.timerConnection.AutoReset = false;
                    }
                    catch (Exception ex)
                    {
                        debugLog.WriteToLog("Token couldn't be obtained from the server. Waiting for next timer event to try to obtaine one. "+ex.Message, 3);
                        this.timerConnection.AutoReset = true;
                    }
                }
                else
                {
                    try
                    {
                        debugLog.WriteToLog("Updating list of scheduled tasks from the server...", 5);
                        this.GetTasks();
                        timerConnection.Interval = timerValues.ConnectionSuccess;
                        debugLog.WriteToLog("List of scheduled tasks now contains " + this.ScheduledTasks.Count + " tasks", 6);

                        if (this.ScheduledTasks.Count>0)
                        {
                            debugLog.WriteToLog("Starting scheduled tasks check", 6);
                            this.OnTimerTasksTick(null, null);
                        }
                    }
                    catch (Exception)
                    {
                        timerConnection.Interval = timerValues.ConnectionFailed;
                        throw;
                    }

                }
                //this.inProgress = false;

            }
            //else
            //{
            //    debugLog.WriteToLog("Service is already in the process of contacting server or stopping, timer action skipped.", 5);
            //}
            timerConnection.Start();
        }


        private void GetTasks()
        {
            int[] ScheduledTasksIdArray = new int[ScheduledTasks.Count];
            int i =0;
            foreach (Task item in ScheduledTasks)
            {
                ScheduledTasksIdArray[i] = item.IDTask;
                i++;
            }

            debugLog.WriteToLog("Searching for cached journals in folder "+ Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\KoFrMa\journalcache\", 6);
            List<int> JournalCacheList = new List<int>();
            if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\KoFrMa\journalcache\"))
            {
                try
                {
                    FileInfo[] JournalCacheListDir = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\KoFrMa\journalcache\").GetFiles();
                    for (int x = 0; x < JournalCacheListDir.Length; x++)
                    {
                        if (JournalCacheListDir[i].Name.EndsWith(".dat"))
                        {
                            try
                            {
                                JournalCacheList.Add(Convert.ToInt32(Path.GetFileNameWithoutExtension(JournalCacheListDir[i].Name)));
                            }
                            catch (Exception)
                            {
                                debugLog.WriteToLog("File isn't named as ID of a task: " + JournalCacheListDir[i].Name, 3);
                            }
                        }
                        else
                        {
                            debugLog.WriteToLog("Couldn't identify file from the cache folder. File name is: " + JournalCacheListDir[i].Name, 3);
                        }

                    }
                }
                catch (Exception)
                {
                    debugLog.WriteToLog("Error occured when trying to load cached files. List may be incomplete or, most likely, empty.", 3);
                }
                debugLog.WriteToLog("Found " + JournalCacheList.Count + " cache files. Returning their names to the server.", 5);
            }
            else
            {
                debugLog.WriteToLog("The cache jounal folder doesn't exist, returning empty list of cached journals", 3);
            }
            

            ScheduledTasks.AddRange(connection.PostRequest(ScheduledTasksIdArray,JournalCacheList));
        }

        private string GetSerNumBIOS()
        {
            string lcPopis = "";

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BaseBoard");
            foreach (ManagementObject wmi in searcher.Get())
            {
                try
                {
                    lcPopis = wmi.GetPropertyValue("SerialNumber").ToString().Trim();
                }
                catch { }
            }
            searcher.Dispose();

            //searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BIOS");
            //foreach (ManagementObject wmi in searcher.Get())
            //{
            //    try
            //    {
            //        lcPopis = lcPopis + wmi.GetPropertyValue("SerialNumber").ToString().Trim();
            //    }
            //    catch { }
            //}
            //searcher.Dispose();

            return lcPopis;
        }

    }
}
