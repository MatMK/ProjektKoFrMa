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
  dataSource = ADMIN_DATA;
  refresh()
  {
    this.service.GettbAdminAccounts().then(res => this.data.AdminAccounts = res);
  }
  ngOnInit() {
  }
}
export interface AdminTable {
  Id: number;
  Username: string;
  Email: string;
  Enabled: boolean;
  Permission: number;
}
const ADMIN_DATA: AdminTable[] = [
  {Id: 1, Username: 'Hydrogen', Email: '1.0079', Enabled: true, Permission: 1},
  {Id: 2, Username: 'Helium', Email: '4.0026', Enabled: true, Permission: 2},
  {Id: 3, Username: 'Lithium', Email: '6.941', Enabled: false, Permission: 3},
  {Id: 4, Username: 'Beryllium', Email: '9.0122', Enabled: false, Permission: 4}
];
