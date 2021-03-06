﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoFrMaDaemon.ConnectionToServer
{
    public class TaskVersion
    {
        /// <summary>
        /// ID of the task
        /// </summary>
        public int TaskID { get; set; }
        /// <summary>
        /// Hash of task data for checking if the task has been changed since the server sent it
        /// </summary>
        public int TaskDataHash { get; set; }
    }
}
