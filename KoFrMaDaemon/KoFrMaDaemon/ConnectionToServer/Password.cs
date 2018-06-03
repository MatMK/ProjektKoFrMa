using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoFrMaDaemon.ConnectionToServer
{
    public class Password
    {
        private Password() { }
        private static readonly Password instance = new Password();
        /// <summary>
        /// Returns current password
        /// </summary>
        public static Password Instance
        {
            get
            {
                return instance;
            }
        }
        /// <summary>
        /// Sets password for the daemon
        /// </summary>
        /// <param name="DaemonPassword">Password</param>
        public void SetPassword(string DaemonPassword)
        {
            password = DaemonPassword;
        }
        //set password by using SetPassword() funciton not directly
        public string password { get; set; }
        public DaemonInfo daemon { get; set; }
    }

}
