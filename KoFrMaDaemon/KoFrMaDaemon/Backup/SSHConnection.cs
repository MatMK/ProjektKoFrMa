using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;
using System.IO;
using System.Net;

namespace KoFrMaDaemon.Backup
{
    public class SSHConnection
    {
        private string SSHAddress;
        private NetworkCredential SSHCredentials;
        private SftpClient client;
        private DirectoryInfo directoryInfo;
        public SSHConnection(string SSHAddress, string username, string password, DebugLog debugLog)
        {
            debugLog.WriteToLog("Setting up settings needed for the SSH trasfer...", 7);
            SSHCredentials = new NetworkCredential(username, password);
            this.SSHAddress = SSHAddress;
        }
        public SSHConnection(string SSHAddress, NetworkCredential networkCredential)
        {
            ServiceKoFrMa.debugLog.WriteToLog("Setting up settings needed for the SSH trasfer...", 7);
            SSHCredentials = networkCredential;
            this.SSHAddress = SSHAddress;
        }
        public void UploadToSSH(string PathToFolder)
        {
            ServiceKoFrMa.debugLog.WriteToLog("Connecting to SSH server...", 5);
            //Passing the sftp host without the "sftp://"
            client = new SftpClient(this.SSHAddress, 22, SSHCredentials.UserName, SSHCredentials.Password);
            client.Connect();


            ServiceKoFrMa.debugLog.WriteToLog("Loading list of files and folders to copy...", 5);
            List<string>[] listToCopy = this.LoadListToCopy(PathToFolder);
            ServiceKoFrMa.debugLog.WriteToLog("Creating folder structure...", 5);
            foreach (string item in listToCopy[0])
            {
                try
                {
                    CreateDirectory(item);
                }
                catch (Exception ex)
                {
                    ServiceKoFrMa.debugLog.WriteToLog("Directory " + item + " could not be created because of error " + ex.Message, 3);
                    throw;
                }

            }
            ServiceKoFrMa.debugLog.WriteToLog("Transfering files...", 5);
            foreach (string item in listToCopy[1])
            {
                this.UploadFile(item, (new FileInfo(item).DirectoryName).Substring(PathToFolder.Length));
            }
            client.Disconnect();
            client.Dispose();
        }

        private void UploadFile(string pathSource, string pathDestination)
        {
            FileInfo f = new FileInfo(pathSource);
            string uploadfile = f.FullName;
            //Console.WriteLine(f.Name);
            //Console.WriteLine("uploadfile" + uploadfile);


            if (client.IsConnected)
            {
                var fileStream = new FileStream(uploadfile, FileMode.Open);
                if (fileStream != null)
                {
                    //If you have a folder located at sftp://ftp.example.com/share
                    //then you can add this like:
                    client.UploadFile(fileStream, @"/"+pathDestination+@"/" + f.Name, null);

                }
            }
        }

        private void CreateDirectory(string pathDestination)
        {
            client.CreateDirectory(pathDestination);
        }




        private List<string>[] LoadListToCopy(string path)
        {

            ServiceKoFrMa.debugLog.WriteToLog("Creating lists for storing files and folder structure...", 7);
            List<string>[] tmpArray = new List<string>[2];

            List<string> FileList = new List<string>();

            List<string> FolderList = new List<string>();
            ServiceKoFrMa.debugLog.WriteToLog("Filling lists...", 7);
            directoryInfo = new DirectoryInfo(path);
            ExploreDirectoryRecursively(directoryInfo, FileList, FolderList);
            ServiceKoFrMa.debugLog.WriteToLog("Reversing lists order for easier folder creation...", 7);
            FolderList.Reverse();
            FileList.Reverse();
            ServiceKoFrMa.debugLog.WriteToLog("Setting references to created lists...", 7);
            tmpArray[0] = FolderList;
            tmpArray[1] = FileList;
            ServiceKoFrMa.debugLog.WriteToLog("Firsts 3 folders are: " + FolderList[0] + ',' + FolderList[1] + ',' + FolderList[2], 8);
            ServiceKoFrMa.debugLog.WriteToLog("Returning array of lists...", 7);
            return tmpArray;

        }

        private void ExploreDirectoryRecursively(DirectoryInfo path, List<string> FileList, List<string> FolderList)
        {
            foreach (FileInfo item in path.GetFiles())
            {
                try
                {
                    FileList.Add(item.FullName);
                }
                catch (Exception)
                {
                    ServiceKoFrMa.debugLog.WriteToLog("Cannot load folder " + item.FullName + " from temporary backup loaction for FTP transfer. File will be skipped!", 3);
                }

            }

            foreach (DirectoryInfo item in path.GetDirectories())
            {
                try
                {
                    this.ExploreDirectoryRecursively(item, FileList, FolderList);
                    FolderList.Add(item.FullName.Substring(directoryInfo.FullName.Length));
                }
                catch (Exception)
                {
                    ServiceKoFrMa.debugLog.WriteToLog("Cannot load file " + item.FullName + " from temporary backup loaction for FTP transfer. Folder will be skipped!", 3);
                }

            }
        }
    }
}
