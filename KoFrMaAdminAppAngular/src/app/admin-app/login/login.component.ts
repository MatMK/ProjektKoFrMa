import { Component, OnInit } from '@angular/core';
import { Http } from '@angular/http';
import { ServerConnectionService } from '../server-connection/server-connection.service';
import { AppComponent } from '../../app.component';
import { Data } from '../server-connection/data.module';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  constructor(private serverConnectionService : ServerConnectionService, private data : Data) {
   }
  LogIn(Password : string, Username : string)
  {
    alert()
    this.serverConnectionService.Login('123456','Pepa', 'http://localhost:49849/').then(res => {
      this.data.Token = res;
      if (this.data.Token != undefined)
      {
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
