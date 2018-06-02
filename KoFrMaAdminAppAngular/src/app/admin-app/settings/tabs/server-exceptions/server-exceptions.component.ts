import { Component, OnInit } from '@angular/core';
import { ServerConnectionService } from '../../../server-connection/server-connection.service';
import { Data } from '../../../server-connection/data.model';
import { MatTableDataSource } from '@angular/material';
import { tbCompletedTasks } from '../../../server-connection/models/sql-data/data/tb-completed-tasks';
import { tbServerExceptions } from '../../../server-connection/models/sql-data/data/tb-server-exceptions.model';

@Component({
  selector: 'app-server-exceptions',
  templateUrl: './server-exceptions.component.html',
  styleUrls: ['./server-exceptions.component.css']
})
export class ServerExceptionsComponent {

  constructor(private service : ServerConnectionService, private data : Data) {
    //this.service.RefreshData([5])
   }
   displayedColumns = ['id', 'timeOfException', 'exceptionInJson', 'severity'];

  private severityDisplay(severity : number) : string
  {
    if(severity == undefined && severity == null)
      return "Unknown";
    return severity.toString();
  }
  exceptionMessage(exceptionJson : string) : string
  {
    return JSON.parse(exceptionJson).Message;
  }
}
