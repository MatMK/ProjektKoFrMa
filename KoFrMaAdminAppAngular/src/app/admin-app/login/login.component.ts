import { Component, OnInit } from '@angular/core';
import { Http } from '@angular/http';
import { ServerConnectionService } from '../server-connection/server-connection.service';
import { AppComponent } from '../../app.component';
import { Data } from '../server-connection/data.model';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  constructor(private serverConnectionService : ServerConnectionService, private data : Data) { }
  
  LogIn(Username : string, Password : string)
  {
    Username = 'Pepa';
    Password = '123456';
    this.data.ServerRootURL = 'http://localhost:49849/';
    this.data.Loading = true;
    this.serverConnectionService.Login(Password,Username).then(res => {
      this.data.Loading = false;
      if (res != undefined)
      {
        this.data.adminInfo.Token = res;
        this.data.adminInfo.UserName = Username;
        this.data.LoggedIn = true;
      }
      else
      {
        this.data.LoggedIn = false;
      }
    })
  }
  ngOnInit() {
  }
}
