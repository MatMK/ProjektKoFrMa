import { Component, Renderer2, OnInit  } from '@angular/core';
import { ActivatedRoute, Router } from "@angular/router";
import { MatRadioModule, MatRadioButton } from '@angular/material/radio';
import { SetTask } from '../../../../server-connection/models/communication-models/task/set-task.model';
import { EventEmitter } from 'events';
import { TaskRepeating } from '../../../../server-connection/models/communication-models/task/task-repeating.model';
import { ServerConnectionService } from '../../../../server-connection/server-connection.service';
import { Data } from '../../../../server-connection/data.model';

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

  constructor(private activeRoute:ActivatedRoute, private service : ServerConnectionService, private renderer: Renderer2, private router : Router, private data : Data) {
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
      var select = (<HTMLSelectElement> document.getElementById('dropdownCompress'));
      t.CompressionType = select.options[select.selectedIndex].value;
      if(t.CompressionType == "Rar")
      {
        var select = (<HTMLSelectElement> document.getElementById('dropdownRar'));
        t.CompressionLevel = +select.options[select.selectedIndex].value;
      }
      if(t.CompressionType == "7zip")
      {
        var select = (<HTMLSelectElement> document.getElementById('dropdownZip'));
        t.CompressionLevel = +select.options[select.selectedIndex].value;
      }
      if(t.CompressionType == "Zip")
      {
        var select = (<HTMLSelectElement> document.getElementById('dropdown7zip'));
        t.CompressionLevel = +select.options[select.selectedIndex].value;
      }
      alert(t.CompressionLevel);
      this.data.Loading = true;
      this.service.SetTask([t]).then(res => this.service.RefreshData([3]))
      this.router.navigate(['backup', 'app','tasks']);
    }
  }
  private onDateChange(value : Date)
  {
    this.date = value;
  }
  
   

BUcheck() {
  var radioOptD = <HTMLInputElement>document.getElementById("distant")
  var radioSftp = <HTMLInputElement>document.getElementById("sftp")
  var radioDiv = <HTMLDivElement>document.getElementById("ifServer")
  if (radioOptD.checked || radioSftp.checked) {
   radioDiv.style.display = 'block';
  }
  else radioDiv.style.display = 'none';
  }

