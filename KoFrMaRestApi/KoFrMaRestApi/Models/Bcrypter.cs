using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BCrypt.Net;

namespace KoFrMaRestApi.Models
{
    public class Bcrypter
    {
        private static readonly string HardcodedSalt = "i!S#F";
        public string BcryptPasswordInBase64(string Base64Password)
        {
            return Base64Encode(BCrypt.Net.BCrypt.HashPassword(Base64Password + HardcodedSalt, BCrypt.Net.BCrypt.GenerateSalt()));
        }
        public bool PasswordMatches(string Base64Password, string EncryptedPasswordInBase64)
        {
            
            bool t = BCrypt.Net.BCrypt.Verify(Base64Password + HardcodedSalt, Base64Decode(EncryptedPasswordInBase64));
            return t;
        }
        public string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        public string Base64Encode(dynamic plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}