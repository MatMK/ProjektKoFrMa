import { Component, OnInit } from '@angular/core';
import { Data } from './admin-app/server-connection/data.model';
import { AdminInfo } from './admin-app/server-connection/models/communication-models/admin-info.model';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  constructor(private data : Data){}
  ngOnInit()
  {
    if(!(localStorage.getItem("adminInfo") === null || localStorage.getItem("url") === null))
    {
      this.data.adminInfo = JSON.parse(localStorage.getItem("adminInfo"));
      this.data.ServerRootURL = localStorage.getItem("url");
    }
  }
  private title = 'app';
  public Token : string;
}