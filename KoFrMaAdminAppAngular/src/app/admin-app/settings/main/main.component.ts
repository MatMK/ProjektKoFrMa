import { Component, OnInit } from '@angular/core';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatListModule } from '@angular/material/list';
import { ServerConnectionService } from '../../server-connection/server-connection.service';
import { Router, RouterLinkActive, RouterLink } from '@angular/router';
import { Data } from '../../server-connection/data.model';

@Component({
  selector: 'app-main',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.css']
})
export class MainComponent {

  constructor(private service : ServerConnectionService, private router: Router, private data : Data) { 

    if(this.data.adminInfo.Token != null)
    {
      this.service.RefreshData([1,2,3]);
    }
  }
}
