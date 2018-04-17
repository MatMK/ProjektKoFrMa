using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Serialization;
using KoFrMaDaemon.Backup;

namespace KoFrMaDaemon.ConnectionToServer
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
