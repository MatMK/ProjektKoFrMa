﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
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
        private Timer timer;
        public static DebugLog debugLog;
        Connection connection;
        DaemonInfo daemon;
        Password daemonPassword;

        private bool inProgress;

        /// <summary>
        /// Naplánované úlohy přijaté od serveru se budou přidávat do tohoto listu
        /// </summary>
        private List<Tasks> ScheduledTasks;

        public ServiceKoFrMa()
        {

            InitializeComponent();

            


            ScheduledTasks = new List<Tasks>();

            inProgress = false;
            timer = new Timer(5000);
            timer.Elapsed += new ElapsedEventHandler(OnTimerTick);
            timer.AutoReset = true;
            Password password = Password.Instance;
            DaemonSettings daemonSettings = new DaemonSettings();


            debugLog = new DebugLog(daemonSettings.LocalLogPath,daemonSettings.WindowsLog, 9);

            /// <summary>
            /// Předávání informací o daemonovi a systému
            /// </summary>

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
            debugLog.WriteToLog("Service started", 4);
            daemon.Token = connection.GetToken();
            timer.Start();
            debugLog.WriteToLog("Daemon version is "+daemon.Version.ToString()+" daemon OS is "+daemon.OS+" and daemon unique motherboard ID is " +daemon.PC_Unique, 6);
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

            //BackupDifferential backupTest = new BackupDifferential();
            //BackupFull fullbackupTestFull = new BackupFull();
            //fullbackupTestFull.BackupFullProcess(@"d:\KoFrMa\BackupThisFolder\", @"d:\KoFrMa\BackupGoesHere\", debugLog);
            //backupTest.BackupDifferentialProcess(@"d:\tmp\testBackup\BackupGoesHere\KoFrMaBackup_2018_02_24_15_14_39_Full\KoFrMaBackup.dat\", @"d:\tmp\testBackup\BackupGoesHere\", debugLog);
            //BackupSwitch backupSwitchTest = new BackupSwitch();
            //try
            //{
            //    backupSwitchTest.Backup(@"d:\Users\Matej\Desktop\KoFrMaBackup\BackupGoesHere\KoFrMaBackup_2018_02_18_20_34_42_Full\KoFrMaBackup.dat", @"d:\Users\Matej\Desktop\KoFrMaBackup\BackupGoesHere\.zip", 0, debugLog);
            //}
            //catch (Exception ex)
            //{
            //    debugLog.WriteToLog(ex.Message, 2);
            //    throw;
            //}


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
                        if (item.TimeToBackup.CompareTo(DateTime.Now) <= 0&&item.InProgress == false) //Pokud čas úlohy už uběhl nebo zrovna neběží
                        {
                            debugLog.WriteToLog("Task " + item.IDTask +" should be running because it was planned to run in " + item.TimeToBackup.ToString() + ", starting the inicialization now...", 6);
                            BackupSwitch backupInstance = new BackupSwitch();
                            try
                            {
                                item.InProgress = true;
                                debugLog.WriteToLog("Task locked, starting the backup...", 6);
                                debugLog.WriteToLog("Destination of the backup is "+item.WhereToBackup, 8);
                                backupInstance.Backup(item.SourceOfBackup,item.BackupJournalSource, item.WhereToBackup,item.CompressionLevel,item.IDTask, debugLog);
                                debugLog.WriteToLog("Task completed, setting task as successfully completed...", 6);
                                connection.TaskCompleted(item, backupInstance.BackupJournalNew,debugLog, true);
                            }
                            catch (Exception ex)
                            {
                                debugLog.WriteToLog("Task failed with fatal error " + ex.Message, 2);
                                connection.TaskCompleted(item, backupInstance.BackupJournalNew, debugLog, false);
                            }
                            finally
                            {
                                debugLog.WriteToLog("Task "+item.IDTask + " ended. Information about the completed task will be send to server on next occasion.", 6);
                                ScheduledTasks.Remove(item);
                                item.InProgress = false;
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
                debugLog.WriteToLog("Service is already in the process of contacting server or stopping, timer action skipped.", 5);
            }

        }


        private void GetTasks()
        {
            int[] ScheduledTasksIdArray = new int[ScheduledTasks.Count];
            int i =0;
            foreach (Tasks item in ScheduledTasks)
            {
                ScheduledTasksIdArray[i] = item.IDTask;
                i++;
            }

            debugLog.WriteToLog("Searching for cached journals in folder "+ Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\KoFrMa\journalcache\", 5);
            List<int> JournalCacheList = new List<int>();
            if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\KoFrMa\journalcache\"))
            {
                try
                {
                    FileInfo[] JournalCacheListDir = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\KoFrMa\journalcache\").GetFiles();
                    for (int x = 0; x < JournalCacheListDir.Length; x++)
                    {
                        try
                        {
                            JournalCacheList.Add(Convert.ToInt32(JournalCacheListDir[i].Name.Substring(0,JournalCacheListDir[i].Name.Length-3)));
                        }
                        catch (Exception)
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
