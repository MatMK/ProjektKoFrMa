import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { Data } from './data.model';

@Injectable()

export class ServerConnectionService{
    constructor( private http : Http, private data : Data) { }

    Login(Password : string, Username : string, ServerRootURL : string): Promise<string> {
        let url = ServerRootURL + "api/AdminApp/RegisterToken";
        return this.http.post(url, { "Username": Username, "Password": Password }).toPromise()
                        .then(res => res.json())
                        .catch(msg => console.log('Error: ' + msg.status + ' ' + msg.statusText))
   }
   GetTbAdmins()
   {

   }
   GetTbDaemons()
   {

   }
   getTbTasks()
   {

   }
}