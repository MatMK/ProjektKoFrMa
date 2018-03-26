import { Component, OnInit } from '@angular/core';
import { ServerConnectionService } from '../../../server-connection/server-connection.service';
import { tbAdminAccounts } from '../../../server-connection/models/sql-data/data/tb-admin-accounts.model';
import { Data } from '../../../server-connection/data.model';

@Component({
  selector: 'app-admin-accounts',
  templateUrl: './admin-accounts.component.html',
  styleUrls: ['./admin-accounts.component.css']
})
export class AdminAccountsComponent implements OnInit {

  constructor(private service : ServerConnectionService, private data : Data) { }
  refresh()
  {
    this.service.GettbAdminAccounts().then(res => this.data.AdminAccounts = res);
  }
  ngOnInit() {
  }
}
