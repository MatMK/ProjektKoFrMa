using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KoFrMaDaemon.ConnectionToServer
{
    /// <summary>
    /// Class that serializes and deserialies objects into/from JSON keeping the information about interfaces
    /// </summary>
    public static class JsonSerializationUtility
    {
        private static JsonSerializerSettings jsonSettings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto, SerializationBinder = new SettingsSerializationBinder() };
        /// <summary>
        /// Gets the JSON interpretation of object given
        /// </summary>
        /// <param name="obj">Any object that you want to serialize</param>
        /// <returns>JSON in string</returns>
        public static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, jsonSettings);
        }
        /// <summary>
        /// Returns the object that was serialized into the JSON
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="json">JSON</param>
        /// <returns>Object</returns>
        public static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, jsonSettings);
        }
    }
}
