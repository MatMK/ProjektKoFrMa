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
using System.Diagnostics;

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
        public static SettingsLoad daemonSettings;

        /// <summary>
        /// Naplánované úlohy přijaté od serveru se budou přidávat do tohoto listu
        /// </summary>
        private List<Task> ScheduledTasks;
        /// <summary>
        /// Dokončené úlohy se přidávají do tohoto listu, při připojení se odešlou na server
        /// </summary>
        private List<TaskComplete> CompletedTasksYetToSend;
        List<int> JournalCacheList;
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
            daemonSettings = new SettingsLoad();


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

            JournalCacheList = new List<int>();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                debugLog.WriteToLog("Service started", 4);
                debugLog.WriteToLog("Daemon version is "+daemon.Version.ToString()+" daemon OS is "+daemon.OS+" and daemon unique motherboard ID is " +daemon.PC_Unique, 7);

                if (daemonSettings.ServerIP==""||daemonSettings.ServerIP==null)
                {
                    debugLog.WriteToLog("Server IP not entered in config file. Run local configurator to set it to your server, than restart this service or computer.", 1);
                }
                else
                {
                    timerConnection.Start();
                }


                //this.CheatTasks();

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
            Task taskTest = new Task
            {
                SourceOfBackup = @"D:\KoFrMa\BackupThisFolder\",
                //BackupJournalSource = cheatBackupJournalOperations.LoadBackupJournalObject(@"d:\KoFrMa\BackupGoesHere\KoFrMaBackup_2018_02_18_20_34_42_Full\KoFrMaBackup.dat", debugLog),
                IDTask = 1,
                LogLevel = 8,
                CompressionLevel = 0,
                TemporaryFolderMaxBuffer = null,
                WhereToBackup = new List<string> { (@"d:\KoFrMa\BackupGoesHere\.rar") },
                TimeToBackup = timeToBackup.AddSeconds(1)
                //ScriptBefore = new ScriptInfo { ScriptItself = @"ping 127.0.0.1 > d:\tmp.txt",ScriptItselfFormat = "bat"},
                //ScriptAfter = new ScriptInfo { ScriptItself = @"ping 127.0.0.1 > d:\tmp.txt", ScriptItselfFormat = "bat" }
                //ScriptAfter = new ScriptInfo { PathToLocalScript = @"c:\Windows\media\Windows Notify.wav" }
            };
            debugLog.WriteJsonTaskToLog(taskTest);
            ScheduledTasks.Add(taskTest);

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
                                if (item.ScriptBefore!=null)
                                {
                                    if (item.ScriptBefore.PathToLocalScript!=null|| item.ScriptBefore.PathToLocalScript != "")
                                    {
                                        debugLog.WriteToLog("Runnig script from disk...", 6);
                                        this.RunScriptFromDisk(item.ScriptBefore.PathToLocalScript);
                                    }
                                    else if (item.ScriptBefore.ScriptItself!=null|| item.ScriptBefore.ScriptItself != "")
                                    {
                                        debugLog.WriteToLog("Runnig script included with the task...", 6);
                                        this.RunScriptFromString(item.ScriptBefore.ScriptItself,item.ScriptBefore.ScriptItselfFormat);
                                    }
                                }
                                debugLog.WriteToLog("Destination of the backup is " + item.WhereToBackup[0], 8);

                                BackupJournalObject backupJournalSource = null;
                                if (item.BackupJournalSource != null)
                                {
                                    backupJournalSource = item.BackupJournalSource;
                                    debugLog.WriteToLog("Task from the server contains backup journal, using it as journal for incemental/differencial backup.", 7);
                                }
                                else
                                {
                                    try
                                    {
                                        int sourceJournalId;
                                        sourceJournalId = Convert.ToInt32(item.SourceOfBackup);
                                        for (int i = 0; i < JournalCacheList.Count; i++)
                                        {
                                            if (sourceJournalId == JournalCacheList[i])
                                            {
                                                BackupJournalOperations o = new BackupJournalOperations();
                                                backupJournalSource = o.LoadBackupJournalObject(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\KoFrMa\journalcache\" + item.SourceOfBackup, debugLog);
                                                debugLog.WriteToLog("Task journal was loaded from local cache and will be used for incremental/differencial backup.", 7);
                                            }
                                            else
                                            {
                                                debugLog.WriteToLog("Task journal should be differencial/incremental but is not in cache, server needs to send one. Process fail is inevitable.", 2);
                                            }
                                        }
                                        
                                    }
                                    catch (Exception)
                                    {
                                        debugLog.WriteToLog("Task from the server doesn't contain backup journal, using the path entered for full backup.", 7);
                                    }
                                }

                                backupInstance.Backup(item.SourceOfBackup, backupJournalSource, item.WhereToBackup, item.CompressionLevel,item.NetworkCredentials, item.IDTask,item.TemporaryFolderMaxBuffer, debugLog);
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
                                if (item.ScriptAfter != null)
                                {
                                    if (item.ScriptAfter.PathToLocalScript != null && item.ScriptAfter.PathToLocalScript != "")
                                    {
                                        debugLog.WriteToLog("Runnig script from disk...", 6);
                                        this.RunScriptFromDisk(item.ScriptAfter.PathToLocalScript);
                                    }
                                    else if (item.ScriptAfter.ScriptItself != null && item.ScriptAfter.ScriptItself != "")
                                    {
                                        debugLog.WriteToLog("Runnig script included with task...", 6);
                                        this.RunScriptFromString(item.ScriptAfter.ScriptItself, item.ScriptAfter.ScriptItselfFormat);
                                    }
                                }
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
                                debugLog.WriteToLog("Task is planned too far in the future, timer not set. This is a problem of server sending tasks too early.", 3);
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
            //int[] ScheduledTasksIdArray = new int[ScheduledTasks.Count];
            //int i =0;
            List<TaskVersion> currentTasks = new List<TaskVersion>(ScheduledTasks.Count);
            foreach (Task item in ScheduledTasks)
            {
                currentTasks.Add(new TaskVersion{ TaskID = item.IDTask, TaskDataHash = item.GetHashCode()});
            }

            debugLog.WriteToLog("Searching for cached journals in folder "+ Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\KoFrMa\journalcache\", 6);

            if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\KoFrMa\journalcache\"))
            {
                try
                {
                    FileInfo[] JournalCacheListDir = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\KoFrMa\journalcache\").GetFiles();
                    for (int i = 0; i < JournalCacheListDir.Length; i++)
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

            try
            {
                List<Task> taskListReceived = connection.PostRequest(currentTasks, JournalCacheList, CompletedTasksYetToSend);
                //přidat ověření než se smažou
                CompletedTasksYetToSend.Clear();
                List<Task> taskListToDelete = new List<Task>();
                foreach (Task taskReceived in taskListReceived)
                {
                    foreach (Task taskLocal in ScheduledTasks)
                    {
                        if (taskLocal.IDTask == taskReceived.IDTask)
                        {
                            taskListToDelete.Add(taskLocal);
                        }
                    }
                }
                for (int y = 0; y < taskListToDelete.Count; y++)
                {
                    ScheduledTasks.Remove(taskListToDelete[y]);
                }
                ScheduledTasks.AddRange(taskListReceived);

            }
            catch (Exception)
            {

                throw;
            }
            
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


        private void RunScriptFromDisk(string path)
        {
            if (path.EndsWith(".bat")|| path.EndsWith(".cmd"))
            {
                debugLog.WriteToLog("Starting command line script located in " + path, 7);
                ProcessStartInfo processInfo;
                Process process;

                processInfo = new ProcessStartInfo(path);
                processInfo.CreateNoWindow = true;
                processInfo.UseShellExecute = false;

                process = Process.Start(processInfo);
                process.PriorityClass = ProcessPriorityClass.BelowNormal;
                process.WaitForExit();
            }
            else if (path.EndsWith(".ps1"))
            {
                debugLog.WriteToLog("Starting the powershell script as follows: powershell.exe -file " + path + "- nologo", 6);
                ProcessStartInfo processInfo;
                Process process;

                processInfo = new ProcessStartInfo("powershell.exe", "-file " + path + " - nologo");
                processInfo.CreateNoWindow = true;

                process = Process.Start(processInfo);
                process.PriorityClass = ProcessPriorityClass.BelowNormal;
                process.WaitForExit();
            }
            else if (path.EndsWith(".vbs"))
            {
                debugLog.WriteToLog("Starting the VBScript as follows: cscript.exe" + @"//B //Nologo" + path, 6);
                ProcessStartInfo processInfo;
                Process process;

                processInfo = new ProcessStartInfo("cscript.exe", @"//B //Nologo" + path);
                processInfo.CreateNoWindow = true;

                process = Process.Start(processInfo);
                process.PriorityClass = ProcessPriorityClass.BelowNormal;
                process.WaitForExit();
            }
            else
            {
                debugLog.WriteToLog("Unknown file should be run as script, not my problem, trying to start it anyway."+path, 6);
                ProcessStartInfo processInfo;
                Process process;

                processInfo = new ProcessStartInfo(path);
                processInfo.CreateNoWindow = true;

                process = Process.Start(processInfo);
                process.PriorityClass = ProcessPriorityClass.BelowNormal;
                process.WaitForExit();
            }

        }

        private void RunScriptFromString(string script, string scriptFormat)
        {
            if (scriptFormat==null)
            {
                if (scriptFormat == "bat" || scriptFormat == "cmd")
                {
                    debugLog.WriteToLog("Starting the script as follows: cmd.exe /c " + script, 6);
                    ProcessStartInfo processInfo;
                    Process process;

                    processInfo = new ProcessStartInfo("cmd.exe", "/c " + script);
                    processInfo.CreateNoWindow = true;

                    process = Process.Start(processInfo);
                    process.PriorityClass = ProcessPriorityClass.BelowNormal;
                    process.WaitForExit();
                }
                else if (scriptFormat == "ps1")
                {
                    debugLog.WriteToLog("Starting the script as follows: powershell.exe -command " + script + " -nologo", 6);
                    ProcessStartInfo processInfo;
                    Process process;

                    processInfo = new ProcessStartInfo("powershell.exe", "-command " + script + " - nologo");
                    processInfo.CreateNoWindow = true;
                    //processInfo.UseShellExecute = false;

                    process = Process.Start(processInfo);
                    process.PriorityClass = ProcessPriorityClass.BelowNormal;
                    process.WaitForExit();
                }
                else
                {
                    debugLog.WriteToLog("Unsupported script format: "+scriptFormat, 3);
                }
            }
            else
            {
                debugLog.WriteToLog("Script format not set, script must be skipped.", 3);
            }
            
        }

    }
}
