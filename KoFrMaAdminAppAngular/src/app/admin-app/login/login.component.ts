import { Component, OnInit } from '@angular/core';
import { Http } from '@angular/http';
import { ServerConnectionService } from '../server-connection/server-connection.service';
import { AppComponent } from '../../app.component';
import { Data } from '../server-connection/data.model';
import { AdminLogin } from '../server-connection/models/communication-models/admin-login.model';
import { tbAdminAccounts } from '../server-connection/models/sql-data/data/tb-admin-accounts.model';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  constructor(private serverConnectionService : ServerConnectionService, private data : Data) {
   }
  tbAdmins : tbAdminAccounts[];
  LogIn(Username : string, Password : string)
  {
    this.data.ServerRootURL = 'http://localhost:49849/';
    this.data.Loading = true;
    this.serverConnectionService.Login('123456','Pepa').then(res => {
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
  test()
  {
    this.serverConnectionService.GetTb([1])
          .then(res => this.tbAdmins = res.AdminAccounts)
          .catch(msg => console.log(msg));

  }
  ngOnInit() {
  }
}
