using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.Tables
{
    public class tbServerExceptions
    {
        public int Id { get; set; }
        public string ExceptionInJson { get; set; }
        public DateTime TimeOfException { get; set; }
        public int? Severity { get; set; }
    }
}