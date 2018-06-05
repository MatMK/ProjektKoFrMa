using KoFrMaRestApi.Models.Daemon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using KoFrMaRestApi.Models.AdminApp;
using KoFrMaRestApi.MySqlCom;
using KoFrMaRestApi.Models.AdminApp.PostAdmin;

namespace KoFrMaRestApi.Models
{
    /// <summary>
    /// Generating and checking tokens
    /// </summary>
    public class Token
    {
        /// <summary>
        /// Generates a new token with date encrypted to it
        /// </summary>
        /// <returns>New token</returns>
        public string GenerateToken()
        {
            byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            byte[] key = Guid.NewGuid().ToByteArray();
            string token = Convert.ToBase64String(time.Concat(key).ToArray());
            return token;
        }
        /// <summary>
        /// Checks if token is valid by time not by database
        /// </summary>
        /// <param name="token">token</param>
        /// <returns></returns>
        public bool IsValid(string token)
        {
            byte[] data = Convert.FromBase64String(token);
            DateTime when = DateTime.FromBinary(BitConverter.ToInt64(data, 0));
            if (when < DateTime.UtcNow.AddHours(-24))
            {
                return false;
            }
            return true;
        }
    }
}