ShowAddSource(){
  var radioOptL = <HTMLInputElement>document.getElementById("sourceLocal")
  var sourceDiv = <HTMLDivElement>document.getElementById("sourceButtonDiv")
  if(radioOptL.checked){
    sourceDiv.style.display = 'block';
  }
  else sourceDiv.style.display = 'none';
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
      var newDiv = this.renderer.createElement('div'); 
      newDiv.Id='idNewDiv'
      newDiv.className='aditionalDivClass'
      var inputDestination = <HTMLDivElement>document.getElementById("inputDestiDiv");
      var input = this.renderer.createElement('input');
      input.type = 'text';
      input.Ngmodel ='kokong';
      input.className = 'BasicInputNew';
      input.placeholder ='Aditional destination';
      var button = this.renderer.createElement('button'); 
      button.innerHTML = 'X';

      this.renderer.listen(button, 'click', (event) => this.RemoveLocalDestination(event));
    
      var br = this.renderer.createElement("br");
      this.renderer.appendChild(newDiv, input);
      this.renderer.appendChild(inputDestination,newDiv);
      this.renderer.appendChild(newDiv,button);
      this.renderer.appendChild(newDiv, br);
      /*inputDestination.appendChild(input);*/
      /*inputDestination.appendChild(br);*/
      
    }
    RemoveLocalDestination(event: any){
      var target = event.target || event.srcElement || event.currentTarget;
      var inputDestiDiv = <HTMLDivElement>document.getElementById("inputDestiDiv");
      this.renderer.removeChild(inputDestiDiv,target.parentNode);
    }

    AddSource(){
      var newDiv = this.renderer.createElement('div'); 
      newDiv.className='aditionalDivClass'

      var inputDestination = <HTMLDivElement>document.getElementById("inputDestiDiv");

      var input = this.renderer.createElement('input');
      input.type = 'text';
      input.Ngmodel ='vong';
      input.className = 'BasicInputNew'
      input.placeholder ='Aditional source'

      var button = this.renderer.createElement('button'); 
      button.innerHTML = 'X';

      this.renderer.listen(button, 'click', (event) => this.RemoveSource(event) )
      var br = this.renderer.createElement("br");
      this.renderer.appendChild(newDiv, input);
      this.renderer.appendChild(inputDestination,newDiv);
      this.renderer.appendChild(newDiv,button);
      this.renderer.appendChild(newDiv, br);
    }

    RemoveSource(event: any){
      var target = event.target || event.srcElement || event.currentTarget;
      var inputDestiDiv = <HTMLDivElement>document.getElementById("inputDestiDiv");
      this.renderer.removeChild(inputDestiDiv,target.parentNode);
    }

  AddDestinationNew(){

    var mensiDiv = <HTMLDivElement>document.getElementById("mensiDiv");

    var destinationDiv = this.renderer.createElement('div'); 
    destinationDiv.className='newTaskDiv'

    var ifServerDiv = this.renderer.createElement('div'); 
    ifServerDiv.Name = 'ifServer';
    ifServerDiv.style.display = 'none';

    var ifCompressDiv = this.renderer.createElement('div'); 
    ifCompressDiv.Id = 'divCompress';
    ifCompressDiv.style.display = 'none';

    var textLocal = this.renderer.createText('Local');
    var destinationTypeRadioLocal = this.renderer.createElement('input'); 
    destinationTypeRadioLocal.type = 'radio';
    destinationTypeRadioLocal.Name = 'destTypeRadio';
    destinationTypeRadioLocal.className = 'destTypeRad';
    destinationTypeRadioLocal.innerHTML = 'Local';
    destinationTypeRadioLocal.click = 'BUcheck';

    var textFTP = this.renderer.createText('FTP');
    var destinationTypeRadioFTP = this.renderer.createElement('input'); 
    destinationTypeRadioFTP.type = 'radio';
    destinationTypeRadioFTP.Name = 'destTypeRadio';
    destinationTypeRadioFTP.className = 'destTypeRad';
    destinationTypeRadioFTP.innerHTML = 'FTP';
    destinationTypeRadioFTP.click = 'BUcheck';

    var textSFTP = this.renderer.createText('SFTP');
    var destinationTypeRadioSFTP = this.renderer.createElement('input'); 
    destinationTypeRadioSFTP.Name = 'destTypeRadio';
    destinationTypeRadioSFTP.type = 'radio';
    destinationTypeRadioSFTP.className = 'destTypeRad';
    destinationTypeRadioSFTP.innerHTML = 'SFTP';
    destinationTypeRadioSFTP.click = 'BUcheck';

    var inputUsername = this.renderer.createElement('input'); 
    inputUsername.type = 'text';
    inputUsername.placeholder = 'Username';
    inputUsername.className = 'BasicInputNew';

    var inputPassword = this.renderer.createElement('input'); 
    inputPassword.type = 'text';
    inputPassword.placeholder = 'Password';
    inputPassword.className = 'BasicInputNew';

    var inputDestinationText = this.renderer.createElement('input'); 
    inputDestinationText.type = 'text';
    inputDestinationText.placeholder ='Path to destination';

    var inputCheckbox = this.renderer.createElement('input'); 
    inputCheckbox.type = 'checkbox';
    inputCheckbox.Id  = 'compressCheckbox';
    inputCheckbox.change ='showCompress()';

    var compressSelect = this.renderer.createElement('select');
    compressSelect.Id = 'compressSelect';

    var compressOptionRar = this.renderer.createElement('option');
    compressOptionRar.value = 'Rar';
    compressOptionRar.Id = 'compressOptionRar';
    compressOptionRar.innerHTML = 'Rar';

    var compressOption7zip = this.renderer.createElement('option');
    compressOption7zip.value = '7zip';
    compressOption7zip.Id = 'compressOption7zip';
    compressOption7zip.innerHTML = '7zip';

    var compressOptionzip = this.renderer.createElement('option');
    compressOptionzip.value = 'zip';
    compressOptionzip.Id = 'compressOption7zip';
    compressOptionzip.innerHTML = 'zip';

    var compressSelectRar = this.renderer.createElement('select');
    compressSelectRar.Id = 'compressSelectRar';
    compressSelectRar.className = 'basicSelect';

    var cOptionRar0 = this.renderer.createElement('option');
    cOptionRar0.value = '0';
    cOptionRar0.Id = 'cOptionRar0';
    cOptionRar0.innerHTML = 'No Compression';

    var cOptionRar1 = this.renderer.createElement('option');
    cOptionRar1.value = '1';
    cOptionRar1.Id = 'cOptionRar1';
    cOptionRar1.innerHTML = 'Fastest';

    var cOptionRar2 = this.renderer.createElement('option');
    cOptionRar2.value = '2';
    cOptionRar2.Id = 'cOptionRar2';
    cOptionRar2.innerHTML = 'Fast';

    var cOptionRar3 = this.renderer.createElement('option');
    cOptionRar3.value = '3';
    cOptionRar3.Id = 'cOptionRar3';
    cOptionRar3.innerHTML = 'Normal';

    var cOptionRar4 = this.renderer.createElement('option');
    cOptionRar4.value = '4';
    cOptionRar4.Id = 'cOptionRar4';
    cOptionRar4.innerHTML = 'Good';

    var cOptionRar5 = this.renderer.createElement('option');
    cOptionRar5.value = '5';
    cOptionRar5.Id = 'cOptionRar5';
    cOptionRar5.innerHTML = 'Best';

    var compressSelect7zip = this.renderer.createElement('select');
    compressSelect7zip.Id = 'compressSelect7zip';
    compressSelect7zip.className = 'basicSelect';

    var cOption7zip0 = this.renderer.createElement('option');
    cOption7zip0.value = '0';
    cOption7zip0.Id = 'cOption7zip0';
    cOption7zip0.innerHTML = 'Optimal';

    var cOption7zip1 = this.renderer.createElement('option');
    cOption7zip1.value = '1';
    cOption7zip1.Id = 'cOption7zip';
    cOption7zip1.innerHTML = 'Fastest';

    var cOption7zip2 = this.renderer.createElement('option');
    cOption7zip2.value = '2';
    cOption7zip2.Id = 'cOption7zip';
    cOption7zip2.innerHTML = 'No Compression';

    var compressSelectZip = this.renderer.createElement('select');
    compressSelectZip.Id = 'compressSelectZip';
    compressSelectZip.className = 'basicSelect';

    var cOptionZip0 = this.renderer.createElement('option');
    cOptionZip0.value = '0';
    cOptionZip0.Id = 'cOptionZip0';
    cOptionZip0.innerHTML = 'No Compression';

    var cOptionZip1 = this.renderer.createElement('option');
    cOptionZip1.value = '1';
    cOptionZip1.Id = 'cOptionZip1';
    cOptionZip1.innerHTML = 'Fastest';

    var cOptionZip2 = this.renderer.createElement('option');
    cOptionZip2.value = '2';
    cOptionZip2.Id = 'cOptionZip2';
    cOptionZip2.innerHTML = 'Fast';

    var cOptionZip3 = this.renderer.createElement('option');
    cOptionZip3.value = '3';
    cOptionZip3.Id = 'cOptionZip3';
    cOptionZip3.innerHTML = 'Normal';

    var cOptionZip4 = this.renderer.createElement('option');
    cOptionZip4.value = '4';
    cOptionZip4.Id = 'cOptionZip4';
    cOptionZip4.innerHTML = 'Maximum';

    var cOptionZip5 = this.renderer.createElement('option');
    cOptionZip5.value = '5';
    cOptionZip5.Id = 'cOptionZip5';
    cOptionZip5.innerHTML = 'Ultra';

    var button = this.renderer.createElement('button'); 
    button.innerHTML = 'X';

    this.renderer.listen(button, 'click', (event) => this.RemoveDestinationNew(event) )

    var br = this.renderer.createElement("br");

    this.renderer.appendChild(mensiDiv,destinationDiv);
    /*
    this.renderer.appendChild(mensiDiv,destinationTypeRadioLocal);
    this.renderer.appendChild(destinationTypeRadioLocal,textLocal);
    this.renderer.appendChild(mensiDiv,destinationTypeRadioFTP);
    this.renderer.appendChild(mensiDiv,destinationTypeRadioSFTP);
*/
  }
RemoveDestinationNew(event: any){
    var target = event.target || event.srcElement || event.currentTarget;
    var inputDestiDiv = <HTMLDivElement>document.getElementById("mensiDiv");
    this.renderer.removeChild(inputDestiDiv,target.parentNode);
}
  }
