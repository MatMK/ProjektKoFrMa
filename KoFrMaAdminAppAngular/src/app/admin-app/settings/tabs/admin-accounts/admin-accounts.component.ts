import { Component, OnInit, ViewChild } from '@angular/core';
import { ServerConnectionService } from '../../../server-connection/server-connection.service';
import { tbAdminAccounts } from '../../../server-connection/models/sql-data/data/tb-admin-accounts.model';
import { Data } from '../../../server-connection/data.model';
import { DataSource } from '@angular/cdk/table';
import { MatTableModule, MatTableDataSource, MatSelectModule} from '@angular/material';
import { VIEWPORT_RULER_PROVIDER } from '@angular/cdk/overlay';

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
    this.service.GettbAdminAccounts().then(res =>
      {
        this.data.AdminAccounts = new MatTableDataSource<tbAdminAccounts>(res)
      });
  }
  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // MatTableDataSource defaults to lowercase matches
    this.data.AdminAccounts.filterPredicate = (data: tbAdminAccounts, filter: string) => this.customFilter(data,filter);
    this.data.AdminAccounts.filter = filterValue;
  }
  private customFilter(Data : tbAdminAccounts, filter : string) : boolean
  {
    if(Data.Id.toString() == filter)
      return true;
    return false;
  }
  ngOnInit() {
  }
}
