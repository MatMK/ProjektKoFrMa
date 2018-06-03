import { Component, Renderer2, OnInit, Testability } from '@angular/core';
import { ActivatedRoute, Router } from "@angular/router";
import { MatRadioModule, MatRadioButton } from '@angular/material/radio';
import { SetTask } from '../../../../server-connection/models/communication-models/task/set-task.model';
import { TaskRepeatingNoTimespan } from '../../../../server-connection/models/communication-models/task/task-repeating.model';
import { ServerConnectionService } from '../../../../server-connection/server-connection.service';
import { Data } from '../../../../server-connection/data.model';
import { IDestination } from '../../../../server-connection/models/communication-models/task/task-models/idestitnation.interface';
import { DestinationPlain } from '../../../../server-connection/models/communication-models/task/task-models/destinations/destination-plain.model';
import { DestinationRar } from '../../../../server-connection/models/communication-models/task/task-models/destinations/destination-rar.model';
import { DestinationZip } from '../../../../server-connection/models/communication-models/task/task-models/destinations/destination-zip.model';
import { Destination7z } from '../../../../server-connection/models/communication-models/task/task-models/destinations/destination-7z.model';
import { DestinationPathLocal } from '../../../../server-connection/models/communication-models/task/task-models/destinations/destination-path-local.model';
import { DestinationPathFTP } from '../../../../server-connection/models/communication-models/task/task-models/destinations/destination-path-ftp.model';
import { NetworkCredential } from '../../../../server-connection/models/communication-models/task/network-credential.model';
import { DestinationPathSFTP } from '../../../../server-connection/models/communication-models/task/task-models/destinations/destionation-path-sftp.model';
import { SourceMySQL } from '../../../../server-connection/models/communication-models/task/task-models/sources/source-my-sql.model';
import { ISource } from '../../../../server-connection/models/communication-models/task/task-models/isource.interface';
import { SourceMSSQL } from '../../../../server-connection/models/communication-models/task/task-models/sources/source-ms-sql.model';
import { SourceFolders } from '../../../../server-connection/models/communication-models/task/task-models/sources/source-file.model';
import { Time } from '@angular/common';
import { ScriptInfo } from '../../../../server-connection/models/communication-models/task/script-info.model';
import { ExceptionDate } from '../../../../server-connection/models/communication-models/task/exception-date.model';


@Component({
  selector: 'app-add-task',
  templateUrl: './add-task.component.html',
  styleUrls: ['./add-task.component.css']
})
export class AddTaskComponent  
{
  private daemonId : number;
  //Backuptype
  private backuptype : string = "Full";
  private fullBackupAfter : number;
  private followupTo : number;
  //Destinations
  private ncUsername : string;
  private ncPassword : string;
  private ncPath : string;
  private splitAfter : number;
  private compress : boolean = false;
  private compressType : string = "Rar";
  private compressLevel : number = 1;
  private destinationtype :string;
  private destinations : IDestination[] = [];
  //Source
  private srcPath : {id: number, value : string}[] = [{id: 0, value: ""}];
  private srcDatabaseName : string;
  private srcServerName : string;
  private srcPassword : string;
  private srcUsername : string;
  private sourcetype : string = "Local";
  //Repeat
  private repeat : number = 3600
  private repeatEvery : number;
  private executionDates : {id : number, date: Date, time: Time}[] = [{id:0,date: undefined, time:undefined}];
  private exceptionDates : {id : number, dateStart: Date, timeStart: Time, dateEnd: Date, timeEnd: Time, }[] = [];
  private repeatOptions : {value : number, text : string}[] = [
    {value: 1 , text:"Seconds"},
    {value: 60 , text:"Minutes"},
    {value: 3600 , text:"Hours"},
    {value: 86400, text:"Days"},
    {value: 604800 , text:"Weeks"},
  ]
  private repeatTill : Date;
  //Advanced
  private advanced : boolean;
  //log
  private logLevel : number = 7;
  private logOptions : {id: number, value : string}[] = [{id: 0, value: "Don't create log"}, 
    {id: 1, value: "Only errors that shut down entire daemon"}, 
    {id: 2, value: "Errors that cause some process to fail"}, 
    {id: 3, value: "Errors that program can handle"}, 
    {id: 4, value: "Basic info about operations that program runs"}, 
    {id: 5, value: "Debug info that could lead to fixing or optimizing some processes"}, 
    {id: 6, value: "Tracing info for every process that is likely to fail"}, 
    {id: 7, value: "Tracing info about everything program does"}, 
    {id: 8, value: "Tracing info including loop cycles"}, 
    {id: 9, value: "Tracing info including large loop cycles that will slow down the process a lot"}, 
    {id: 10, value: "Program will be more like a log writer than actually doing the process"}]
  //scirpts
  private scriptBefore : string;
  private scriptBeforePath : string;
  private scriptAfter : string;
  private scriptAfterPath : string;
  private scriptBeforeLocal : boolean = true;
  private scriptAfterLocal : boolean = true;
  private scriptBeforeType : string;
  private scriptAfterType : string;
  
