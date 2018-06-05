using KoFrMaRestApi.Models;
using KoFrMaRestApi.Models.AdminApp;
using KoFrMaRestApi.Models.AdminApp.GetList;
using KoFrMaRestApi.Models.AdminApp.PostAdmin;
using KoFrMaRestApi.Models.Tables;
using KoFrMaRestApi.MySqlCom;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using Newtonsoft.Json;
using KoFrMaRestApi.Models.Daemon.Task;
using System.Net;
using BCrypt.Net;
using System.Net.Http;
using KoFrMaRestApi.Models.AdminApp.RepeatingTasks;
using System.Diagnostics;

namespace KoFrMaRestApi.Controllers
{
    /// <summary>
    /// Used for communication between AdminApp and RestApi
    /// AUTHENTICATION:
    /// Use valid token in <see cref="PostAdmin.adminInfo"/> retrievable from <see cref="RegisterToken(AdminLogin)"/>
    /// Http codes:
    /// <list type="bullet">
    /// <item>
    /// <description>400 Bad Request - invalid input</description>
    /// </item>
    /// <item>
    /// <description>401 Unauthorized - invalid token or username</description>
    /// </item>
    /// <item>
    /// <description>403 Forbidden - insufficient permission</description>
    /// </item>
    /// <item>
    /// <description>200 OK - OK</description>
    /// </item>
    /// <item>
    /// <description>500 Internal Server Error - Something when wrong on the server, check ServerExceptions table in AdminApp to see more details</description>
    /// </item>
    /// </list>
    /// Permission codes:
    /// 1 - Add Admins
    /// 2 - Add Tasks
    /// 3 - Change Table Data
    /// 4 - Change Permissions
    /// 5 - Change Passwords
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "*", exposedHeaders: "X-My-Header")]
    public class AdminAppController : ApiController
    {
        private string BadRequestCantDeserialize = "Server didn't recieve correct data, please try again.";
        private string BadRequestInvalidUsername = "Invalid username";
        private string BadRequestCannotSetPermission = "You don't have permission to set permissions";
        private string BadRequestUsernameExists = "This username already exists";
        InputCheck check = new InputCheck();
        MySqlAdmin mySqlCom = new MySqlAdmin();
        MySqlDaemon mySqlDaemon = new MySqlDaemon();
        /// <summary>
        /// Used for logging in
        /// </summary>
        /// <param name="adminLogin">Admin login credentials</param>
        /// <returns>Token usable only in <see cref="AdminAppController"/></returns>
        [HttpPost, Route(@"api/AdminApp/RegisterToken")]
        public string RegisterToken(AdminLogin adminLogin)
        {
            Debug.WriteLine(JsonConvert.SerializeObject(adminLogin));
            try
            {
                Debug.WriteLine(JsonConvert.SerializeObject(mySqlCom.RegisterToken(adminLogin)));
                return mySqlCom.RegisterToken(adminLogin);
            }
            catch(Exception ex)
            {
                if (ex.Message == "No admin with this username")
                { 
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
                }
                throw ex;
            }
        }
        /// <summary>
        /// Used for determining if an admin has permission
        /// </summary>
        /// <param name="adminInfo">Information about administrator</param>
        /// <returns>True if administrator has permission</returns>
        [HttpPost, Route(@"api/AdminApp/Permitted")]
        public bool Permitted(AdminInfo adminInfo)
        {
            int? AdminId = mySqlCom.GetAdminId(adminInfo.UserName);
            if (AdminId == null)
            {
                return false;
            }
            return mySqlCom.HasPermission((int)AdminId,adminInfo.Permission);
        }
        //Used for determining if an admin has permission
        //Returns true if administrator has permission
        private bool Permitted(string Username, int[] Permission)
        {
            int? AdminId = mySqlCom.GetAdminId(Username);
            if (AdminId == null)
            {
                return false;
            }
            return mySqlCom.HasPermission((int)AdminId, Permission);
        }
         /// <summary>
        /// Used for checking whether administrator is authorized
        /// </summary>
        /// <param name="adminInfo">Information about administrator</param>
        /// <returns></returns>
        [HttpPost, Route(@"api/AdminApp/Authorized")]
        public bool Authorized(AdminInfo adminInfo)
        {
            return mySqlCom.Authorized(adminInfo.UserName, adminInfo.Token);
        }
        /// <summary>
        /// Used for setting a new list of <see cref="Task"/>
        /// INPUT:
        /// <see cref="SetTasksRequest"/>
        /// AUTHENTICATION:
        /// Use valid token in <see cref="PostAdmin.adminInfo"/> retrievable from <see cref="RegisterToken(AdminLogin)"/>
        /// </summary>
        /// <param name="postAdmin">Information about administrator</param>
        [HttpPost, Route(@"api/AdminApp/SetTask")]
        public void SetTask(PostAdmin postAdmin)
        {
            if (this.Authorized(postAdmin.adminInfo))
            {
                var i = ((SetTasksRequest)postAdmin.request).setTasks;
                foreach (var item in i)
                {
                    if (item.Sources == null || item.ExecutionTimes== null || item.ExecutionTimes.ExecutionTimes == null || item.ExecutionTimes.ExecutionTimes.Count==0 || item.Destinations == null || item.DaemonId < 0 || item.Destinations.Count<1)
                    {
                        throw new HttpResponseException(HttpStatusCode.BadRequest);
                    }
                }
                mySqlCom.SetTasks(((SetTasksRequest)postAdmin.request).setTasks);
            }
            else
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }
        }
        /// <summary>
        /// Used for getting data from database
        /// AUTHENTICATION:
        /// Use valid token in <see cref="PostAdmin.adminInfo"/> retrievable from <see cref="RegisterToken(AdminLogin)"/>
        /// </summary>
        /// <param name="postAdmin">IRequest is <see cref="GetDataRequest"/>
        /// <para />Data you want to retrieve from SQL database are marked as nubmers in <see cref="GetDataRequest.getData"/>
        /// <para />1 = <see cref="tbAdminAccounts"/>
        /// <para />2 = <see cref="tbDaemons"/>
        /// <para />3 = <see cref="tbTasks"/>
        /// <para />4 = <see cref="tbTasksCompleted"/>
        /// <para />5 = <see cref="tbServerExceptions"/>
        /// </param>
        /// <returns>Returns selected data from database</returns>
        [HttpPost, Route(@"api/AdminApp/GetSqlData")]
        public Data GetSqlData(PostAdmin postAdmin)
        {
            if (this.Authorized(postAdmin.adminInfo))
            {
                Data d = new Data(((GetDataRequest) postAdmin.request).getData);
                return d;
            }
            else
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }
        }
        /// <summary>
        /// Changes permission of a user
        /// AUTHENTICATION:
        /// Use valid token in <see cref="PostAdmin.adminInfo"/> retrievable from <see cref="RegisterToken(AdminLogin)"/>
        /// Permissions 3 and 4 are required
        /// </summary>
        /// <param name="postAdmin">IRequest is <see cref="GetDataRequest"/></param>
        [HttpPost, Route(@"api/AdminApp/AlterDataPermissions")]
        public void AlterDataPermissions(PostAdmin postAdmin)
        {
            if (this.Authorized(postAdmin.adminInfo))
            {
                if (Permitted(postAdmin.adminInfo.UserName, new int[] { 3, 4 }))
                {
                    if (postAdmin.request is ChangePermissionRequest)
                        mySqlCom.AlterPermissions(((ChangePermissionRequest)postAdmin.request).changePermission);
                    else
                        throw new HttpResponseException(HttpStatusCode.BadRequest);
                }
                else
                    throw new HttpResponseException(HttpStatusCode.Forbidden);
            }
            else
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }
        /// <summary>
        /// Used for adding new administrators
        /// AUTHENTICATION:
        /// Use valid token in <see cref="PostAdmin.adminInfo"/> retrievable from <see cref="RegisterToken(AdminLogin)"/>
        /// Permission 4 is required if admin's permissions are being set
        /// </summary>
        /// <param name="postAdmin">IRequest is <see cref="AddAdminRequest"/></param>
        /// <returns>Returns <see cref="HttpStatusCode"/></returns>
        [HttpPost, Route(@"api/AdminApp/AddAdmin")]
        public HttpResponseMessage AddAdmin(PostAdmin postAdmin)
        {
            if (this.Authorized(postAdmin.adminInfo))
            {
                if (Permitted(postAdmin.adminInfo.UserName, new int[] { 1 }))
                {
                    if (postAdmin.request is AddAdminRequest)
                    {
                        if ((((AddAdminRequest)postAdmin.request).addAdmin.Permissions.Length != 0 && Permitted(postAdmin.adminInfo.UserName, new int[] { 4 })) || ((AddAdminRequest)postAdmin.request).addAdmin.Permissions.Length == 0)
                        {
                            mySqlCom.AddAdmin(((AddAdminRequest)postAdmin.request).addAdmin);
                        }
                        else
                            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, BadRequestCannotSetPermission);
                    }
                    else
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, BadRequestCantDeserialize);
                }
                else
                    throw new HttpResponseException(HttpStatusCode.Forbidden);
            }
            else
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        /// <summary>
        /// Deletes admin's token
        /// AUTHENTICATION:
        /// Use valid token in <see cref="PostAdmin.adminInfo"/> retrievable from <see cref="RegisterToken(AdminLogin)"/>
        /// </summary>
        /// <param name="admin">Information about administrator</param>
        [HttpPost, Route(@"api/AdminApp/LogOut")]
        public void LogOut(AdminInfo admin)
        {
            if (this.Authorized(admin))
            {
                int? id = mySqlCom.GetAdminId(admin.UserName);
                if (id != null)
                {
                    mySqlCom.LogOut((int)id);
                }
                else
                {
                    throw new Exception("No admin with such name");
                }
            }
        }
        /// <summary>
        /// Used for deleting rows in table tbAdminAccounts, tbDaemons or tbTasks
        /// AUTHENTICATION:
        /// Use valid token in <see cref="PostAdmin.adminInfo"/> retrievable from <see cref="RegisterToken(AdminLogin)"/>
        /// Permission 3 is required
        /// </summary>
        /// <param name="postAdmin">IRequest is <see cref="DeleteRowRequest"/></param>
        /// <returns></returns>
        [HttpPost, Route(@"api/AdminApp/DeleteRow")]
        public string DeleteRow(PostAdmin postAdmin)
        {
            if(this.Authorized(postAdmin.adminInfo) && (((DeleteRowRequest)postAdmin.request).TableName == "tbAdminAccounts"|| ((DeleteRowRequest)postAdmin.request).TableName == "tbDaemons" || ((DeleteRowRequest)postAdmin.request).TableName == "tbTasks"))
            {
                if (Permitted(postAdmin.adminInfo.UserName, new int[] { 3 }))
                {
                    this.mySqlCom.DeleteRow((DeleteRowRequest)postAdmin.request);
                    return null;
                }
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }
            else
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }
        /// <summary>
        /// Used for checking if username already exists.
        /// </summary>
        /// AUTHENTICATION:
        /// Use valid token in <see cref="PostAdmin.adminInfo"/> retrievable from <see cref="RegisterToken(AdminLogin)"/>
        /// <param name="postAdmin">IRequest is <see cref="ExistsRequest"/></param>
        /// <returns>True if username already exists</returns>
        [HttpPost, Route(@"api/AdminApp/Exists")]
        public bool Exists(PostAdmin postAdmin)
        {
            if (this.Authorized(postAdmin.adminInfo))
            {
                return this.mySqlCom.Exists((ExistsRequest)postAdmin.request);
            }
            else
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }
        /// <summary>
        /// Used for changing passwords
        /// AUTHENTICATION:
        /// Use valid token in <see cref="PostAdmin.adminInfo"/> retrievable from <see cref="RegisterToken(AdminLogin)"/>
        /// Permission 3 and 5 is required unless you are changing your own account's password
        /// </summary>
        /// <param name="postAdmin">IRequest is <see cref="ChangePasswordRequest"/></param>
        [HttpPost, Route(@"api/AdminApp/UpdatePassword")]
        public void UpdatePassword(PostAdmin postAdmin)
        {
            if (this.Authorized(postAdmin.adminInfo))
            {
                if (((ChangePasswordRequest)postAdmin.request).newPasswordInBase64.Length < 6)
                {
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
                }
                if (Permitted(postAdmin.adminInfo.UserName, new int[] { 3, 5 })||postAdmin.adminInfo.UserName == ((ChangePasswordRequest)postAdmin.request).targetUsername)
                {
                    mySqlCom.UpdatePassword(((ChangePasswordRequest)postAdmin.request).newPasswordInBase64, ((ChangePasswordRequest)postAdmin.request).targetUsername);
                }
                else
                    throw new HttpResponseException(HttpStatusCode.Forbidden);
            }
            else
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }
        /// <summary>
        /// Used for chaning data in sql table
        /// AUTHENTICATION:
        /// Use valid token in <see cref="PostAdmin.adminInfo"/> retrievable from <see cref="RegisterToken(AdminLogin)"/>
        /// Permission 3 is required
        /// Allowed columns
        /// <list type="bullet">
        /// <item>
        /// <description>Allowed - boolean</description>
        /// </item>
        /// <item>
        /// <description>IdDaemon - integer</description>
        /// </item>
        /// <item>
        /// <description>Enabled - boolean</description>
        /// </item>
        /// <item>
        /// <description>Email - string</description>
        /// </item>
        /// <item>
        /// <description>Username - string, has to be unique -> to check use <see cref="Exists(PostAdmin)"/></description>
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="postAdmin">IRequest is <see cref="ChangeTableRequest"/></param>
        [HttpPost, Route(@"api/AdminApp/AlterTable")]
        public void AlterTable(PostAdmin postAdmin)
        {
            if (postAdmin.request is ChangeTableRequest)
            {
                if (this.Authorized(postAdmin.adminInfo))
                {
                    if (Permitted(postAdmin.adminInfo.UserName, new int[] { 3 }))
                    {
                        if  (((ChangeTableRequest)postAdmin.request).changeTable.ColumnName == "Allowed" &&
                            ((ChangeTableRequest)postAdmin.request).changeTable.Value is bool)
                        {
                            mySqlCom.AlterTable(((ChangeTableRequest)postAdmin.request).changeTable);
                        }
                        else if (((ChangeTableRequest)postAdmin.request).changeTable.ColumnName == "IdDaemon" &&
                                ((ChangeTableRequest)postAdmin.request).changeTable.Value is int)
                        {
                            mySqlCom.AlterTable(((ChangeTableRequest)postAdmin.request).changeTable);
                        }
                        else if (((ChangeTableRequest)postAdmin.request).changeTable.ColumnName == "Enabled" &&
                                ((ChangeTableRequest)postAdmin.request).changeTable.Value is bool)
                        {
                            mySqlCom.AlterTable(((ChangeTableRequest)postAdmin.request).changeTable);
                        }
                        else if (((ChangeTableRequest)postAdmin.request).changeTable.ColumnName == "Email" &&
                                ((ChangeTableRequest)postAdmin.request).changeTable.Value is string)
                        {
                            mySqlCom.AlterTable(new ChangeTable() {ColumnName = "RecievingEmail", Id = (int)mySqlCom.GetAdminId(postAdmin.adminInfo.UserName),TableName = "tbEmailPreferences", Value = ((ChangeTableRequest)postAdmin.request).changeTable.Value}, "IdAdmin");
                            mySqlCom.AlterTable(((ChangeTableRequest)postAdmin.request).changeTable);
                        }
                        else if (((ChangeTableRequest)postAdmin.request).changeTable.ColumnName == "Username" &&
                                !mySqlCom.Exists(new ExistsRequest()
                                {
                                    Column = ((ChangeTableRequest)postAdmin.request).changeTable.ColumnName,
                                    TableName = ((ChangeTableRequest)postAdmin.request).changeTable.TableName,
                                    Value = ((ChangeTableRequest)postAdmin.request).changeTable.Value
                                }))
                        {
                            mySqlCom.AlterTable(((ChangeTableRequest)postAdmin.request).changeTable);
                        }
                        else if (((ChangeTableRequest)postAdmin.request).changeTable.ColumnName == "TimerTick" &&
                                (((ChangeTableRequest)postAdmin.request).changeTable.Value is long) || ((ChangeTableRequest)postAdmin.request).changeTable.Value is int)
                        {
                            mySqlCom.AlterTable(((ChangeTableRequest)postAdmin.request).changeTable);
                        }
                        else if (((ChangeTableRequest)postAdmin.request).changeTable.ColumnName == "TimerOnStart" &&
        (((ChangeTableRequest)postAdmin.request).changeTable.Value is long) || ((ChangeTableRequest)postAdmin.request).changeTable.Value is int)
                        {
                            mySqlCom.AlterTable(((ChangeTableRequest)postAdmin.request).changeTable);
                        }
                        else if (((ChangeTableRequest)postAdmin.request).changeTable.ColumnName == "TimerAfterFail" &&
        (((ChangeTableRequest)postAdmin.request).changeTable.Value is long) || ((ChangeTableRequest)postAdmin.request).changeTable.Value is int)
                        {
                            mySqlCom.AlterTable(((ChangeTableRequest)postAdmin.request).changeTable);
                        }
                        else
                        {
                            throw new HttpResponseException(HttpStatusCode.BadRequest);
                        }
                    }
                    else
                        throw new HttpResponseException(HttpStatusCode.Forbidden);
                }
                else
                    throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }
            else
                throw new HttpResponseException(HttpStatusCode.BadRequest);
        }
        /// <summary>
        /// Changes email settings
        /// </summary>
        /// <param name="postAdmin">PostAdmin with EditEmailRequest as IRequest</param>
        [HttpPost, Route("api/AdminApp/ChangeEmail")]
        public void ChangeEmail(PostAdmin postAdmin)
        {
            if (this.Authorized(postAdmin.adminInfo))
            {
                int? id = this.mySqlCom.GetAdminId(postAdmin.adminInfo.UserName);
                if (id == null)
                    throw new Exception("No admin with such name");
                this.mySqlCom.UpdateEmail((int)id,((EditEmailRequest)postAdmin.request));
            }
            else
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }
        /// <summary>
        /// Returns your email settings
        /// </summary>
        /// <param name="adminInfo"></param>
        /// <returns>Data about email</returns>
        [HttpPost, Route("api/AdminApp/GetEmail")]
        public EditEmailRequest GetEmail(AdminInfo adminInfo)
        {
            if (this.Authorized(adminInfo))
            {
                int? i = this.mySqlCom.GetAdminId(adminInfo.UserName);
                if (i == null)
                {
                    throw new Exception("No admin with such name");
                }
                return this.mySqlCom.GetEmailData((int)i);
            }
            else
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }
        /// <summary>
        /// Used for changing timer of daemon
        /// </summary>
        /// <param name="postAdmin"></param>
        /// <returns>Updated timer</returns>
        [HttpPost, Route("api/AdminApp/ChangeTimerDaemon")]
        public TimerTicks GetTimerDaemon(PostAdmin postAdmin)
        {
            if (this.Authorized(postAdmin.adminInfo))
            {
                if (Permitted(postAdmin.adminInfo.UserName, new int[] { 3 }))
                {
                    return mySqlCom.GetTimerTick(((GetTimerDaemonRequest)postAdmin.request).DaemonId);
                }
                else
                    throw new HttpResponseException(HttpStatusCode.Forbidden);
            }
            else
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }
    }
}
