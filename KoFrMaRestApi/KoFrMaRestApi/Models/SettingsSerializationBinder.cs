using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KoFrMaRestApi.Models.AdminApp.PostAdmin;
using KoFrMaRestApi.Models.Daemon.Task;
using KoFrMaRestApi.Models.Daemon.Task.BackupJournal;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace KoFrMaRestApi.Models
{
    public class SettingsSerializationBinder : ISerializationBinder
    {
        public void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = null;
            typeName = serializedType.Name;
        }

        public Type BindToType(string assemblyName, string typeName)
        {
            switch (typeName)
            {
                //Admin
                case "AddAdminRequest":
                    return typeof(AddAdminRequest);
                case "GetDataRequest":
                    return typeof(GetDataRequest);
                case "SetTasksRequest":
                    return typeof(SetTasksRequest);
                case "ChangeTableRequest":
                    return typeof(ChangeTableRequest);
                case "ChangePermissionRequest":
                    return typeof(ChangePermissionRequest);
                case "DeleteRowRequest":
                    return typeof(DeleteRowRequest);
                case "ExistsRequest":
                    return typeof(ExistsRequest);
                case "ChangePasswordRequest":
                    return typeof(ChangePasswordRequest);
                case "EditEmailRequest":
                    return typeof(EditEmailRequest);
                case "GetTimerDaemonRequest":
                    return typeof(GetTimerDaemonRequest);


                //Task
                case "Destination7z":
                    return typeof(Destination7z);
                case "DestinationPlain":
                    return typeof(DestinationPlain);
                case "DestinationRar":
                    return typeof(DestinationRar);
                case "DestinationZip":
                    return typeof(DestinationZip);
                case "DestinationPathFTP":
                    return typeof(DestinationPathFTP);
                case "DestinationPathLocal":
                    return typeof(DestinationPathLocal);
                case "DestinationPathNetworkShare":
                    return typeof(DestinationPathNetworkShare);
                case "DestinationPathSFTP":
                    return typeof(DestinationPathSFTP);
                case "SourceFolders":
                    return typeof(SourceFolders);
                case "SourceJournalLoadFromCache":
                    return typeof(SourceJournalLoadFromCache);
                case "BackupJournalObject":
                    return typeof(BackupJournalObject);
                case "SourceMSSQL":
                    return typeof(SourceMSSQL);
                case "SourceMySQL":
                    return typeof(SourceMySQL);
            }
            return null;
        }
    }
}