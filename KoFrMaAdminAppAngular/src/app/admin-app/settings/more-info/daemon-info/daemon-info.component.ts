import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from "@angular/router";
import { MatRadioModule, MatRadioButton } from '@angular/material/radio';
import { SetTask } from '../../../server-connection/models/communication-models/task/set-task.model';
import { EventEmitter } from 'events';
import { TaskRepeating } from '../../../server-connection/models/communication-models/task/task-repeating.model';
import { ServerConnectionService } from '../../../server-connection/server-connection.service';

@Component({
  selector: 'app-daemon-info',
  templateUrl: './daemon-info.component.html',
  styleUrls: ['./daemon-info.component.css']
})
export class DaemonInfoComponent implements OnInit {

  private daemonId : number;
  private sourcepath : string;
  private destinationpath : string;
  private backuptype : string;
  private date : Date;


  constructor(private activeRoute:ActivatedRoute, private service : ServerConnectionService, private router : Router) {
    this.activeRoute.params.subscribe(params => {
      this.daemonId = params.daemonid;
      //check if it is a number
      if (!(this.daemonId >= 0 || this.daemonId<0))
      {
        this.daemonId = undefined;
        alert("Error, unsupported daemon type!")
      }

    });
   }
  AddTask()
  {
    let t : SetTask = new SetTask()
    t.DaemonId = this.daemonId;
    t.SourceOfBackup = this.sourcepath;
    t.WhereToBackup = [this.destinationpath];
    t.ExecutionTimes = new TaskRepeating();
    t.ExecutionTimes.ExecutionTimes = [this.date];
    t.TimeToBackup = this.date;
    this.service.SetTask([t]);
    this.router.navigate(['app','tasks']);
  }
  private onDateChange(value : Date)
  {
    this.date = value;
  }

  ngOnInit() {
  }

}
