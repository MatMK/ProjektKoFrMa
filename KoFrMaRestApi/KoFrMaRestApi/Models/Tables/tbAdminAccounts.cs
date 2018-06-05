using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.Tables
{
    /// <summary>
    /// Table with admin accounts
    /// </summary>
    public class tbAdminAccounts
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Username
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Is admin enabled
        /// </summary>
        public bool Enabled { get; set; }
        /// <summary>
        /// List of his permissions
        /// </summary>
        public List<int> Permission { get; set; }
    }
}