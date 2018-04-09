import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from "@angular/router";
import { MatRadioModule, MatRadioButton } from '@angular/material/radio';
import { SetTask } from '../../../../server-connection/models/communication-models/task/set-task.model';
import { EventEmitter } from 'events';
import { TaskRepeating } from '../../../../server-connection/models/communication-models/task/task-repeating.model';
import { ServerConnectionService } from '../../../../server-connection/server-connection.service';

@Component({
  selector: 'app-add-task',
  templateUrl: './add-task.component.html',
  styleUrls: ['./add-task.component.css']
})
export class AddTaskComponent implements OnInit {
  private daemonId : number;
  private sourcepath : string;
  private destinationpath : string;
  private backuptype : string;
  private date : Date;


  constructor(private activeRoute:ActivatedRoute, private service : ServerConnectionService, private router : Router) {
      this.activeRoute.params.subscribe(params => {
      this.daemonId = params.daemonid;
      this.checkIfNumberValid(false);
    });
   }
   checkIfNumberValid(showMsg : boolean) : boolean
   {
    if (!(this.daemonId >= 0 || this.daemonId<0))
    {
      this.daemonId = undefined;
      if(showMsg == true)
        alert("Daemon Id is not valid!")
      return false;
    }
    return true;
   }
  AddTask()
  {
    if(this.checkIfNumberValid(true))
    {
      let t : SetTask = new SetTask()
      t.DaemonId = this.daemonId;
      t.SourceOfBackup = this.sourcepath;
      t.WhereToBackup = [this.destinationpath];
      t.ExecutionTimes = new TaskRepeating();
      t.ExecutionTimes.ExecutionTimes = [this.date];
      t.TimeToBackup = this.date;
      this.service.SetTask([t]);
      this.router.navigate(['backup', 'app','tasks']);
    }
  }
  private onDateChange(value : Date)
  {
    this.date = value;
  }


  ngOnInit() {
    this.date = new Date(2018,1,1,0,0)
  }

}
