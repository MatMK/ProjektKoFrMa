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
    public partial class KoFrMaDaemon : ServiceBase
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
        //List<int> JournalCacheList;
        public KoFrMaDaemon()
        {
            InitializeComponent();

            
            ScheduledTasks = new List<Task>();

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


            debugLog = new DebugLog(daemonSettings.LocalLogPath, daemonSettings.WindowsLog, 9);

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

            //JournalCacheList = new List<int>();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                debugLog.WriteToLog("Service started", 4);
                debugLog.WriteToLog("Daemon version is " + daemon.Version.ToString() + " daemon OS is " + daemon.OS + " and daemon unique motherboard ID is " + daemon.PC_Unique, 7);

                if (daemonSettings.ServerIP == "" || daemonSettings.ServerIP == null)
                {
                    //debugLog.WriteToLog("Server IP not entered in config file. Run local configurator to set it to your server, than restart this service or computer.", 1);
                    this.CheatTasks();
                    //throw new ArgumentNullException("ServerIP","Server IP not entered in config file. Run local configurator to set it to your server, than restart this service or computer.");
                }
                else
                {
                    timerConnection.Start();
                }

                this.CompletedTasksYetToSend = this.LoadCompletedTasksFromDisk();

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
                debugLog.WriteToLog("Cannot start service because of error: " + ex.Message + ex, 1);
                throw;
            }

        }

        protected override void OnStop()
        {
            debugLog.WriteToLog("Stopping service...", 6);
            this.inProgress = true;
            //this.SaveCompletedTasksToDisk(CompletedTasksYetToSend);
            debugLog.WriteToLog("Service stopped.", 4);
        }

        private void CheatTasks()
        {
            BackupJournalOperations cheatBackupJournalOperations = new BackupJournalOperations();
            DateTime timeToBackup = DateTime.Now;
            List<IDestination> tmpDestinations = new List<IDestination>();
            SourceFolders tmpSourceFolders = new SourceFolders();
            List<string> tmpSourceFoldersPaths = new List<string>();
            tmpSourceFoldersPaths.Add(@"D:\KoFrMa\BackupThisFolder\");
            tmpSourceFolders.Paths = tmpSourceFoldersPaths;
            SourceMSSQL sourceMSSQL = new SourceMSSQL();
            sourceMSSQL.ServerName = "(local)";
            sourceMSSQL.DatabaseName = "dbNw";
            sourceMSSQL.NetworkCredential = new System.Net.NetworkCredential() { UserName = "dbo",Password = "123456"};

            tmpDestinations.Add(new DestinationPlain() { Path = new DestinationPathLocal() { Path = @"d:\KoFrMa\BackupGoesHere\" } });
            tmpDestinations.Add(new Destination7z() { Path = new DestinationPathLocal() { Path = @"d:\KoFrMa\BackupGoesHere\" }, CompressionLevel = 0, SplitAfter = 5 });
            Task taskTest = new Task
            {
                //Sources = cheatBackupJournalOperations.LoadBackupJournalObject(@"d:\KoFrMa\BackupGoesHere\KoFrMaBackup_2018_06_02_15_02_59\KoFrMaBackup.dat", debugLog),
                //Sources = tmpSourceFolders,
                Sources = sourceMSSQL,
                IDTask = 0,
                LogLevel = 8,
                TemporaryFolderMaxBuffer = null,
                Destinations = tmpDestinations,
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
                List<Task> tasksDelete = new List<Task>();
                debugLog.WriteToLog("Tasks found, starting to check if the time has come for each of the tasks", 5);
                bool successfull = false;
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
                                if (item.ScriptBefore != null)
                                {
                                    if (item.ScriptBefore.PathToLocalScript != null || item.ScriptBefore.PathToLocalScript != "")
                                    {
                                        debugLog.WriteToLog("Runnig script from disk...", 6);
                                        this.RunScriptFromDisk(item.ScriptBefore.PathToLocalScript);
                                    }
                                    else if (item.ScriptBefore.ScriptItself != null || item.ScriptBefore.ScriptItself != "")
                                    {
                                        debugLog.WriteToLog("Runnig script included with the task...", 6);
                                        this.RunScriptFromString(item.ScriptBefore.ScriptItself, item.ScriptBefore.ScriptItselfFormat);
                                    }
                                }
                                //debugLog.WriteToLog("Destination of the backup is " + item.WhereToBackup[0], 8);



                                backupInstance.Backup(this.LoadJournalFromCacheIfNeeded(item));

                                debugLog.WriteToLog("Task completed, setting task as successfully completed...", 6);
                                successfull = true;
                                //connection.TaskCompleted(item, backupInstance.BackupJournalNew, debugLog, true);
                            }
                            catch (Exception ex)
                            {
                                debugLog.WriteToLog("Task failed with fatal error " + ex, 2);
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
                                if (daemonSettings.LocalLogPath != null && daemonSettings.LocalLogPath != "")
                                {
                                    StreamWriter w = new StreamWriter(daemonSettings.LocalLogPath, true);
                                    for (int i = 0; i < backupInstance.taskDebugLog.logReport.Count; i++)
                                    {
                                        w.WriteLine(backupInstance.taskDebugLog.logReport[i]);
                                    }
                                    w.Close();
                                    w.Dispose();

                                }


                                debugLog.WriteToLog("Task " + item.IDTask + " ended. Information about the completed task will be send with the rest to the server on next occasion.", 6);
                                TaskComplete completedTask = new TaskComplete { TimeOfCompletition = DateTime.Now, IDTask = item.IDTask, DatFile = backupInstance.BackupJournalNew, IsSuccessfull = successfull,DebugLog = backupInstance.taskDebugLog.logReport };
                                CompletedTasksYetToSend.Add(completedTask);
                                this.SaveCompletedTaskToDisk(completedTask);
                                tasksDelete.Add(item);
                                //, DebugLog = backupInstance.taskDebugLog.logReport
                            }
                        }
                        else
                        {
                            TimeSpan tmp = item.TimeToBackup - DateTime.Now;
                            if (tmp.TotalMilliseconds < 2147483647)
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
                foreach (Task item in tasksDelete)
                {
                    ScheduledTasks.Remove(item);
                }
                tasksDelete = null;
                if (timerTasks.Interval == 2147483647)
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
                        RegisterData data = connection.GetToken();
                        this.timerValues.ConnectionFailed = data.TimerTick.AfterFailed*1000;
                        this.timerValues.ConnectionSuccess = data.TimerTick.TimerTick*1000;
                        this.timerValues.OnStart = data.TimerTick.AfterStart*1000;

                        daemon.Token = data.Token;
                        debugLog.WriteToLog("Token obtained.", 5);
                        this.timerConnection.AutoReset = false;
                    }
                    catch (Exception ex)
                    {
                        debugLog.WriteToLog("Token couldn't be obtained from the server. Waiting for next timer event to try to obtaine one. " + ex.Message, 3);
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

                        if (this.ScheduledTasks.Count > 0)
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
                currentTasks.Add(new TaskVersion { TaskID = item.IDTask, TaskDataHash = item.GetHashCode() });
            }

            debugLog.WriteToLog("Searching for cached journals in folder " + Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\KoFrMa\journalcache\", 6);

            this.SearchJournalCacheFolder();
            List<Task> taskListReceived = new List<Task>() ;
            try
            {
                List<int> currentCompletedTasksIDs = new List<int>(CompletedTasksYetToSend.Count) ;
                for (int i = 0; i < CompletedTasksYetToSend.Count; i++)
                {
                    currentCompletedTasksIDs.Add(CompletedTasksYetToSend[i].IDTask);
                }
                try
                {
                    taskListReceived = connection.PostRequest(currentTasks, this.SearchJournalCacheFolder(), CompletedTasksYetToSend);
                    for (int i = 0; i < currentCompletedTasksIDs.Count; i++)
                    {
                        this.DeleteCompletedTaskDromDisk(currentCompletedTasksIDs[i]);
                    }
                }
                catch (Exception)
                {

                }
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
            if (path.EndsWith(".bat") || path.EndsWith(".cmd"))
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
                debugLog.WriteToLog("Unknown file should be run as script, not my problem, trying to start it anyway." + path, 6);
                ProcessStartInfo processInfo;
                Process process;

                processInfo = new ProcessStartInfo(path);
                processInfo.CreateNoWindow = true;

                process = Process.Start(processInfo);
                process.PriorityClass = ProcessPriorityClass.BelowNormal;
                process.WaitForExit();
            }

        }

        private void RunScriptFromString(string scriptBase64, string scriptFormat)
        {
            if (scriptFormat == null)
            {
                string script = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(scriptBase64));
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
                    debugLog.WriteToLog("Unsupported script format: " + scriptFormat, 3);
                }
            }
            else
            {
                debugLog.WriteToLog("Script format not set, script must be skipped.", 3);
            }

        }

        private Task LoadJournalFromCacheIfNeeded(Task task)
        {
            Task taskWithLoadedJournal;
            if (task.Sources is SourceJournalLoadFromCache)
            {
                taskWithLoadedJournal = task;
                BackupJournalOperations o = new BackupJournalOperations();

                List<int> currentCacheFiles = this.SearchJournalCacheFolder();

                if (currentCacheFiles.Contains(((SourceJournalLoadFromCache)task.Sources).JournalID))
                {
                    taskWithLoadedJournal.Sources = o.LoadBackupJournalObject(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\KoFrMa\journalcache\" + ((SourceJournalLoadFromCache)task.Sources).JournalID.ToString(), debugLog);
                    debugLog.WriteToLog("Task journal was loaded from local cache and will be used for incremental/differencial backup.", 7);

                }

                else
                {
                    debugLog.WriteToLog("Task journal should be differencial/incremental but is not in cache, server needs to send one. Process fail is inevitable.", 2);
                }
                return taskWithLoadedJournal;

            }
            else
            {
                return task;
            }


            //BackupJournalObject backupJournalSource = null;
            //if (item.BackupJournalSource != null)
            //{
            //    backupJournalSource = item.BackupJournalSource;
            //    debugLog.WriteToLog("Task from the server contains backup journal, using it as journal for incremental/differencial backup.", 7);
            //}
            //else
            //{
            //    try
            //    {
            //        int sourceJournalId;
            //        sourceJournalId = Convert.ToInt32(item.SourceOfBackup);
            //        for (int i = 0; i < JournalCacheList.Count; i++)
            //        {
            //            if (sourceJournalId == JournalCacheList[i])
            //            {
            //                BackupJournalOperations o = new BackupJournalOperations();
            //                backupJournalSource = o.LoadBackupJournalObject(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\KoFrMa\journalcache\" + item.SourceOfBackup, debugLog);
            //                debugLog.WriteToLog("Task journal was loaded from local cache and will be used for incremental/differencial backup.", 7);
            //            }
            //            else
            //            {
            //                debugLog.WriteToLog("Task journal should be differencial/incremental but is not in cache, server needs to send one. Process fail is inevitable.", 2);
            //            }
            //        }

            //    }
            //    catch (Exception)
            //    {
            //        debugLog.WriteToLog("Task from the server doesn't contain backup journal, using the path entered for full backup.", 7);
            //    }
            //}
        }
        private List<int> SearchJournalCacheFolder()
        {
            List<int> tmpList = new List<int>();
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
                                tmpList.Add(Convert.ToInt32(Path.GetFileNameWithoutExtension(JournalCacheListDir[i].Name)));
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
                debugLog.WriteToLog("Found " + tmpList.Count + " cache files. Returning their names to the server.", 5);
            }
            else
            {
                debugLog.WriteToLog("The cache jounal folder doesn't exist, returning empty list of cached journals", 3);
            }
            return tmpList;
        }

        private void SaveCompletedTasksToDiskList(List<TaskComplete> tasks)
        {
            if (tasks.Count>0)
            {
                StreamWriter w = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\KoFrMa\CompletedTasksBuffer.dat", true);
                for (int i = 0; i < tasks.Count; i++)
                {
                    w.WriteLine(JsonSerializationUtility.Serialize(tasks[i]));
                }
                w.Close();
                w.Dispose();
            }

        }
        private void SaveCompletedTaskToDisk(TaskComplete task)
        {
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\KoFrMa\CompletedTasksBuffer\");
            StreamWriter w = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\KoFrMa\CompletedTasksBuffer\"+task.IDTask+".dat", true);
            w.WriteLine(JsonSerializationUtility.Serialize(task));
            w.Close();
            w.Dispose();
        }
        private List<int> SearchCompletedTasksCacheFolder()
        {
            List<int> tmpList = new List<int>();
            if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\KoFrMa\CompletedTasksBuffer\"))
            {
                try
                {
                    FileInfo[] taskIDs = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\KoFrMa\CompletedTasksBuffer\").GetFiles();
                    for (int i = 0; i < taskIDs.Length; i++)
                    {
                        if (taskIDs[i].Name.EndsWith(".dat"))
                        {
                            try
                            {
                                tmpList.Add(Convert.ToInt32(Path.GetFileNameWithoutExtension(taskIDs[i].Name)));
                            }
                            catch (Exception)
                            {
                                debugLog.WriteToLog("File isn't named as ID of a task: " + taskIDs[i].Name, 3);
                            }
                        }
                        else
                        {
                            debugLog.WriteToLog("Couldn't identify file from the folder. File name is: " + taskIDs[i].Name, 3);
                        }

                    }
                }
                catch (Exception)
                {
                    debugLog.WriteToLog("Error occured when trying to load cached files. List may be incomplete or, most likely, empty.", 3);
                }
                debugLog.WriteToLog("Found " + tmpList.Count + " cache files. Returning their names to the server.", 5);
            }
            else
            {
                debugLog.WriteToLog("The cache jounal folder doesn't exist, returning empty list of cached journals", 3);
            }
            return tmpList;
        }
        private List<TaskComplete> LoadCompletedTasksFromDiskList()
        {
            List<TaskComplete> tmp = new List<TaskComplete>();
            StreamReader r;
            try
            {
                FileInfo file = new FileInfo(Environment.SpecialFolder.CommonApplicationData + @"\KoFrMa\CompletedTasksBuffer.dat");
                if (file.Exists)
                {
                    r = new StreamReader(file.FullName);
                    while (!r.EndOfStream)
                    {
                        tmp.Add(JsonSerializationUtility.Deserialize<TaskComplete>(r.ReadLine()));
                    }
                    file.Delete();
                }
            }
            catch (Exception)
            {

            }

            return tmp;
        }

        private List<TaskComplete> LoadCompletedTasksFromDisk()
        {
            List<TaskComplete> tmp = new List<TaskComplete>();
            StreamReader r;
            try
            {
                List<int> listIDs = this.SearchCompletedTasksCacheFolder();
                FileInfo file;
                for (int i = 0; i < listIDs.Count; i++)
                {
                    file = new FileInfo(Environment.SpecialFolder.CommonApplicationData + @"\KoFrMa\CompletedTasksBuffer\" + listIDs[i] + ".dat");
                    if (file.Exists)
                    {
                        r = new StreamReader(file.FullName);
                        while (!r.EndOfStream)
                        {
                            tmp.Add(JsonSerializationUtility.Deserialize<TaskComplete>(r.ReadLine()));
                        }
                    }
                }

            }
            catch (Exception)
            {

            }

            return tmp;
        }

        private void DeleteCompletedTaskDromDisk(int id)
        {
            try
            {
                FileInfo file = new FileInfo(Environment.SpecialFolder.CommonApplicationData + @"\KoFrMa\CompletedTasksBuffer.dat" + id + ".dat");
                if (file.Exists)
                {
                    file.Delete();
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
