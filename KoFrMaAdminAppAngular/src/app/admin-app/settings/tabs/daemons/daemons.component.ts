
import { Component, OnInit } from '@angular/core';
import { ServerConnectionService } from '../../../server-connection/server-connection.service';
import { tbDaemons } from '../../../server-connection/models/sql-data/data/tb-daemons.model';
import { Data } from '../../../server-connection/data.model';
import { DataSource } from '@angular/cdk/table';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { RouterLink } from '@angular/router'

@Component({
  selector: 'app-daemons',
  templateUrl: './daemons.component.html',
  styleUrls: ['./daemons.component.css']
})
export class DaemonsComponent implements OnInit {

  constructor(private service: ServerConnectionService, private data: Data) {
    this.refresh();
  }
  displayedColumns = ['Id', 'Version', 'OS', 'PC_Unique', 'Allowed', 'LastSeen', 'MoreInfo'];
  
  refresh() {
    this.service.GettbDaemons().then(res => this.data.Daemons = new MatTableDataSource<tbDaemons>(res));
  }
  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // MatTableDataSource defaults to lowercase matches
    this.data.Daemons.filterPredicate = (data: tbDaemons, filter: string) => this.customFilter(data,filter);
    this.data.Daemons.filter = filterValue;
  }
  private customFilter(Data : tbDaemons, filter : string) : boolean
  {
    if(Data.Id.toString() == filter)
      return true;
    return false;
  }
  ngOnInit() {
  }
}
