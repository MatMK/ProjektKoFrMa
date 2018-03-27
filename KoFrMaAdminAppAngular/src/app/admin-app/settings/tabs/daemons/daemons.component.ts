
import { Component, OnInit } from '@angular/core';
import { ServerConnectionService } from '../../../server-connection/server-connection.service';
import { tbDaemons } from '../../../server-connection/models/sql-data/data/tb-daemons.model';
import { Data } from '../../../server-connection/data.model';
import { DataSource } from '@angular/cdk/table';
import { MatTableModule } from '@angular/material/table';
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
    this.service.GettbDaemons().then(res => this.data.Daemons = res);
  }
  ngOnInit() {
  }
}
