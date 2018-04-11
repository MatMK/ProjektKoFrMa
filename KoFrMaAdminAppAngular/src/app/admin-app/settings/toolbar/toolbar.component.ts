import { Component, OnInit } from '@angular/core';
import { MatToolbarModule } from '@angular/material/toolbar';
import { Router } from '@angular/router';
import { ServerConnectionService } from '../../server-connection/server-connection.service';

@Component({
  selector: 'app-toolbar',
  templateUrl: './toolbar.component.html',
  styleUrls: ['./toolbar.component.css']
})
export class ToolbarComponent implements OnInit {

  constructor(private service : ServerConnectionService, private router : Router) { }
  logOut()
  {
    this.service.LogOut().then(res=>{
    this.router.navigate(["login"]);
    localStorage.clear();
    })
  }
  ngOnInit() {
  }

}
