import { Injectable } from '@angular/core';
import { Http, Jsonp } from '@angular/http';
import { Data } from './data.model';
import { AdminLogin } from './models/communication-models/admin-login.model'
import { PostAdmin } from './models/communication-models/post-admin/post-admin.model';
import { AdminInfo } from './models/communication-models/admin-info.model';
import { SqlData } from './models/sql-data/sql-data.model';
import { tbAdminAccounts } from './models/sql-data/data/tb-admin-accounts.model';
import { tbDaemons } from './models/sql-data/data/tb-daemons.model';
import { tbTasks } from './models/sql-data/data/tb-tasks.model';
import { MainTask } from './models/communication-models/task/main-task.model';
import { SetTask } from './models/communication-models/task/set-task.model';
import { AddAdmin } from './models/communication-models/add-admin.model';
import { GetDataRequest } from './models/communication-models/post-admin/get-data-request.model';
import { SetTasksRequest } from './models/communication-models/post-admin/set-tasks-request.model';
import { AddAdminRequest } from './models/communication-models/post-admin/add-admin-request.model';
import { ChangeTable } from './models/sql-data/change-table.model';

@Injectable()

export class ServerConnectionService{
    constructor( private http : Http, private data : Data) { }

    Login(Password : string, Username : string): Promise<string> 
    {
        this.data.Loading = true;
        let adminLogin : AdminLogin = new AdminLogin();
        adminLogin.Password = Password;
        adminLogin.UserName = Username;
        let url = this.data.ServerRootURL + "api/AdminApp/RegisterToken";
        return this.http.post(url,adminLogin).toPromise()
                        .then(res => {
                            this.data.Loading = false;
                            return res.json()
                        })
                        .catch(msg => {
                            this.data.Loading = false;
                            console.log('Error: ' + msg.status + ' ' + msg.statusText)
                        })
   }
   GettbAdminAccounts() : Promise<tbAdminAccounts[]>
   {
        this.data.Loading = true;
        let postAdmin : PostAdmin = new PostAdmin(this.data.adminInfo,new GetDataRequest("GetDataRequest",[1]));
        let url = this.data.ServerRootURL + "api/AdminApp/GettbAdminAccounts";
        return this.http.post(url,postAdmin).toPromise()
                        .then(res => {
                            this.data.Loading = false;
                            return res.json()
                        })
                        .catch(msg => 
                            {
                                this.data.Loading = false;
                                console.log('Error: ' + msg.status + ' ' + msg.statusText);
                            })
    }
    GettbDaemons() : Promise<tbDaemons[]>
   {
        this.data.Loading = true;
        let postAdmin : PostAdmin = new PostAdmin(this.data.adminInfo, new GetDataRequest("GetDataRequest",[2]));
        let url = this.data.ServerRootURL + "api/AdminApp/GettbDaemons";
        return this.http.post(url,postAdmin).toPromise()
                        .then(res =>{
                            this.data.Loading = false;
                            return res.json();
                        })
                        .catch(msg => 
                            {
                                this.data.Loading = false;
                                console.log('Error: ' + msg.status + ' ' + msg.statusText);
                            })
    }
    GettbTasks() : Promise<tbTasks[]>
   {
        this.data.Loading = true;
        let postAdmin : PostAdmin = new PostAdmin(this.data.adminInfo, new GetDataRequest("GetDataRequest",[3]));
        let url = this.data.ServerRootURL + "api/AdminApp/GettbTasks";
        return this.http.post(url,postAdmin).toPromise()
                        .then(res => {
                            this.data.Loading = false;
                            return res.json()
                        })
                        .catch(msg => 
                            {
                                this.data.Loading = false;
                                console.log('Error: ' + msg.status + ' ' + msg.statusText);
                            })
    }
    ConvertToMainTask(tbTask : tbTasks[]) : MainTask[]
    {
        let mainTask : MainTask[] = [];
        tbTask.forEach(item => {
            let tmp = new MainTask();
            tmp.Id = item.Id;
            tmp.IdDaemon = item.IdDaemon;
            tmp.TimeOfExecution = item.TimeOfExecution;
            tmp.Completed = item.Completed;
            tmp.Task = JSON.parse(item.Task)
            tmp.RepeatInJSON = JSON.parse(item.RepeatInJSON);
            mainTask.push(tmp);
        });
        return mainTask;
    }
    SetTask(setTask:SetTask[]) 
    {
        this.data.Loading = true;
        let postAdmin : PostAdmin = new PostAdmin(this.data.adminInfo,new SetTasksRequest("SetTasksRequest",setTask));
        let url = this.data.ServerRootURL + "api/AdminApp/SetTask";
        this.http.post(url,postAdmin).toPromise()
                        .then(res => this.data.Loading = false)
                        .catch(msg => 
                            {
                                this.data.Loading = false;
                                console.log('Error: ' + msg.status + ' ' + msg.statusText);
                            })
    }
    HashString(toHash : string) : string
    {
        //unfinished
        return toHash;
    }
    IsAuthorized() : Promise<boolean>
    {
        this.data.Loading = true;
        let url = this.data.ServerRootURL + "api/AdminApp/Authorized";
        return this.http.post(url,this.data.adminInfo).toPromise()
            .then(res => {
                this.data.Loading = false;
                return res.json()
            })
            .catch(msg => 
                {
                    this.data.Loading = false;
                    console.log('Error: ' + msg.status + ' ' + msg.statusText);
                })
    }
    HasPermission(perm : number[]) : Promise<boolean>
    {
        this.data.Loading = true;
        let url = this.data.ServerRootURL + "api/AdminApp/Permitted";
        let temp : AdminInfo = this.data.adminInfo;
        temp.Permission = perm;
        return this.http.post(url,temp).toPromise()
            .then(res => {
                this.data.Loading = false;
                return res.json();
            })
            .catch(msg => 
                {
                    this.data.Loading = false;
                    console.log('Error: ' + msg.status + ' ' + msg.statusText);
                })
    }
    AddAdmin(addAdmin:AddAdmin) : Promise<boolean>
    {
        this.data.Loading = true;
        let postAdmin : PostAdmin = new PostAdmin(this.data.adminInfo,new AddAdminRequest("AddAdminRequest",addAdmin));
        let url = this.data.ServerRootURL + "api/AdminApp/AddAdmin";
        return this.http.post(url,postAdmin).toPromise()
                        .then(res => 
                            {
                                this.data.Loading = false
                                return true;
                            })
                        .catch(msg => 
                            {
                                this.data.Loading = false;
                                console.log('Error: ' + msg.status + ' ' + msg.statusText);
                                return false;
                            })
    }
    NumberToPermission(permissions : number[]) : string[]
    {  
        let result : string[] = [];
        if(permissions.length == 0)
        {
            return ["No permissions"];
        }
        permissions.forEach(element => {
            this.data.Permissions.forEach(number => {
                if(number.number == element)
                {
                    result.push(" " + number.name);
                }
            });
        });
        return result;
    }
    LogOut()
    {
        this.data.Loading = true;
        let url = this.data.ServerRootURL + "api/AdminApp/LogOut";
        return this.http.post(url,this.data.adminInfo).toPromise()
                        .then(res => 
                            {
                                this.data.Loading = false
                            })
                        .catch(msg => 
                            {
                                this.data.Loading = false;
                                console.log('Error: ' + msg.status + ' ' + msg.statusText);
                            })
    }
    AlterTable(table : ChangeTable)
    {
        this.data.Loading = true;
        let url = this.data.ServerRootURL + "api/AdminApp/AlterData";
        return this.http.post(url,table).toPromise()
                        .then(res => 
                            {
                                this.data.Loading = false
                                return true;
                            })
                        .catch(msg => 
                            {
                                this.data.Loading = false;
                                console.log('Error: ' + msg.status + ' ' + msg.statusText);
                                return false;
                            })
    }
}