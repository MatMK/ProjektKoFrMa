
import { Component, OnInit } from '@angular/core';
import { ServerConnectionService } from '../../../server-connection/server-connection.service';
import { tbDaemons } from '../../../server-connection/models/sql-data/data/tb-daemons.model';
import { Data } from '../../../server-connection/data.model';
import { DataSource } from '@angular/cdk/table';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { RouterLink } from '@angular/router'
import { ChangeTable } from '../../../server-connection/models/sql-data/change-table.model';
import { InputCheck } from '../../../server-connection/input-check.service';

@Component({
  selector: 'app-daemons',
  templateUrl: './daemons.component.html',
  styleUrls: ['./daemons.component.css']
})
export class DaemonsComponent implements OnInit {

  private check : InputCheck
  constructor(private service: ServerConnectionService, private data: Data) {
  }
  displayedColumns = ['Id', 'Version', 'OS', 'PC_Unique', 'Allowed', 'LastSeen', 'MoreInfo'];
  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // MatTableDataSource defaults to lowercase matches
    this.data.Data.tbDaemons.filterPredicate = (data: tbDaemons, filter: string) => this.customFilter(data,filter);
    this.data.Data.tbDaemons.filter = filterValue;
  }
  private customFilter(Data : tbDaemons, filter : string) : boolean
  {
    if(Data.Id.toString() == filter)
      return true;
    return false;
  }
  saveVal(elem : HTMLInputElement)
  {
    elem.setAttribute('prevVal',elem.value);
  }
  changeEnabled(id, value)
  {
    this.service.AlterDataAllowed(new ChangeTable(id,value)).catch(msg=>{
      this.data.Data.tbDaemons.data.forEach(element => {
        if(element.Id == id)
          element.Allowed = !element.Allowed;
      });
    })
  }
  ngOnInit() {
  }
}
