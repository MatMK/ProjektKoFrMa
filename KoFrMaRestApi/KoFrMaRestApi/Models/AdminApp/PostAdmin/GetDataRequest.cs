using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.AdminApp.PostAdmin
{
    public class GetDataRequest : IRequest
    {
        /// Number of data you want to retrieve from database
        /// 1 = <see cref="KoFrMaRestApi.Models.Tables.tbAdminAccounts"/>
        /// 2 = <see cref="KoFrMaRestApi.Models.Tables.tbDaemons"/>
        /// 3 = <see cref="KoFrMaRestApi.Models.Tables.tbTasks"/>
        /// 4 = <see cref="KoFrMaRestApi.Models.Tables.tbTasksCompleted"/>
        /// 5 = <see cref="KoFrMaRestApi.Models.Tables.tbServerExceptions"/>
        public List<int> getData { get; set; }
    }
}