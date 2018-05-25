using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.Daemon.Task
{
    public class ScriptInfo
    {
        /// <summary>
        /// Path to script or programm that should be executed, can be exe, bat, cmd, ps1, vbs (or other formats, but that is not reccomanded)
        /// </summary>
        public string PathToLocalScript { get; set; }
        /// <summary>
        /// Format of the script that is saved here, can be bat, cmd, ps1 or vbs
        /// </summary>
        public string ScriptItselfFormat { get; set; }
        /// <summary>
        /// Script in text interpretation
        /// </summary>
        public string ScriptItself { get; set; }
    }
}