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
   GetTbAdmins() : tbAdminAccounts[]
   {
       let admin : tbAdminAccounts[];
       this.GetTb([1])
            .then(res => admin = res.AdminAccounts)
            .catch(msg => console.log(msg.status));;
       return admin;
   }
   GetTbDaemons() : tbDaemons[]
   {
       let daemons : tbDaemons[];
       this.GetTb([2])
            .then(res => daemons = res.Daemons)
            .catch(msg => console.log(msg.status));;
       return daemons;
   }
   GetTbTasks() : tbTasks[]
   {
       let tasks : tbTasks[];
       this.GetTb([3])
            .then(res => tasks = res.Tasks)
            .catch(msg => console.log(msg.status));

       return tasks;
   }
   GetTb(tables : number[]) : Promise<SqlData>
    {
        let postAdmin : PostAdmin = new PostAdmin(this.data.adminInfo, null, tables);
        let url = this.data.ServerRootURL + "api/AdminApp/GetList";
        return this.http.post(url, PostAdmin  ).toPromise()
                        .then(res => res.json())
                        .catch(msg => console.log('Error: ' + msg.status + ' ' + msg.statusText))
   }
}