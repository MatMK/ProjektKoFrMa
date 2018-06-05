using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.Tables
{
    /// <summary>
    /// Table with exception on server
    /// </summary>
    public class tbServerExceptions
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Exception message
        /// </summary>
        public string ExceptionInJson { get; set; }
        /// <summary>
        /// Time when exception happened
        /// </summary>
        public DateTime TimeOfException { get; set; }
        /// <summary>
        /// Severity, if null it is unknown
        /// </summary>
        public int? Severity { get; set; }
    }
}