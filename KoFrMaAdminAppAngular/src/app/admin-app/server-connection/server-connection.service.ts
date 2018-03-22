import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { useAnimation } from '@angular/core/src/animation/dsl';

@Injectable()

export class ServerConnectionService{
    constructor( private http : Http)
    {
        
    }
    Login(Password : string, UserName : string) : string
    {
        let token = "";
        let url = "http://localhost:50576/api/AdminApp/RegisterToken";
        this.http.post(url, { "UserName": UserName, "Password": Password }).toPromise()
            .then(res => token = res.json())
            .catch(msg => console.log('Error: ' + msg.status + ' ' + msg.statusText))
        return token;        
    }
}
