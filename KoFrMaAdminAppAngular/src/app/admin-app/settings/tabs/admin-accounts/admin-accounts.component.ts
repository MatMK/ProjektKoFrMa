import { Component, OnInit, ViewChild } from '@angular/core';
import { ServerConnectionService } from '../../../server-connection/server-connection.service';
import { tbAdminAccounts } from '../../../server-connection/models/sql-data/data/tb-admin-accounts.model';
import { Data } from '../../../server-connection/data.model';
import { DataSource } from '@angular/cdk/table';
import { MatTableModule, MatTableDataSource, MatSelectModule} from '@angular/material';
import { VIEWPORT_RULER_PROVIDER } from '@angular/cdk/overlay';
import { ChangeTable } from '../../../server-connection/models/sql-data/change-table.model';
import { InputCheck } from '../../../server-connection/input-check.service';
import { FormControl } from '@angular/forms';
import { ChangePermission } from '../../../server-connection/models/sql-data/change-permission.model';
import { DeleteRowRequest } from '../../../server-connection/models/communication-models/post-admin/delete-row-request.model';

@Component({
  selector: 'app-admin-accounts',
  templateUrl: './admin-accounts.component.html',
  styleUrls: ['./admin-accounts.component.css']
})

export class AdminAccountsComponent {

  private check : InputCheck = new InputCheck();
  private selectedPerm : PermInterface[][];
  constructor(private service : ServerConnectionService, private data : Data) {
  }
  displayedColumns = ['Id', 'Username', 'Email', 'Enabled', 'Permission', 'Delete'];
  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // MatTableDataSource defaults to lowercase matches
    this.data.Data.tbAdminAccounts.filterPredicate = (data: tbAdminAccounts, filter: string) => this.customFilter(data,filter);
    this.data.Data.tbAdminAccounts.filter = filterValue;
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
    else if(columnName.toLowerCase() == 'email')
    {
      this.service.AlterDataEmail(new ChangeTable(id, value))
    }
    if(columnName.toLowerCase() == 'username' && !this.check.username(value))
    {
      let val = elem.getAttribute('prevVal');
      elem.value = val;
      return;
    }
    else if(columnName.toLowerCase() == 'username')
    {
      this.service.AlterDataUsername(new ChangeTable(id, value))
    }
    if(columnName.toLowerCase()== 'password' && !this.check.password(value))
    {
      let val = elem.getAttribute('prevVal');
      elem.value = val;
      return;
    }
    else if(columnName.toLowerCase() == 'password')
    {
      //this.service.AlterDataUsername(new ChangeTable(id, value))
      alert("unifinished");
    }
    
  }
  saveVal(elem : HTMLInputElement)
  {
    elem.setAttribute('prevVal',elem.value);
  }
  changePerm(Id)
  {
    let newPerm : number[];
    this.data.Data.tbAdminAccounts.data.forEach(element => {
      if(element.Id == Id)
      {
        newPerm = element.Permission;
      }
    });
    this.service.AlterDataPermissions(new ChangePermission(Id,newPerm));
  }
  changeEnabled(id, value)
  {
    this.service.AlterDataEnabled(new ChangeTable(id, value));
  }
  deleteRow(rowId)
  {
    this.service.DeleteRow(new DeleteRowRequest("DeleteRowRequest","tbAdminAccounts", rowId)).then(r => this.service.RefreshData([1]))
  }
}
