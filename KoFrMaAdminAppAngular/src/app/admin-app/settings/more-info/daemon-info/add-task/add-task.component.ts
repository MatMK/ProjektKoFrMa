import { Component, Renderer2, OnInit  } from '@angular/core';
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
export class AddTaskComponent  {
  private daemonId : number;
  private sourcepath : string;
  private destinationpath : string;
  private backuptype : string;
  private date : Date;
  private ncUsername : string;
  private ncPassword : string;
  private compression : string;

  constructor(private activeRoute:ActivatedRoute, private service : ServerConnectionService, private renderer: Renderer2, private router : Router) {
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
    alert(this.compression);
    /*
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
    }*/
  }
  private onDateChange(value : Date)
  {
    this.date = value;
  }
  
   

BUcheck() {
  var radioOptD = <HTMLInputElement>document.getElementById("distant")
  var radioDiv = <HTMLDivElement>document.getElementById("ifServer")
  if (radioOptD.checked) {
   radioDiv.style.display = 'block';
  }
  else radioDiv.style.display = 'none';
  }

ShowCompress(){
var checkbox = <HTMLInputElement>document.getElementById("checkboxCompression")
var compressDiv = <HTMLDivElement>document.getElementById("divCompress")
var dropMenuDiv = <HTMLDivElement>document.getElementById("dropMenuDiv")
if(checkbox.checked){
 compressDiv.style.display = 'block';
 dropMenuDiv.style.display = 'block';
}
else
compressDiv.style.display = 'none';

}
ShowCompressOption(){
  var selectBox = <HTMLSelectElement>document.getElementById("dropdownCompress")
  var optionRar = <HTMLOptionElement>document.getElementById("optionRar")
  var option7zip = <HTMLOptionElement>document.getElementById("option7zip")
  var optionZip = <HTMLOptionElement>document.getElementById("optionZip")
  var dropRar = <HTMLSelectElement>document.getElementById("dropdownRar")
  var dropZip = <HTMLSelectElement>document.getElementById("dropdownZip")
  var drop7Zip = <HTMLSelectElement>document.getElementById("dropdown7zip")
  /*if(optionRar.selected){
    dropRar.style.display = 'block';
    drop7Zip.style.display = 'none'; 
    dropZip.style.display = 'none'; 
  }
  else if(option7zip.selected){
    drop7Zip.style.display = 'block';
    dropZip.style.display = 'none'; 
    dropRar.style.display = 'none'; 
  }
  else {
    dropZip.style.display = 'block';
    drop7Zip.style.display = 'none'; 
    dropRar.style.display = 'none'; 
  }*/

  if(selectBox.selectedIndex == 0){
    dropRar.style.display = 'block';
    drop7Zip.style.display = 'none'; 
    dropZip.style.display = 'none'; 
  }
  else if(selectBox.selectedIndex == 1){
    drop7Zip.style.display = 'block';
    dropZip.style.display = 'none'; 
    dropRar.style.display = 'none'; 
  }
  else if(selectBox.selectedIndex == 2){
    dropZip.style.display = 'block';
    drop7Zip.style.display = 'none'; 
    dropRar.style.display = 'none'; 
  }
 
}
AddLocalDestination(){
      var AddDesti = <HTMLInputElement>document.getElementById("distant")   
      var inputDestination = <HTMLDivElement>document.getElementById("inputDestiDiv");
      var input = this.renderer.createElement('input');
      input.type = 'text';
      input.name = 'trying';
      input.Ngmodel ='kokotko';
      input.class = 'autoDestiLocal'
      input.placeholder ='Aditional destination'
      var button = this.renderer.createElement('button'); 
      button.innerHTML = 'Remove';

      this.renderer.listen(button, 'click', (event) => this.RemoveLocalDestination(event));
    
      var br = this.renderer.createElement("br");
      this.renderer.appendChild(inputDestination, input);
      this.renderer.appendChild(inputDestination,button);
      this.renderer.appendChild(inputDestination, br);
      /*inputDestination.appendChild(input);*/
      /*inputDestination.appendChild(br);*/
      
    }
    RemoveLocalDestination(event: any){
      var target = event.target || event.srcElement || event.currentTarget;
      var inputDestiDiv = <HTMLDivElement>document.getElementById("inputDestiDiv");
      this.renderer.removeChild(inputDestiDiv,target.parent.node);
    }
  

  }
