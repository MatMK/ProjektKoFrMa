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
import { ExistsRequest } from '../../../server-connection/models/communication-models/post-admin/exists-request.model';

@Component({
  selector: 'app-admin-accounts',
  templateUrl: './admin-accounts.component.html',
  styleUrls: ['./admin-accounts.component.css']
})

export class AdminAccountsComponent {
  
  disableSelect = new FormControl(false);
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
  alterEmail(value, id, elem : HTMLInputElement)
  {
    if(!this.check.email(value))
    {
      let val = elem.getAttribute('prevVal');
      elem.value = val;
      return;
    }
    this.service.AlterDataEmail(new ChangeTable(id, value)).catch(msg=>{
        let val = elem.getAttribute('prevVal');
        elem.value = val;
      });
  }
  alterUsername(value, id, elem : HTMLInputElement)
  {
    if(!this.check.username(value))
    {
      let val = elem.getAttribute('prevVal');
      elem.value = val;
      return;
    }
    this.service.Exists(new ExistsRequest("ExistsRequest", "tbAdminAccounts",value, "Username")).then(res=>
    {
      if(!res)
      {
      this.service.AlterDataUsername(new ChangeTable(id, value)).catch(res => 
        {
          alert(res)
          let val = elem.getAttribute('prevVal');
          elem.value = val;
        });
      }
      else
      {
        alert("Username already exists");
        let val = elem.getAttribute('prevVal');
        elem.value = val;
      }
    });
  }
  alterPassword(value, id, elem : HTMLInputElement)
  {
    if(!this.check.password(value))
    {
      let val = elem.getAttribute('prevVal');
      elem.value = val;
      return;
    }
    //this.service.AlterDataUsername(new ChangeTable(id, value))
    alert("unifinished");
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
    this.service.AlterDataPermissions(new ChangePermission(Id,newPerm)).catch(res => 
      {
        this.service.RefreshData([1])
      });
  }
  changeEnabled(id, value)
  {
    this.service.AlterDataEnabled(new ChangeTable(id, value)).catch(res => 
      {
        this.service.RefreshData([1])
      });
  }
  deleteRow(rowId)
  {
    this.service.DeleteRow(new DeleteRowRequest("DeleteRowRequest","tbAdminAccounts", rowId)).then(r => {
      this.service.RefreshData([1])
    }).catch(r=>{});
  }
}
