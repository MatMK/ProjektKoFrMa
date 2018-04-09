import { Component, OnInit } from '@angular/core';
import { ServerConnectionService } from '../../../server-connection/server-connection.service';
import { tbTasks } from '../../../server-connection/models/sql-data/data/tb-tasks.model';
import { Data } from '../../../server-connection/data.model';
import { DataSource } from '@angular/cdk/table';
import { MatTableModule } from '@angular/material/table';
import { MatTableDataSource } from '@angular/material';
import { MainTask } from '../../../server-connection/models/communication-models/task/main-task.model';

@Component({
  selector: 'app-tasks',
  templateUrl: './tasks.component.html',
  styleUrls: ['./tasks.component.css']
})
export class TasksComponent implements OnInit {


  constructor(private service: ServerConnectionService, private data: Data) {
    this.refresh();
  }
  displayedColumns = ['Id', 'IdDaemon', 'TimeOfExecution', 'Completed'];
  
  refresh() {
    this.service.GettbTasks().then(res => this.data.Tasks = new MatTableDataSource<MainTask>(this.service.ConvertToMainTask(res)));
  }
  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // MatTableDataSource defaults to lowercase matches
    this.data.Tasks.filterPredicate = (data: MainTask, filter: string) => this.customFilter(data,filter);
    this.data.Tasks.filter = filterValue;
  }
  private customFilter(Data : MainTask, filter : string) : boolean
  {
    if(Data.Id.toString() == filter)
      return true;
    return false;
  }
  ngOnInit() {
  }
}
