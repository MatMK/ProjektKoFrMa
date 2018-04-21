import { Component, OnInit } from '@angular/core';
import { ServerConnectionService } from '../../../server-connection/server-connection.service';
import { tbTasks } from '../../../server-connection/models/sql-data/data/tb-tasks.model';
import { Data } from '../../../server-connection/data.model';
import { DataSource } from '@angular/cdk/table';
import { MatTableModule } from '@angular/material/table';
import { MatTableDataSource } from '@angular/material';
import { MainTask } from '../../../server-connection/models/communication-models/task/main-task.model';
import { DeleteRowRequest } from '../../../server-connection/models/communication-models/post-admin/delete-row-request.model';
import { ChangeTable } from '../../../server-connection/models/sql-data/change-table.model';

@Component({
  selector: 'app-tasks',
  templateUrl: './tasks.component.html',
  styleUrls: ['./tasks.component.css']
})
export class TasksComponent implements OnInit {


  constructor(private service: ServerConnectionService, private data: Data) {
  }
  displayedColumns = ['Id', 'IdDaemon', 'TimeOfExecution', 'Completed', 'Delete'];
  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // MatTableDataSource defaults to lowercase matches
    this.data.Data.tbTasks.filterPredicate = (data: MainTask, filter: string) => this.customFilter(data,filter);
    this.data.Data.tbTasks.filter = filterValue;
  }
  private customFilter(Data : MainTask, filter : string) : boolean
  {
    if(Data.Id.toString() == filter)
      return true;
    return false;
  }
  deleteRow(rowId)
  {
    this.service.DeleteRow(new DeleteRowRequest("DeleteRowRequest","tbTasks", rowId)).then(r => this.service.RefreshData([3]))
  }
  changeDaemonId(value, id)
  {
    this.service.AlterDataIdDaemon(new ChangeTable(id, value));
  }
  ngOnInit() {
  }
}
