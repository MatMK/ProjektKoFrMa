using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace KoFrMaDaemon.Backup
{
    public class FTPConnection
    {
        private string FTPAddress;
        private DebugLog debugLog;
        private NetworkCredential FTPCredentials;
        private DirectoryInfo directoryInfo;
        public FTPConnection(string FTPAddress,string username, string password,DebugLog debugLog)
        {
            debugLog.WriteToLog("Setting up settings needed for the FTP trasfer...", 7);
            FTPCredentials = new NetworkCredential(username, password);
            this.FTPAddress = FTPAddress;
            this.debugLog = debugLog;
        }

        public void UploadToFTP(string path)
        {
            debugLog.WriteToLog("Loading list of files and folders to copy...", 5);
            List<string>[] listToCopy = this.LoadListToCopy(path);
            debugLog.WriteToLog("Creating folder structure...", 5);
            foreach (string item in listToCopy[0])
            {
                try
                {
                    this.CreateDirectory(this.FTPAddress + item);
                }
                catch (Exception ex)
                {
                    debugLog.WriteToLog("Directory " + this.FTPAddress + item + " could not be created because of error " + ex.Message, 3);
                    throw;
                }
                
            }
            debugLog.WriteToLog("Transfering files...", 5);
            foreach (string item in listToCopy[1])
            {
                this.CopyFile(item, this.FTPAddress + item.Substring(0, path.Length));
            }

        }

        private void CreateDirectory(string path)
        {
            debugLog.WriteToLog("Creating folder " + path, 9);
            WebRequest request = WebRequest.Create(path);
            Stream ftpStream;
            request.Method = WebRequestMethods.Ftp.MakeDirectory;
            //request.UseBinary = true;
            request.Credentials = FTPCredentials;
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            ftpStream = response.GetResponseStream();
            debugLog.WriteToLog("FTP Folder creation completed with status " + response.StatusDescription, 9);
            ftpStream.Close();
            response.Close();
        }

        private void CopyFile(string pathSource, string pathDestination)
        {
            // Get the object used to communicate with the server.  
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(pathDestination);
            request.Method = WebRequestMethods.Ftp.UploadFile;

            // This example assumes the FTP site uses anonymous logon.  
            request.Credentials = new NetworkCredential(FTPCredentials.UserName, FTPCredentials.Password);

            // Copy the contents of the file to the request stream.  
            StreamReader sourceStream = new StreamReader(pathSource);
            byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
            sourceStream.Close();
            request.ContentLength = fileContents.Length;

            Stream requestStream = request.GetRequestStream();
            requestStream.Write(fileContents, 0, fileContents.Length);
            requestStream.Close();

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            debugLog.WriteToLog("FTP Upload completed with status " + response.StatusDescription, 9);

            response.Close();
        }

        private List<string>[] LoadListToCopy(string path)
        {
            debugLog.WriteToLog("Creating lists for storing files and folders structure...", 7);
            List<string>[] tmpArray = new List<string>[2];

            List<string> FileList = new List<string>();

            List<string> FolderList = new List<string>();
            debugLog.WriteToLog("Filling lists...", 7);
            directoryInfo = new DirectoryInfo(path);
            ExploreDirectoryRecursively(directoryInfo, FileList, FolderList);
            debugLog.WriteToLog("Reversing lists order for easier folder creation...", 7);
            FolderList.Reverse();
            FileList.Reverse();
            debugLog.WriteToLog("Setting references to created lists...", 7);
            tmpArray[0] = FolderList;
            tmpArray[1] = FileList;
            debugLog.WriteToLog("Firsts 3 folders are: " + FolderList[0] + ',' + FolderList[1] + ','+ FolderList[2], 8);
            debugLog.WriteToLog("Returning array of lists...", 7);
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
                    debugLog.WriteToLog("Cannot load folder " + item.FullName + " from temporary backup loaction for FTP transfer. File will be skipped!", 3);
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
                    debugLog.WriteToLog("Cannot load file " + item.FullName + " from temporary backup loaction for FTP transfer. Folder will be skipped!", 3);
                }

            }
        }
    }
}
