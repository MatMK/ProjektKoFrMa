using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.Daemon.Task
{
    public class SourceJournalLoadFromCache:ISource
    {
        /// <summary>
        /// ID Tasku na který se bude navazovat, pokud je soubor na 100% v cachi daemona
        /// </summary>
        public int JournalID { get; set; }
    }
}