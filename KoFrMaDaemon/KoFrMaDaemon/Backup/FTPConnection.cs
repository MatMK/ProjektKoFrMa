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
        public FTPConnection(string FTPAddress,string username, string password,DebugLog debugLog)
        {
            FTPCredentials = new NetworkCredential(username, password);
            this.FTPAddress = FTPAddress;
            this.debugLog = debugLog;
        }

        public void UploadToFTP(string path)
        {


            List<string>[] listToCopy = this.LoadListToCopy(path);

            foreach (string item in listToCopy[0])
            {
                this.CreateDirectory(this.FTPAddress + @"/" + item);
            }

            foreach (string item in listToCopy[1])
            {
                this.CopyFile(this.FTPAddress + @"/" + item);
            }

        }

        private void CreateDirectory(string path)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(path);
            Stream ftpStream;
            request.Method = WebRequestMethods.Ftp.MakeDirectory;
            request.UseBinary = true;
            request.Credentials = FTPCredentials;
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            ftpStream = response.GetResponseStream();
            ftpStream.Close();
            response.Close();
        }

        private void CopyFile(string path)
        {
            // Get the object used to communicate with the server.  
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(FTPAddress);
            request.Method = WebRequestMethods.Ftp.UploadFile;

            // This example assumes the FTP site uses anonymous logon.  
            request.Credentials = new NetworkCredential(FTPCredentials.UserName, FTPCredentials.Password);

            // Copy the contents of the file to the request stream.  
            StreamReader sourceStream = new StreamReader("testfile.txt");
            byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
            sourceStream.Close();
            request.ContentLength = fileContents.Length;

            Stream requestStream = request.GetRequestStream();
            requestStream.Write(fileContents, 0, fileContents.Length);
            requestStream.Close();

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            debugLog.WriteToLog("FTP Upload completed with status " + response.StatusDescription, 5);

            response.Close();
        }

        private List<string>[] LoadListToCopy(string path)
        {
            List<string>[] tmpArray = new List<string>[2];

            DirectoryInfo directoryInfo = new DirectoryInfo(path);

            List<string> FileList = new List<string>();

            List<string> FolderList = new List<string>();

            ExploreDirectoryRecursively(directoryInfo, FileList, FolderList);

            tmpArray[0] = FolderList;
            tmpArray[1] = FileList;

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
                    debugLog.WriteToLog("Cannot load folder " + item.FullName + " from temporary backup loaction for FTP transfer. Folder will be skipped!", 3);
                }

            }

            foreach (DirectoryInfo item in path.GetDirectories())
            {
                try
                {
                    this.ExploreDirectoryRecursively(item, FileList, FolderList);
                    FolderList.Add(item.FullName);
                }
                catch (Exception)
                {
                    debugLog.WriteToLog("Cannot load file " + item.FullName + " from temporary backup loaction for FTP transfer. File will be skipped!", 3);
                }

            }
        }
    }
}
