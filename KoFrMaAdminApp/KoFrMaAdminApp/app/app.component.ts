import { Component } from '@angular/core';
import { Http } from '@angular/http';

@Component({
  selector: 'my-app',
  template: `<h1 (click)="Login()">Welcome to {{token}}</h1>` , 
})
export class AppComponent
{
    name = 'KoFrMa-AdminApp';
    token: string = "";
    Login()
    {
        let url = "http://localhost:50576/api/AdminApp/RegisterToken";
        this.http.post(url, { "UserName": "Pepa", "Password": 123456 }).toPromise()
            .then(res => this.token = res.json())
            .catch(msg => console.log('Error: ' + msg.status + ' ' + msg.statusText))
    }
    constructor(private http: Http) {

    }
}
