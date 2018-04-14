using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KoFrMaRestApi.Models.AdminApp.PostAdmin;
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
            }
            return null;
        }
    }
}