import { Injectable } from '@angular/core';
import { Http, Jsonp } from '@angular/http';
import { Data } from './data.model';
import { AdminLogin } from './models/communication-models/admin-login.model'
import { PostAdmin } from './models/communication-models/post-admin.model';
import { AdminInfo } from './models/communication-models/admin-info.model';
import { SqlData } from './models/sql-data/sql-data.model';
import { tbAdminAccounts } from './models/sql-data/data/tb-admin-accounts.model';
import { tbDaemons } from './models/sql-data/data/tb-daemons.model';
import { tbTasks } from './models/sql-data/data/tb-tasks.model';
import { MainTask } from './models/communication-models/task/main-task.model';
import { SetTask } from './models/communication-models/task/set-task.model';

@Injectable()

export class ServerConnectionService{
    constructor( private http : Http, private data : Data) { }

    Login(Password : string, Username : string): Promise<string> 
    {
        let adminLogin : AdminLogin = new AdminLogin();
        adminLogin.Password = Password;
        adminLogin.UserName = Username;
        let url = this.data.ServerRootURL + "api/AdminApp/RegisterToken";
        return this.http.post(url,adminLogin).toPromise()
                        .then(res => res.json())
                        .catch(msg => console.log('Error: ' + msg.status + ' ' + msg.statusText))
   }
   GettbAdminAccounts() : Promise<tbAdminAccounts[]>
   {
        let postAdmin : PostAdmin = new PostAdmin(this.data.adminInfo, [], [1]);
        let url = this.data.ServerRootURL + "api/AdminApp/GettbAdminAccounts";
        return this.http.post(url,postAdmin).toPromise()
                        .then(res => res.json())
                        .catch(msg => 
                            {
                                console.log('Error: ' + msg.status + ' ' + msg.statusText);
                            })
    }
    GetGettbDaemons() : Promise<tbDaemons[]>
   {
        let postAdmin : PostAdmin = new PostAdmin(this.data.adminInfo, [], [2]);
        let url = this.data.ServerRootURL + "api/AdminApp/GettbDaemons";
        return this.http.post(url,postAdmin).toPromise()
                        .then(res => res.json())
                        .catch(msg => 
                            {
                                console.log('Error: ' + msg.status + ' ' + msg.statusText);
                            })
    }
    GettbTasks() : Promise<tbTasks[]>
   {
        let postAdmin : PostAdmin = new PostAdmin(this.data.adminInfo, [], [3]);
        let url = this.data.ServerRootURL + "api/AdminApp/GettbTasks";
        return this.http.post(url,postAdmin).toPromise()
                        .then(res => res.json())
                        .catch(msg => 
                            {
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
        let postAdmin : PostAdmin = new PostAdmin(this.data.adminInfo,setTask, null);
        let url = this.data.ServerRootURL + "api/AdminApp/GettbTasks";
        this.http.post(url,postAdmin).toPromise()
                        .then(res => this.data.Loading = false)
                        .catch(msg => 
                            {
                                this.data.Loading = false;
                                console.log('Error: ' + msg.status + ' ' + msg.statusText);
                            })
    }
}