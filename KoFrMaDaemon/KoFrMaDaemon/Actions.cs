using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace KoFrMaDaemon
{
    public class Actions
    {
        public string State;

        //const int dim1 = 5;
        //public string[,] poleStr = new string[dim1, dim1];

        public List<String[,]> SkippedFiles;

        public List<String> FilesCorrent;

        public List<String> SkippedFolders;

        public List<String> FoldersCorrent;

        public void FullBackupFolder(string source, string destination, bool createLog)
        {
            try
            {
                State = "Copying folders";
                this.CopyDirectoryRecursivly(new DirectoryInfo(source), new DirectoryInfo(destination), createLog);
            }
            catch (System.IO.IOException)
            {
                State = "Cannot copy!";
            }

            catch (System.UnauthorizedAccessException)
            {
                State = "Access denied!";
            }

            State = "";
        }

        private void CopyDirectoryRecursivly(DirectoryInfo from, DirectoryInfo to, bool createLog)
        {
            //Rekurzivní kopírovaní složky
            if (!to.Exists)
            {
                to.Create();
            }
                
                foreach (FileInfo item in from.GetFiles())
                {
                    try
                    {
                        item.CopyTo(to.FullName + @"\" + item.Name);
                    }
                    catch (Exception x)
                    {
                        this.SkippedFiles.Add(item.FullName,);
                    }
                    
                }

                foreach (DirectoryInfo item in from.GetDirectories())
                {
                    try
                    {
                        this.CopyDirectoryRecursivly(item, to.CreateSubdirectory(item.Name), createLog);
                    }
                    catch (Exception x)
                    {
                        this.SkippedFolders.Add(item.FullName);
                    }
                    
                }
        }
    }
}
