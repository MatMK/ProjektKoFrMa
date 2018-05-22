import { Component, OnInit } from '@angular/core';
import { ServerConnectionService } from '../../../server-connection/server-connection.service';
import { Data } from '../../../server-connection/data.model';
import { tbAdminAccounts } from '../../../server-connection/models/sql-data/data/tb-admin-accounts.model';
import { MatTableDataSource } from '@angular/material';
import { tbCompletedTasks } from '../../../server-connection/models/sql-data/data/tb-completed-tasks';
import { Router } from '@angular/router';

@Component({
  selector: 'app-completed-tasks',
  templateUrl: './completed-tasks.component.html',
  styleUrls: ['./completed-tasks.component.css']
})
export class CompletedTasksComponent{

  constructor(private service : ServerConnectionService, private data : Data, router : Router) {
    //this.service.RefreshData([4]);
   }
  displayedColumns = ['id', 'idTask', 'idDaemon', 'timeOfCompletion', 'isSuccessful', 'moreInfo'];
}
