using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models
{
    public class JsonSerializationUtility
    {
        private static JsonSerializerSettings jsonSettings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto, SerializationBinder = new SettingsSerializationBinder() };
        public static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, jsonSettings);
        }

        public static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, jsonSettings);
        }
    }
}