  private temporaryFolderMaxBuffer : number;

  constructor(private activeRoute:ActivatedRoute, private service : ServerConnectionService, private renderer: Renderer2, private router : Router, private data : Data) {
    if(this.data.Data.tbCompletedTasks.data.length == 0)  
      this.service.RefreshData([4])
    this.activeRoute.params.subscribe(params => {
      this.daemonId = params.daemonid;
      this.checkIfNumberValid(false);
    });
   }
   AddTask()
  {
    if(this.checkIfNumberValid(true))
    {
      if(this.fullBackupAfter < 0)
      {
        alert("Full backup after every: cannot be null")
        return;
      }

      if(this.exceptionDates == undefined || this.executionDates.length == 0)
      {
        alert("At least one execution time is required")
        return;
      }
      this.exceptionDates.forEach(element => {
        if(element.dateEnd==undefined||element.dateStart == undefined||element.timeEnd==undefined||element.timeStart==undefined)
        {
          alert("Please fill in all exception.")
          return;
        }
      });
      this.executionDates.forEach(element => {
        if(element.date == undefined || element.time == undefined)
        {
          alert("Please fill in all execution dates");
          return;
        }
      });
      if(this.ncPath != undefined && this.ncPath.length !=0)
        this.AddLocalDestination();
      var i : number = 0;
      this.destinations.forEach(element => {
        if(element != undefined)
          i++;
      });
      if(i==0)
      {
        alert("Select at least one destination");
        return;
      }

      let newTask : SetTask = new SetTask()
      if(this.backuptype != "Full" && this.fullBackupAfter != undefined && this.fullBackupAfter > 0)
      {
        newTask.FullAfterBackup = "";
        if(this.followupTo == undefined)
        {
          newTask.FullAfterBackup = "0";
        }
        for (let i = 0; i < this.fullBackupAfter; i++) {
          newTask.FullAfterBackup += this.backuptype == "Incremental"?"1":"2"
        }
      }
      else if(this.backuptype != "Full")
      {
        newTask.FullAfterBackup += this.backuptype == "Incremental"?"1":"2"
      }
      else
      {
        newTask.FullAfterBackup = "0";
      }
      newTask.DaemonId = this.daemonId;
      newTask.Destinations = this.destinations.filter(function( element ) {
        return element !== undefined;
      });
      newTask.Sources = this.getSource(this.sourcetype, this.backuptype)
      if(newTask.Sources==undefined)
      {
        alert("Please select a source");
        return;
      }
      //Adding repeat task class
      if(this.repeatEvery <0)
      {
        alert("Repeat every: cannot be negative");
        return;
      }
      newTask.ExecutionTimes = new TaskRepeatingNoTimespan()
      newTask.ExecutionTimes.Repeating = this.repeatEvery * this.repeat;
      newTask.ExecutionTimes.RepeatTill = this.repeatTill;
      newTask.ExecutionTimes.ExecutionTimes = [];
      newTask.ExecutionTimes.ExceptionDates = [];
      let count : number = 0;
      this.executionDates.forEach(element => {
        if(element != undefined)
        {
          count++;
          newTask.ExecutionTimes.ExecutionTimes.push(this.parseDate(element.date, element.time))
        }
      });
      if(count == 0)
      {
        alert("Please insert at least one execution time");
        return;
      }
      this.exceptionDates.forEach(element => {
        if(element != undefined)
        {
          let exceptionDate : ExceptionDate = new ExceptionDate();
          exceptionDate.Start = this.parseDate(element.dateStart, element.timeStart)
          exceptionDate.End = this.parseDate(element.dateEnd, element.timeEnd)
          newTask.ExecutionTimes.ExceptionDates.push(exceptionDate)
        }
      });
      //Advanced
      if(this.advanced)
      {
        newTask.LogLevel = this.logLevel;
        if(this.temporaryFolderMaxBuffer < 0)
        {
          alert("Max temporary folder size cannot be lower than 0");
          return;
        }
        newTask.TemporaryFolderMaxBuffer = this.temporaryFolderMaxBuffer == 0?null:this.temporaryFolderMaxBuffer

        if(!this.scriptBeforeLocal)
        {
          if(this.scriptBeforePath != undefined && this.scriptBeforePath.length != 0)
          {
            newTask.ScriptBefore = new ScriptInfo();
            newTask.ScriptBefore.PathToLocalScript = this.scriptBeforePath;
          }
        }
        else
        {
          if(this.scriptBefore != undefined && this.scriptBefore.length != 0)
          {
            newTask.ScriptBefore = new ScriptInfo();
            newTask.ScriptBefore.ScriptItself = btoa(this.scriptBefore);
            newTask.ScriptBefore.ScriptItselfFormat = this.scriptAfterType;
          }
        }
        if(!this.scriptAfterLocal)
        {
          if(this.scriptAfterPath != undefined && this.scriptAfterPath.length != 0)
          {
            newTask.ScriptAfter = new ScriptInfo();
            newTask.ScriptAfter.PathToLocalScript = this.scriptAfterPath;
            newTask.ScriptAfter.ScriptItselfFormat = this.scriptAfterType;
          }
        }
        else
        {
          if(this.scriptAfter != undefined && this.scriptAfter.length != 0)
          {
            newTask.ScriptAfter = new ScriptInfo();
            newTask.ScriptAfter.ScriptItself = btoa(this.scriptAfter);
            newTask.ScriptAfter.ScriptItselfFormat = this.scriptAfterType;
          }
        }
      }
      else
      {
        newTask.LogLevel = 7;
      }
      console.log(newTask);
      this.service.SetTask([newTask])//.then(res => this.service.RefreshData([3]))
      //this.router.navigate(['backup', 'app','tasks']);
    }
  }
  isEnabled()
  {
    if(this.backuptype == "Incremental" || this.backuptype == "Differencial")
    {
      document.getElementById("sourceDbMY").setAttribute("disabled", "true")
      document.getElementById("sourceDbMS").setAttribute("disabled", "true")
      this.sourcetype = "Local"
    }
    else
    {
      document.getElementById("sourceDbMY").removeAttribute("disabled")
      document.getElementById("sourceDbMS").removeAttribute("disabled")
    }
    if(this.sourcetype == "MsSQL" || this.sourcetype == "MySQL")
    {
      document.getElementById("incr").setAttribute("disabled", "true")
      document.getElementById("diff").setAttribute("disabled", "true")
      this.backuptype = "Full"
    }
    else
    {
      document.getElementById("incr").removeAttribute("disabled")
      document.getElementById("diff").removeAttribute("disabled")
    }
  }
  parseDate(date : Date, time : Time) : Date
  {
    return new Date(date+"T"+time)
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
  
  private onDateChange(value : Date)
  {
    this.repeatTill= value;
  }
  fileUpload(event, scriptBefore : boolean) {
    var reader = new FileReader();
    if(event.srcElement.files.length != 0)
    {
      reader.readAsText(event.srcElement.files[0]);
      var me = this;
      reader.onload = function () {
        if(scriptBefore)
        {
          me.scriptBefore = reader.result;
          me.scriptBeforeType = event.srcElement.files[0].name.substring(event.srcElement.files[0].name.length-3)
        }
        else if(!scriptBefore)
        {
          me.scriptAfter = reader.result;
          me.scriptAfterType = event.srcElement.files[0].name.substring(event.srcElement.files[0].name.length-3)
        }
      }
    }
  }
  AddLocalDestination()
  {
    if(this.destinationtype != undefined)
    {
      if(this.ncPath == undefined || this.ncPath.length == 0)
      {
        alert("Please enter destination path")
        return;
      }
      //Adding destinaion to an array
      var result : IDestination = this.getDestination(this.compressType, this.compressLevel, this.splitAfter);
      if(result == undefined)
      {
        alert("Define destination");
      }
      else
      {
        if(this.destinationtype.trim() === 'LOCAL')
        {
          let dest = new DestinationPathLocal();
          dest.Path = this.ncPath;
          result.Path = dest;
        }
        if(this.destinationtype.trim() === 'FTP')
        {
          let dest = new DestinationPathFTP();
          dest.NetworkCredential = new NetworkCredential();
          dest.NetworkCredential.Password = this.ncPassword;
          dest.NetworkCredential.Password = this.ncUsername;
          dest.Path = this.ncPath;
          result.Path = dest;
        }
        if(this.destinationtype.trim() === 'SFTP')
        {
          let dest = new DestinationPathSFTP();
          dest.NetworkCredential = new NetworkCredential();
          dest.NetworkCredential.Password = this.ncPassword;
          dest.NetworkCredential.Password = this.ncUsername;
          dest.Path = this.ncPath;
          result.Path = dest;
        }
        this.destinations.push(result);
        //Rendering new destination
        var newDiv = this.renderer.createElement('div'); 
        newDiv.Id='idNewDiv'
        newDiv.className='aditionalDivClass'
        var inputDestination = <HTMLDivElement>document.getElementById("inputDestiDiv");
        var input = this.renderer.createElement('div');
        input.style = "display:inline-block; vertical-align: middle;";
        input.innerHTML = "Path: " + (this.ncPath.length>40?"..." + this.ncPath.substring(this.ncPath.length-43):this.ncPath) + ", Type: " + this.destinationtype +", Compression: "  + (this.compress?this.compressType: "None")
        var button = this.renderer.createElement('button');
        button.innerHTML = 'X';
        button.style = "display:inline-block; vertical-align: middle;float:right;";
        button.id = this.destinations.length-1;
        this.renderer.listen(button, 'click', (event) => this.RemoveLocalDestination(event));
        var br = this.renderer.createElement("br");
        this.renderer.appendChild(newDiv, input);
        this.renderer.appendChild(inputDestination,newDiv);
        this.renderer.appendChild(newDiv,button);

        this.compress = false;
        this.splitAfter = undefined;
        this.ncPath = undefined;
        this.ncPassword = undefined;
        this.ncUsername = undefined;
      }
    }
    else
    {
      alert("You need to select destination first")
    }
  }
  private getDestination(compression : string, compressionLevel : number, compressionMaxFile : number) : IDestination
  {
    if(!this.compress)
    {
      return new DestinationPlain()
    }
    if(compression == "Rar")
    {
      let result : DestinationRar = new DestinationRar();
      result.CompressionLevel = compressionLevel;
      result.SplitAfter = compressionMaxFile;
      return result;
    }
    if(compression == "Zip")
    {
      let result : DestinationZip = new DestinationZip();
      result.CompressionLevel = compressionLevel;
      result.SplitAfter = compressionMaxFile;
      return result;
    }
    if(compression == "7zip")
    {
      let result : Destination7z = new Destination7z();
      result.CompressionLevel = compressionLevel;
      result.SplitAfter = compressionMaxFile;
      return result;
    }
    return undefined;
  }
  private getSource(source : string, backup : string) : ISource
  {
    if(source == "Local")
    {
      let i : number = 0;
      this.srcPath.forEach(element => {
        if(element != undefined && element.value.length !=0)
          i++;
      });
      if(i == 0)
      {
        alert("Please select at least one source path");
        return undefined;
      }
      let result : SourceFolders = new SourceFolders();
      result.Paths = [];
      this.srcPath.forEach(element => {
        if(element != undefined)
          result.Paths.push(element.value);
      });
      return result;
    }
    if (backup == "Full")
    {
      if(source == "MySQL")
      {
        let result : SourceMySQL = new SourceMySQL()
        result.DatabaseName = this.srcDatabaseName;
        result.ServerName = this.srcServerName;
        result.NetworkCredential = {UserName: this.srcUsername, Password: this.srcPassword}
        return result;
      }
      else if(source == "MsSQL")
      {
        let result : SourceMSSQL = new SourceMSSQL()
        result.DatabaseName = this.srcDatabaseName;
        result.ServerName = this.srcServerName;
        result.NetworkCredential = {UserName: this.srcUsername, Password: this.srcPassword}
        return result;
      }
    }
    else
    {
      alert("Backup type is not compatible with" + source)
    }
    return undefined;
  }
  addSrcPath()
  {
    var count : number = this.srcPath.length;
    this.srcPath.push({id: count, value: ""});
  }
  removeSrcPath(id : number)
  {
    delete this.srcPath[id];
  }
  AddExecutionDate()
  {
    var i :number = this.executionDates.length;
    this.executionDates.push({id:i, date: undefined, time: undefined});
  }
  RemoveExecutionDate(id : number)
  {
    delete this.executionDates[id];
  }
  RemoveExceptionDate(id: number)
  {
    delete this.exceptionDates[id];
  }
  AddExceptionDate()
  {
    var i :number = this.exceptionDates.length;
    this.exceptionDates.push({id:i, dateStart: undefined, dateEnd: undefined, timeEnd: undefined, timeStart: undefined});
  }
  RemoveLocalDestination(event: any)
  {
    var target = event.target || event.srcElement || event.currentTarget;
    var inputDestiDiv = <HTMLDivElement>document.getElementById("inputDestiDiv");
    this.renderer.removeChild(inputDestiDiv,target.parentNode);
    delete this.destinations[target.id];
  }
}
