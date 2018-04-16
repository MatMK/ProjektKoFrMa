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
  constructor(private data : Data, private service : ServerConnectionService){}
  ngOnInit()
  {
    if(!(localStorage.getItem("adminInfo") === null || localStorage.getItem("url") === null))
    {
      this.data.adminInfo = JSON.parse(localStorage.getItem("adminInfo"));
      this.data.ServerRootURL = localStorage.getItem("url");
    }
    if(this.data.adminInfo.Token != null)
    {
      this.service.RefreshData([1,2,3]);
    }
  }
  private title = 'app';
  public Token : string;
}