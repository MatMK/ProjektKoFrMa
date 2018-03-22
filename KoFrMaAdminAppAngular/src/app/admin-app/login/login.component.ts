import { Component, OnInit } from '@angular/core';
import { Http } from '@angular/http';
import { ServerConnectionService } from '../server-connection/server-connection.service';
import { AppComponent } from '../../app.component';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  constructor(private serverConnectionService : ServerConnectionService, private appComponent : AppComponent) { }
  test()
  {
    this.appComponent.Token = this.serverConnectionService.Login('123','Karel')
    alert(this.appComponent.Token);
  }
  ngOnInit() {
  }
}
