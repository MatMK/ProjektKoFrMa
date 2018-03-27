import { Component, OnInit } from '@angular/core';
import { ServerConnectionService } from '../../../server-connection/server-connection.service';
import { tbAdminAccounts } from '../../../server-connection/models/sql-data/data/tb-admin-accounts.model';
import { Data } from '../../../server-connection/data.model';
import { DataSource } from '@angular/cdk/table';
import { MatTableModule } from '@angular/material/table';

@Component({
  selector: 'app-admin-accounts',
  templateUrl: './admin-accounts.component.html',
  styleUrls: ['./admin-accounts.component.css']
})
export class AdminAccountsComponent implements OnInit {

  constructor(private service : ServerConnectionService, private data : Data) { 
    this.refresh();
  }
  displayedColumns = ['Id', 'Username', 'Email', 'Enabled', 'Permission'];
  refresh()
  {
    this.service.GettbAdminAccounts().then(res => this.data.AdminAccounts = res);
  }
  ngOnInit() {
  }
}
