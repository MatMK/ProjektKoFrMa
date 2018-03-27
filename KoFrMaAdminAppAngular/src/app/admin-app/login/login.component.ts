import { Component, OnInit } from '@angular/core';
import { Http } from '@angular/http';
import { ServerConnectionService } from '../server-connection/server-connection.service';
import { AppComponent } from '../../app.component';
import { Data } from '../server-connection/data.model';
import { FormsModule } from '@angular/forms';
import {Router} from "@angular/router";

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  constructor(private serverConnectionService : ServerConnectionService, private data : Data, private router : Router) { }
  
  name : string;
  password : string;
  rootURL : string = 'http://localhost:49849/';
  LogIn()
  {
    if(!this.rootURL.endsWith('/'))
      this.rootURL += '/';
    this.data.ServerRootURL = this.rootURL;
    this.data.Loading = true;
    this.serverConnectionService.Login(btoa(this.password), this.name).then(res => {
      this.data.Loading = false;
      if (res != undefined)
      {
        this.data.adminInfo.Token = res;
        this.data.adminInfo.UserName = this.name;
        this.data.LoggedIn = true;
        this.router.navigate(['app']);
      }
      else
      {
        this.data.LoggedIn = false;
      }
      this.test();
    })
  }
  test()
  {
    this.serverConnectionService.GettbTasks().then(res=> 
      console.log(this.serverConnectionService.ConvertToMainTask(res)));
  }
  ngOnInit() {
  }
}
