using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoFrMaLocalDaemonConfig
{
    public class Bcrypt
    {
        private static readonly string HardcodedSalt = "i!S#F";
        public string BcryptPasswordInBase64(string Base64Password)
        {
            return Base64Encode(BCrypt.Net.BCrypt.HashPassword(Base64Password + HardcodedSalt, BCrypt.Net.BCrypt.GenerateSalt()));
        }
        public string Base64Encode(dynamic plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}
