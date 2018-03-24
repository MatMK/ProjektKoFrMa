import { Component, OnInit } from '@angular/core';
import { Http } from '@angular/http';
import { ServerConnectionService } from '../server-connection/server-connection.service';
import { AppComponent } from '../../app.component';
import { Data } from '../server-connection/data.model';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  constructor(private serverConnectionService : ServerConnectionService, private data : Data) { }
  
  name : string;
  password : string;
  rootURL : string = 'http://localhost:49849/';


  LogIn()
  {
    if(!this.rootURL.endsWith('/'))
      this.rootURL += '/';
    this.data.ServerRootURL = this.rootURL;
    this.data.Loading = true;
    this.serverConnectionService.Login(this.password, this.name).then(res => {
      this.data.Loading = false;
      if (res != undefined)
      {
        this.data.adminInfo.Token = res;
        this.data.adminInfo.UserName = this.name;
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
