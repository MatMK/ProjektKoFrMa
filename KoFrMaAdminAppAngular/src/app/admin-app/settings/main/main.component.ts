import { Component, OnInit } from '@angular/core';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatListModule } from '@angular/material/list';
import { ServerConnectionService } from '../../server-connection/server-connection.service';
@Component({
  selector: 'app-main',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.css']
})
export class MainComponent implements OnInit {

  constructor(private service : ServerConnectionService) { }
  test()
  {
    console.log(this.service.HashString("ahoj"));
  }
  ngOnInit() {
  }

}
