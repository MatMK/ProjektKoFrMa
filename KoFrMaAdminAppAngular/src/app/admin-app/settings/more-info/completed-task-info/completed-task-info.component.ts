import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, Params } from '@angular/router';
import { Data } from '../../../server-connection/data.model';
import { ServerConnectionService } from '../../../server-connection/server-connection.service';

@Component({
  selector: 'app-completed-task-info',
  templateUrl: './completed-task-info.component.html',
  styleUrls: ['./completed-task-info.component.css']
})
export class CompletedTaskInfoComponent {

  
  //placeholder
  taskId : number = 5;
  log : string ="21.05.2018 19:03:33 5 Inicializing the backup instance...\n21.05.2018 19:03:33 6 At least one of destinations is plain local backup, there is no need to create temporary folder.\n21.05.2018 19:03:33 4 Starting the backup...\n21.05.2018 19:03:33 7 Deciding what type of backup it is...21.05.2018 19:03:33 5 Inicializing the backup instance...\n21.05.2018 19:03:33 6 At least one of destinations is plain local backup, there is no need to create temporary folder.\n21.05.2018 19:03:33 4 Starting the backup...\n21.05.2018 19:03:33 7 Deciding what type of backup it is...21.05.2018 19:03:33 5 Inicializing the backup instance...\n21.05.2018 19:03:33 6 At least one of destinations is plain local backup, there is no need to create temporary folder.\n21.05.2018 19:03:33 4 Starting the backup...\n21.05.2018 19:03:33 7 Deciding what type of backup it is...21.05.2018 19:03:33 5 Inicializing the backup instance...\n21.05.2018 19:03:33 6 At least one of destinations is plain local backup, there is no need to create temporary folder.\n21.05.2018 19:03:33 4 Starting the backup...\n21.05.2018 19:03:33 7 Deciding what type of backup it is...21.05.2018 19:03:33 5 Inicializing the backup instance...\n21.05.2018 19:03:33 6 At least one of destinations is plain local backup, there is no need to create temporary folder.\n21.05.2018 19:03:33 4 Starting the backup...\n21.05.2018 19:03:33 7 Deciding what type of backup it is...21.05.2018 19:03:33 5 Inicializing the backup instance...\n21.05.2018 19:03:33 6 At least one of destinations is plain local backup, there is no need to create temporary folder.\n21.05.2018 19:03:33 4 Starting the backup...\n21.05.2018 19:03:33 7 Deciding what type of backup it is..."
  isSuccessful : boolean = true;
  timeOfCompletion : string = "1.1.2018 21:11:00";
  //Accual code
  /*
  taskId : number;
  log : string;
  isSuccessful : boolean;
  timeOfCompletion : Date;*/
/*
  constructor(private activeRoute:ActivatedRoute, private data : Data, private router : Router, private service : ServerConnectionService) {
    this.activeRoute.params.subscribe(params => 
    {
      this.taskId = params.completedTaskId;
      if(this.checkIfNumberValid(true))
      {
        if(this.data.Data.tbCompletedTasks.data.length == 0)
        {
          this.service.RefreshData([4]).then(res=>{
              this.data.Data.tbCompletedTasks.data.forEach(element=> {
                if(this.taskId == element.Id)
                {
                  this.log = element.DebugLog;
                  this.taskId = element.Id;
                  this.isSuccessful = element.IsSuccessful;
                  this.timeOfCompletion = element.TimeOfCompetion;
                }
              })
          });
        }
        else
        {
          this.data.Data.tbCompletedTasks.data.forEach(element=> {
            if(this.taskId == element.Id)
            {
              this.log = element.DebugLog;
              this.taskId = element.Id;
              this.isSuccessful = element.IsSuccessful;
              this.timeOfCompletion = element.TimeOfCompetion;
            }
          });
        }
      }
    })
   }
   */
   checkIfNumberValid(showMsg : boolean) : boolean
   {
    if (!(this.taskId >= 0 || this.taskId<0))
    {
      this.taskId = undefined;
      if(showMsg)
        alert("Id is not valid!")
      return false;
    }
    return true;
   }
}
