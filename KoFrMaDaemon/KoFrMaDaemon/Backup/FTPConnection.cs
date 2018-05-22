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
        private NetworkCredential FTPCredential;
        private DirectoryInfo directoryInfo;
        /// <summary>
        /// Creates new connection to FTP server
        /// </summary>
        /// <param name="FTPAddress">Address with subfolder to the FTP server</param>
        /// <param name="username">Username to the FTP server</param>
        /// <param name="password">Password to the FTP server</param>
        public FTPConnection(string FTPAddress,string username, string password)
        {
            ServiceKoFrMa.debugLog.WriteToLog("Setting up settings needed for the FTP trasfer...", 7);
            FTPCredential = new NetworkCredential(username, password);
            this.FTPAddress = FTPAddress;
        }
        /// <summary>
        /// Creates new connection to FTP server
        /// </summary>
        /// <param name="FTPAddress">Address with subfolder to the FTP server</param>
        /// <param name="networkCredential"><c>NetworkCredential</c> object that contains credentials to the FTP server</param>
        public FTPConnection(string FTPAddress, NetworkCredential networkCredential)
        {
            ServiceKoFrMa.debugLog.WriteToLog("Setting up settings needed for the FTP trasfer...", 7);
            this.FTPCredential = networkCredential;
            this.FTPAddress = FTPAddress;
        }
        /// <summary>
        /// Creates new connection to FTP server
        /// </summary>
        /// <param name="destinationPathFTP"><c>DestinationPathFTP</c> object containing all necessary values needed to connect</param>
        public FTPConnection(DestinationPathFTP destinationPathFTP)
        {
            ServiceKoFrMa.debugLog.WriteToLog("Setting up settings needed for the FTP trasfer...", 7);
            this.FTPCredential = destinationPathFTP.NetworkCredential;
            this.FTPAddress = destinationPathFTP.Path;
        }
        /// <summary>
        /// Uploads specified folder along with its subfolders and files to the FTP server
        /// </summary>
        /// <param name="path">Path to folder that will be uploaded</param>
        public void UploadToFTP(string path)
        {
            ServiceKoFrMa.debugLog.WriteToLog("Loading list of files and folders to copy...", 5);
            List<string>[] listToCopy = this.LoadListToCopy(path);
            ServiceKoFrMa.debugLog.WriteToLog("Creating folder structure...", 5);
            foreach (string item in listToCopy[0])
            {
                try
                {
                    CreateDirectory(this.FTPAddress + item);
                }
                catch (Exception ex)
                {
                    ServiceKoFrMa.debugLog.WriteToLog("Directory " + this.FTPAddress + item + " could not be created because of error " + ex.Message, 3);
                    throw;
                }
                
            }
            ServiceKoFrMa.debugLog.WriteToLog("Transfering files...", 5);
            foreach (string item in listToCopy[1])
            {
                this.CopyFile(item, this.FTPAddress + item.Substring(0, path.Length));
            }

        }

        private void CreateDirectory(string path)
        {
            ServiceKoFrMa.debugLog.WriteToLog("Creating folder " + path, 9);
            WebRequest request = WebRequest.Create(path);
            Stream ftpStream;
            request.Method = WebRequestMethods.Ftp.MakeDirectory;
            //request.UseBinary = true;
            request.Credentials = FTPCredential;
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            ftpStream = response.GetResponseStream();
            ServiceKoFrMa.debugLog.WriteToLog("FTP Folder creation completed with status " + response.StatusDescription, 9);
            ftpStream.Close();
            response.Close();
        }

        private void CopyFile(string pathSource, string pathDestination)
        {
            // Get the object used to communicate with the server.  
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(pathDestination);
            request.Method = WebRequestMethods.Ftp.UploadFile;

            // This example assumes the FTP site uses anonymous logon.  
            request.Credentials = new NetworkCredential(FTPCredential.UserName, FTPCredential.Password);

            // Copy the contents of the file to the request stream.  
            StreamReader sourceStream = new StreamReader(pathSource);
            byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
            sourceStream.Close();
            request.ContentLength = fileContents.Length;

            Stream requestStream = request.GetRequestStream();
            requestStream.Write(fileContents, 0, fileContents.Length);
            requestStream.Close();

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            ServiceKoFrMa.debugLog.WriteToLog("FTP Upload completed with status " + response.StatusDescription, 9);

            response.Close();
        }

        private List<string>[] LoadListToCopy(string path)
        {
            ServiceKoFrMa.debugLog.WriteToLog("Creating lists for storing files and folders structure...", 7);
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
            ServiceKoFrMa.debugLog.WriteToLog("Firsts 3 folders are: " + FolderList[0] + ',' + FolderList[1] + ','+ FolderList[2], 8);
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
