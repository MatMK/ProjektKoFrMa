import { Component, OnInit } from '@angular/core';
import { ServerConnectionService } from '../../../server-connection/server-connection.service';
import { tbTasks } from '../../../server-connection/models/sql-data/data/tb-tasks.model';
import { Data } from '../../../server-connection/data.model';
import { DataSource } from '@angular/cdk/table';
import { MatTableModule } from '@angular/material/table';

@Component({
  selector: 'app-tasks',
  templateUrl: './tasks.component.html',
  styleUrls: ['./tasks.component.css']
})
export class TasksComponent implements OnInit {


  constructor(private service: ServerConnectionService, private data: Data) {
    this.refresh();
  }
  displayedColumns = ['Id', 'IDaemon', 'Task', 'TimeOfExecution', 'RepeatInJSON', 'Completed'];
  
  refresh() {
    this.service.GettbTasks().then(res => this.data.Tasks = res);
  }
  ngOnInit() {
  }
}
export interface TaskTable {
   Id: number;
   IdDaemon: number;
   Task: string;
   TimeOfExecution: Date;
   RepeatInJSON: string;
  Completed: boolean;
}
