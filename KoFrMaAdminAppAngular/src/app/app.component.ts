import { Component, OnInit } from '@angular/core';
import { Data } from './admin-app/server-connection/data.model';
import { AdminInfo } from './admin-app/server-connection/models/communication-models/admin-info.model';
import { ServerConnectionService } from './admin-app/server-connection/server-connection.service';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  constructor(private data : Data, private service : ServerConnectionService){ }
  private password : string;
  private targetUsername
  ngOnInit()
  {
    if(!(localStorage.getItem("adminInfo") === null || localStorage.getItem("url") === null))
    {
      this.data.adminInfo = JSON.parse(localStorage.getItem("adminInfo"));
      this.data.ServerRootURL = localStorage.getItem("url");
    }
  }
  changePasswordModal(Username : string)
  {
    this.data.ChangePassword = true;
    this.targetUsername = Username;
  }
  private cancel()
  {
    this.data.ChangePassword = false;
    this.password = undefined;
    this.targetUsername = undefined;
  }
  private ok()
  {
    if(this.password == undefined || this.password == null || this.password.trim().length < 6)
      alert("Password is too short, minimum size is characters")
    else
    {
      this.data.ChangePassword = false;
      if(this.targetUsername == undefined || this.targetUsername == null || this.targetUsername.trim() == "")
      {
        alert("Oops, something went wrong. Please try again");
      }
      this.service.UpdatePassword(this.password,this.targetUsername).then(res=>
        {
          this.targetUsername = undefined;
          this.password = undefined;
        });
    }
  }
  private title = 'app';
  public Token : string;
}