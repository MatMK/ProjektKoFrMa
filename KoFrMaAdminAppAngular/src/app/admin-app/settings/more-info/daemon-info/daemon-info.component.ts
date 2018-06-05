import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from "@angular/router";
import { MatRadioModule, MatRadioButton } from '@angular/material/radio';
import { SetTask } from '../../../server-connection/models/communication-models/task/set-task.model';
import { EventEmitter } from 'events';
import { TaskRepeatingNoTimespan } from '../../../server-connection/models/communication-models/task/task-repeating.model';
import { ServerConnectionService } from '../../../server-connection/server-connection.service';
import { Data } from '../../../server-connection/data.model';
import { MainTask } from '../../../server-connection/models/communication-models/task/main-task.model';
import { DeleteRowRequest } from '../../../server-connection/models/communication-models/post-admin/delete-row-request.model';
import { tbCompletedTasks } from '../../../server-connection/models/sql-data/data/tb-completed-tasks';
import { ChangeTable } from '../../../server-connection/models/sql-data/change-table.model';

@Component({
  selector: 'app-daemon-info',
  templateUrl: './daemon-info.component.html',
  styleUrls: ['./daemon-info.component.css']
})
export class DaemonInfoComponent implements OnInit {
  daemonId : number;
  timertickValue : string;
  repeat : number = 60;
  timertickValueError : string;
  repeatError : number = 60;
  timertickValueStart : string;
  repeatStart : number = 60;
  private repeatOptions : {value : number, text : string}[] = [
    {value: 1 , text:"Seconds"},
    {value: 60 , text:"Minutes"},
    {value: 3600 , text:"Hours"},
    {value: 86400, text:"Days"},
    {value: 604800 , text:"Weeks"},
  ]

  constructor(private activeRoute:ActivatedRoute, private service : ServerConnectionService, private router : Router, private data: Data)
  {
    
      this.activeRoute.params.subscribe(params => {
        this.daemonId = params.daemonid;
        this.checkIfNumberValid();
        this.service.RefreshData([2,3,4]).then(res=>{
          this.data.Data.tbTasks.filterPredicate = (data: MainTask, filter: string) => data.IdDaemon.toString() == filter;
          this.data.Data.tbTasks.filter = this.daemonId.toString();
          this.data.Data.tbCompletedTasks.filterPredicate = (data: tbCompletedTasks, filter: string) => data.IdDaemon.toString() == filter;
          this.data.Data.tbCompletedTasks.filter = this.daemonId.toString();
          this.service.GetTimerTick(this.daemonId).then(res=>
          {
            if(res.AfterFailed > 604800 && res.AfterFailed%604800==0)
            {
              this.repeatError = 604800
              this.timertickValueError = (res.AfterFailed/604800).toString();
            }
            else if(res.AfterFailed > 86400 && res.AfterFailed%86400==0)
            {
              this.repeatError = 86400
              this.timertickValueError = (res.AfterFailed/86400).toString();
            }
            else if(res.AfterFailed > 3600 && res.AfterFailed%3600==0)
            {
              this.repeatError = 3600
              this.timertickValueError = (res.AfterFailed/3600).toString();
            }
            else if(res.AfterFailed > 60 && res.AfterFailed%60==0)
            {
              this.repeatError = 60
              this.timertickValueError = (res.AfterFailed/60).toString();
            }
            else
            {
              this.repeatError = 1
              this.timertickValueError = res.AfterFailed.toString();
            }

            if(res.AfterStart > 604800 && res.AfterStart%604800==0)
            {
              this.repeatStart= 604800
              this.timertickValueStart= (res.AfterStart/604800).toString();
            }
            else if(res.AfterStart > 86400 && res.AfterStart%86400==0)
            {
              this.repeatStart= 86400
              this.timertickValueStart= (res.AfterStart/86400).toString();
            }
            else if(res.AfterStart > 3600 && res.AfterStart%3600==0)
            {
              this.repeatStart= 3600
              this.timertickValueStart= (res.AfterStart/3600).toString();
            }
            else if(res.AfterStart > 60 && res.AfterStart%60==0)
            {
              this.repeatStart= 60
              this.timertickValueStart= (res.AfterStart/60).toString();
            }
            else
            {
              this.repeatStart= 1
              this.timertickValueStart= res.AfterStart.toString();
            }

            if(res.TimerTick > 604800 && res.TimerTick%604800==0)
            {
              this.repeat = 604800
              this.timertickValue = (res.TimerTick/604800).toString();
            }
            else if(res.TimerTick > 86400 && res.TimerTick%86400==0)
            {
              this.repeat = 86400
              this.timertickValue = (res.TimerTick/86400).toString();
            }
            else if(res.TimerTick > 3600 && res.TimerTick%3600==0)
            {
              this.repeat = 3600
              this.timertickValue = (res.TimerTick/3600).toString();
            }
            else if(res.TimerTick > 60 && res.TimerTick%60==0)
            {
              this.repeat = 60
              this.timertickValue = (res.TimerTick/60).toString();
            }
            else
            {
              this.repeat = 1
              this.timertickValue = res.TimerTick.toString();
            }
          });
        })
    });
  }
  displayedColumns = ['Id', 'TimeOfExecution', 'Completed', 'Delete'];
  displayedColumnsCompletedTask = ['idTask', 'timeOfCompletion', 'isSuccessful', 'moreInfo'];
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
  deleteRow(rowId)
  {
    this.service.DeleteRow(new DeleteRowRequest("DeleteRowRequest","tbTasks", rowId)).then(r => {
      this.service.RefreshData([3])
    }).catch(r=>{});
  }
  ngOnInit() {
  }
  changeTick()
  {
    this.service.AlterTable(new ChangeTable("tbDaemons", this.daemonId, "TimerTick", parseInt(this.timertickValue)*this.repeat))
  }
  changeTickStart()
  {
    this.service.AlterTable(new ChangeTable("tbDaemons", this.daemonId, "TimerOnStart", parseInt(this.timertickValueStart)*this.repeatStart))
  }
  changeTickError()
  {
    this.service.AlterTable(new ChangeTable("tbDaemons", this.daemonId, "TimerAfterFail", parseInt(this.timertickValueError)*this.repeatError))
  }
}
