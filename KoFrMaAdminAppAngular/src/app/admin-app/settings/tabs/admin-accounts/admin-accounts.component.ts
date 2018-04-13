import { Component, OnInit, ViewChild } from '@angular/core';
import { ServerConnectionService } from '../../../server-connection/server-connection.service';
import { tbAdminAccounts } from '../../../server-connection/models/sql-data/data/tb-admin-accounts.model';
import { Data } from '../../../server-connection/data.model';
import { DataSource } from '@angular/cdk/table';
import { MatTableModule, MatTableDataSource, MatSelectModule} from '@angular/material';
import { VIEWPORT_RULER_PROVIDER } from '@angular/cdk/overlay';
import { ChangeTable } from '../../../server-connection/models/sql-data/change-table.model';
import { InputCheck } from '../../../server-connection/input-check.service';

@Component({
  selector: 'app-admin-accounts',
  templateUrl: './admin-accounts.component.html',
  styleUrls: ['./admin-accounts.component.css']
})

export class AdminAccountsComponent {

  private check : InputCheck = new InputCheck();

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
  alterData(value, id, columnName:string, elem : HTMLInputElement)
  {
    if(columnName.toLowerCase() == 'email' && !this.check.email(value))
    {
      let val = elem.getAttribute('prevVal');
      elem.value = val;
      return;
    }
    if(columnName.toLowerCase() == 'username' && !this.check.username(value))
    {
      let val = elem.getAttribute('prevVal');
      elem.value = val;
      return;
    }
    if(columnName.toLowerCase()== 'password' && !this.check.password(value))
    {
      let val = elem.getAttribute('prevVal');
      elem.value = val;
      return;
    }
    if(columnName.toLowerCase()== 'enabled' && !this.check.isboolean(value))
    {
      let val = elem.getAttribute('prevVal');
      elem.value = val;
      return;
    }
    else if(columnName.toLowerCase()== 'enabled')
    {
      let res : boolean = value == "true"?true:false
      value = res;
    }
    let table : ChangeTable = new ChangeTable('tbAdminAccounts',id,columnName, value)
    this.service.AlterTable(table);
  }
  saveVal(elem : HTMLInputElement)
  {
    elem.setAttribute('prevVal',elem.value);
  }
}
