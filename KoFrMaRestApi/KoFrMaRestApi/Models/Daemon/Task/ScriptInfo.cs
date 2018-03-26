using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.Daemon.Task
{
    public class ScriptInfo
    {
        /// <summary>
        /// Cesta ke scriptu nebo programu který se má spustit, může být exe, bat, cmd, ps1 nebo vbs
        /// </summary>
        public string PathToLocalScript { get; set; }
        /// <summary>
        /// Formát vloženého scriptu k úkolu, může být bat, cmd, ps1 nebo vbs
        /// </summary>
        public string ScriptItselfFormat { get; set; }
        /// <summary>
        /// Vložený script k úkolu
        /// </summary>
        public string ScriptItself { get; set; }
    }
}