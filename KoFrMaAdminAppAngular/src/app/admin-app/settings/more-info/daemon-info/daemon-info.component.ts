import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from "@angular/router";
import { MatRadioModule, MatRadioButton } from '@angular/material/radio';
import { SetTask } from '../../../server-connection/models/communication-models/task/set-task.model';
import { EventEmitter } from 'events';
import { TaskRepeatingNoTimespan } from '../../../server-connection/models/communication-models/task/task-repeating.model';
import { ServerConnectionService } from '../../../server-connection/server-connection.service';
import { Data } from '../../../server-connection/data.model';

@Component({
  selector: 'app-daemon-info',
  templateUrl: './daemon-info.component.html',
  styleUrls: ['./daemon-info.component.css']
})
export class DaemonInfoComponent implements OnInit {
  daemonId : number
  constructor(private activeRoute:ActivatedRoute, private service : ServerConnectionService, private router : Router, private data: Data)
  {
    this.activeRoute.params.subscribe(params => {
      this.daemonId = params.daemonid;
      this.checkIfNumberValid();
    })
  }
  displayedColumns = ['Id', 'IdDaemon', 'TimeOfExecution', 'Completed', 'Delete'];
  displayedColumnsCompletedTask = ['id', 'idTask', 'idDaemon', 'timeOfCompletion', 'isSuccessful', 'moreInfo'];
  checkIfNumberValid()
   {
    if (!(this.daemonId >= 0 || this.daemonId<0))
    {
      this.daemonId = undefined;
      alert("Error, unsupported daemon type!")
    this.router.navigate(['backup', 'app','daemons']);
    }
   }
  addTask()
  {
    this.router.navigate(['backup','add-task', this.daemonId]);
  }
  ngOnInit() {
  }

}
