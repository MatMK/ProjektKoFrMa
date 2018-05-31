import { Component, Renderer2, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from "@angular/router";
import { MatRadioModule, MatRadioButton } from '@angular/material/radio';
import { SetTask } from '../../../../server-connection/models/communication-models/task/set-task.model';
import { EventEmitter } from 'events';
import { TaskRepeating } from '../../../../server-connection/models/communication-models/task/task-repeating.model';
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

@Component({
  selector: 'app-add-task',
  templateUrl: './add-task.component.html',
  styleUrls: ['./add-task.component.css']
})
export class AddTaskComponent  
{
  private daemonId : number;
  //private sourcepath : string;
  //private destinationpath : string;
  private backuptype : string;
  private date : Date;
  private ncUsername : string;
  private ncPassword : string;
  private ncPath : string;
  private splitAfter : number;

  private srcPath : {id: number, value : string}[] = [{id: 0, value: ""}];
  private srcDatabaseName : string;
  private srcServerName : string;
  private srcPassword : string;
  private srcUsername : string;
  
  private sourcetype : string;
  private destinationtype :string;
  private destinations : IDestination[] = [];
  private compress : boolean = false;
  private compressType : string = "Rar";
  private compressLevel : number = 1;

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
      if(this.ncPath != undefined && this.ncPath.length !=0)
        this.AddLocalDestination();
      let newTask : SetTask = new SetTask()
      newTask.DaemonId = this.daemonId;
      newTask.Destinations = this.destinations.filter(function( element ) {
        return element !== undefined;
      });
      newTask.Sources = this.getSource(this.sourcetype, this.backuptype)
      if(newTask.Sources==undefined)
        return;
      console.log(newTask);
      //this.data.Loading = true;
      //this.service.SetTask([newTask]).then(res => this.service.RefreshData([3]))
      //this.router.navigate(['backup', 'app','tasks']);
    }
  }
  private onDateChange(value : Date)
  {
    this.date = value;
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
        var input = this.renderer.createElement('p');
        input.style = "display:inline-block; vertical-align: middle;";
        input.innerHTML = "Path: " + (this.ncPath.length>20?"..." + this.ncPath.substring(this.ncPath.length-25):this.ncPath) + ", Type: " + this.destinationtype +", Compression: "  + (this.compress?this.compressType:"None")
        var button = this.renderer.createElement('button');
        button.innerHTML = 'X';
        button.style = "display:inline-block; vertical-align: middle;";
        button.id = this.destinations.length-1;
        this.renderer.listen(button, 'click', (event) => this.RemoveLocalDestination(event));
        var br = this.renderer.createElement("br");
        this.renderer.appendChild(newDiv, input);
        this.renderer.appendChild(inputDestination,newDiv);
        this.renderer.appendChild(newDiv,button);
        
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
/*
BUcheck() {
  var radioOptD = <HTMLInputElement>document.getElementById("distant")
  var radioSftp = <HTMLInputElement>document.getElementById("sftp")
  var radioDiv = <HTMLDivElement>document.getElementById("ifServer")
  if (radioOptD.checked || radioSftp.checked) {
   radioDiv.style.display = 'block';
  }
  else radioDiv.style.display = 'none';
  }
*/
/*
ShowAddSource(){
  var radioOptL = <HTMLInputElement>document.getElementById("sourceLocal")
  var radioOptF = <HTMLInputElement>document.getElementById("IdtypeFull")
  var sourceDiv = <HTMLDivElement>document.getElementById("sourceButtonDiv")
  if(radioOptL.checked && radioOptF.checked){
    sourceDiv.style.display = 'block';
  }
  else sourceDiv.style.display = 'none';
  }
*/
/*
  DbCheck(){
    var dbMYSQL = <HTMLInputElement>document.getElementById("sourceDbMY")
    var dbMSSQL = <HTMLInputElement>document.getElementById("sourceDbMS")
    var dbName = <HTMLInputElement>document.getElementById("inputDbName")
    if(dbMSSQL.checked || dbMYSQL.checked ){
      dbName.style.display = 'block';
    }
    else dbName.style.display = 'none';
  }
*/
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

ShowRepeat(){
  var checkbox = <HTMLInputElement>document.getElementById("repeatCheckboxId")
  var showRepeat = <HTMLDivElement>document.getElementById("showRepeat")

  if(checkbox.checked){
    showRepeat.style.display = 'block';
  }
  else
  showRepeat.style.display = 'none';
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
  RemoveLocalDestination(event: any)
  {
    var target = event.target || event.srcElement || event.currentTarget;
    var inputDestiDiv = <HTMLDivElement>document.getElementById("inputDestiDiv");
    this.renderer.removeChild(inputDestiDiv,target.parentNode);
    delete this.destinations[target.id];
  }
    AddSource(){
      var newDiv = this.renderer.createElement('div'); 
      newDiv.className='aditionalDivClass'

      var inputDestination = <HTMLDivElement>document.getElementById("inputDestiDiv");

      var input = this.renderer.createElement('input');
      input.type = 'text';
      input.Ngmodel ='newSource';
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

    AddExecutionDate(){
      var newDiv = this.renderer.createElement('div'); 
      newDiv.className='aditionalDivClass'

      var showRepeat = <HTMLDivElement>document.getElementById("inputDestiDiv");

      var textExecution = this.renderer.createText('Execution date');

      var input = this.renderer.createElement('input');
      input.type = 'datetime-local';
      input.Ngmodel ='newExecutionDate';
      input.className = 'BasicInputNew'
      input.placeholder ='Aditional execution date'

      var button = this.renderer.createElement('button'); 
      button.innerHTML = 'X';

      this.renderer.listen(button, 'click', (event) => this.RemoveExecutionDate(event) )
      var br = this.renderer.createElement("br");
      this.renderer.appendChild(newDiv, input);
      this.renderer.appendChild(showRepeat,newDiv);
      this.renderer.appendChild(newDiv,textExecution);
      this.renderer.appendChild(newDiv,button);
      this.renderer.appendChild(newDiv, br);
    }

    RemoveExecutionDate(event : any){
      var target = event.target || event.srcElement || event.currentTarget;
      var inputDestiDiv = <HTMLDivElement>document.getElementById("inputDestiDiv");
      this.renderer.removeChild(inputDestiDiv,target.parentNode);
    }

    AddExceptionDate(){
      var newDiv = this.renderer.createElement('div'); 
      newDiv.className='aditionalDivClass'

      var showRepeat = <HTMLDivElement>document.getElementById("inputDestiDiv");

      var textException = this.renderer.createText('Exception date');

      var input = this.renderer.createElement('input');
      input.type = 'datetime-local';
      input.Ngmodel ='newExceptionDate';
      input.className = 'BasicInputNew'
      input.placeholder ='Aditional exception date'

      var button = this.renderer.createElement('button'); 
      button.innerHTML = 'X';

      this.renderer.listen(button, 'click', (event) => this.RemoveExecutionDate(event) )
      var br = this.renderer.createElement("br");
      this.renderer.appendChild(newDiv, input);
      this.renderer.appendChild(showRepeat,newDiv);
      this.renderer.appendChild(newDiv,textException);
      this.renderer.appendChild(newDiv,button);
      this.renderer.appendChild(newDiv, br);
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
    this.renderer.setAttribute(destinationTypeRadioLocal, 'name', 'destTypeRadio');

    var textFTP = this.renderer.createText('FTP');
    var destinationTypeRadioFTP = this.renderer.createElement('input'); 
    destinationTypeRadioFTP.type = 'radio';
    destinationTypeRadioFTP.Name = 'destTypeRadio';
    destinationTypeRadioFTP.className = 'destTypeRad';
    destinationTypeRadioFTP.innerHTML = 'FTP';
    destinationTypeRadioFTP.click = 'BUcheck';
    this.renderer.setAttribute(destinationTypeRadioFTP, 'name', 'destTypeRadio');

    var textSFTP = this.renderer.createText('SFTP');
    var destinationTypeRadioSFTP = this.renderer.createElement('input'); 
    destinationTypeRadioSFTP.Name = 'destTypeRadio';
    destinationTypeRadioSFTP.type = 'radio';
    destinationTypeRadioSFTP.className = 'destTypeRad';
    destinationTypeRadioSFTP.innerHTML = 'SFTP';
    destinationTypeRadioSFTP.click = 'BUcheck';
    this.renderer.setAttribute(destinationTypeRadioSFTP, 'name', 'destTypeRadio');

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
    inputDestinationText.className = 'BasicInputNew';

    var inputCheckbox = this.renderer.createElement('input'); 
    inputCheckbox.type = 'checkbox';
    inputCheckbox.Id  = 'compressCheckbox';
    inputCheckbox.change ='ShowCompressNew()';

    var compressSelect = this.renderer.createElement('select');
    compressSelect.Id = 'compressSelect';
    compressSelect.className = 'BasicSelect';
    compressSelect.change = "ShowCompressNew()";
    

    var compressOptionRar = this.renderer.createElement('option');
    compressOptionRar.value = 'Rar';
    compressOptionRar.Id = 'compressOptionRar';
    compressOptionRar.innerHTML = 'Rar';

    var compressOption7zip = this.renderer.createElement('option');
    compressOption7zip.value = '7zip';
    compressOption7zip.Id = 'compressOption7zip';
    compressOption7zip.innerHTML = '7zip';

    var compressOptionZip = this.renderer.createElement('option');
    compressOptionZip.value = 'Zip';
    compressOptionZip.Id = 'compressOptionZip';
    compressOptionZip.innerHTML = 'Zip';

    var compressSelectRar = this.renderer.createElement('select');
    compressSelectRar.Id = 'compressSelectRar';
    compressSelectRar.className = 'BasicSelect';

    var cOptionRar0 = this.renderer.createElement('option');
    cOptionRar0.value = '0';
    cOptionRar0.Id = 'cOptionRar0';
    cOptionRar0.innerHTML = 'Rar- No Compression';

    var cOptionRar1 = this.renderer.createElement('option');
    cOptionRar1.value = '1';
    cOptionRar1.Id = 'cOptionRar1';
    cOptionRar1.innerHTML = 'Rar- Fastest';

    var cOptionRar2 = this.renderer.createElement('option');
    cOptionRar2.value = '2';
    cOptionRar2.Id = 'cOptionRar2';
    cOptionRar2.innerHTML = 'Rar- Fast';

    var cOptionRar3 = this.renderer.createElement('option');
    cOptionRar3.value = '3';
    cOptionRar3.Id = 'cOptionRar3';
    cOptionRar3.innerHTML = 'Rar- Normal';

    var cOptionRar4 = this.renderer.createElement('option');
    cOptionRar4.value = '4';
    cOptionRar4.Id = 'cOptionRar4';
    cOptionRar4.innerHTML = 'Rar- Good';

    var cOptionRar5 = this.renderer.createElement('option');
    cOptionRar5.value = '5';
    cOptionRar5.Id = 'cOptionRar5';
    cOptionRar5.innerHTML = 'Rar- Best';

    var compressSelect7zip = this.renderer.createElement('select');
    compressSelect7zip.Id = 'compressSelect7zip';
    compressSelect7zip.className = 'BasicSelect';

    var cOption7zip0 = this.renderer.createElement('option');
    cOption7zip0.value = '0';
    cOption7zip0.Id = 'cOption7zip0';
    cOption7zip0.innerHTML = '7zip- Optimal';

    var cOption7zip1 = this.renderer.createElement('option');
    cOption7zip1.value = '1';
    cOption7zip1.Id = 'cOption7zip';
    cOption7zip1.innerHTML = '7zip- Fastest';

    var cOption7zip2 = this.renderer.createElement('option');
    cOption7zip2.value = '2';
    cOption7zip2.Id = 'cOption7zip';
    cOption7zip2.innerHTML = '7zip- No Compression';

    var compressSelectZip = this.renderer.createElement('select');
    compressSelectZip.Id = 'compressSelectZip';
    compressSelectZip.className = 'BasicSelect';

    var cOptionZip0 = this.renderer.createElement('option');
    cOptionZip0.value = '0';
    cOptionZip0.Id = 'cOptionZip0';
    cOptionZip0.innerHTML = 'Zip- No Compression';

    var cOptionZip1 = this.renderer.createElement('option');
    cOptionZip1.value = '1';
    cOptionZip1.Id = 'cOptionZip1';
    cOptionZip1.innerHTML = 'Zip- Fastest';

    var cOptionZip2 = this.renderer.createElement('option');
    cOptionZip2.value = '2';
    cOptionZip2.Id = 'cOptionZip2';
    cOptionZip2.innerHTML = 'Zip- Fast';

    var cOptionZip3 = this.renderer.createElement('option');
    cOptionZip3.value = '3';
    cOptionZip3.Id = 'cOptionZip3';
    cOptionZip3.innerHTML = 'Zip- Normal';

    var cOptionZip4 = this.renderer.createElement('option');
    cOptionZip4.value = '4';
    cOptionZip4.Id = 'cOptionZip4';
    cOptionZip4.innerHTML = 'Zip- Maximum';

    var cOptionZip5 = this.renderer.createElement('option');
    cOptionZip5.value = '5';
    cOptionZip5.Id = 'cOptionZip5';
    cOptionZip5.innerHTML = 'Zip- Ultra';

    var button1 = this.renderer.createElement('button'); 
    button1.innerHTML = 'X';
    button1.Id = 'newButton';

    this.renderer.listen(button1, 'click', (event) => this.RemoveDestinationNew(event) )

    var divLocalD = this.renderer.createElement('div');
    divLocalD.className = 'RadioDiv'

    var divFTPD = this.renderer.createElement('div');
    divFTPD.className = 'RadioDiv'

    var divSFTPD = this.renderer.createElement('div');
    divSFTPD.className = 'RadioDiv'

    compressSelectRar.style.display = 'none';
    compressSelectZip.style.display = 'none';
    compressSelect7zip.style.display = 'none';
    
    this.renderer.listen(compressSelect, 'change',()=> this.ShowCompressNew());

    var br = this.renderer.createElement("br");
    var hr = this.renderer.createElement("hr");
    var hr1 = this.renderer.createElement("hr");

    
    this.renderer.appendChild(mensiDiv,destinationDiv);
    
    this.renderer.appendChild(destinationDiv,divLocalD);
    this.renderer.appendChild(divLocalD,textLocal);
    this.renderer.appendChild(divLocalD,destinationTypeRadioLocal);

    this.renderer.appendChild(destinationDiv,divFTPD);
    this.renderer.appendChild(divFTPD,textFTP);
    this.renderer.appendChild(divFTPD,destinationTypeRadioFTP);

    this.renderer.appendChild(destinationDiv,divSFTPD);
    this.renderer.appendChild(divSFTPD,textSFTP);
    this.renderer.appendChild(divSFTPD,destinationTypeRadioSFTP);
    
    this.renderer.appendChild(destinationDiv,inputUsername);
    this.renderer.appendChild(destinationDiv,inputPassword);
    this.renderer.appendChild(destinationDiv,inputDestinationText);
    
    this.renderer.appendChild(destinationDiv,hr);

    this.renderer.appendChild(destinationDiv, compressSelect)
    this.renderer.appendChild(compressSelect, cOptionRar0)
    this.renderer.appendChild(compressSelect, cOptionRar1)
    this.renderer.appendChild(compressSelect, cOptionRar2)
    this.renderer.appendChild(compressSelect, cOptionRar3)
    this.renderer.appendChild(compressSelect, cOptionRar4)
    this.renderer.appendChild(compressSelect, cOptionRar5)
    this.renderer.appendChild(compressSelect, cOption7zip0)
    this.renderer.appendChild(compressSelect, cOption7zip1)
    this.renderer.appendChild(compressSelect, cOption7zip2)
    this.renderer.appendChild(compressSelect, cOptionZip0)
    this.renderer.appendChild(compressSelect, cOptionZip1)
    this.renderer.appendChild(compressSelect, cOptionZip2)
    this.renderer.appendChild(compressSelect, cOptionZip3)
    this.renderer.appendChild(compressSelect, cOptionZip4)
    this.renderer.appendChild(compressSelect, cOptionZip5)
   

    /*this.renderer.appendChild(destinationDiv, compressSelectRar)
    this.renderer.appendChild(compressSelectRar, cOptionRar0)
    this.renderer.appendChild(compressSelectRar, cOptionRar1)
    this.renderer.appendChild(compressSelectRar, cOptionRar2)
    this.renderer.appendChild(compressSelectRar, cOptionRar3)
    this.renderer.appendChild(compressSelectRar, cOptionRar4)
    this.renderer.appendChild(compressSelectRar, cOptionRar5)

    this.renderer.appendChild(destinationDiv, compressSelect7zip)
    this.renderer.appendChild(compressSelect7zip, cOption7zip0)
    this.renderer.appendChild(compressSelect7zip, cOption7zip1)
    this.renderer.appendChild(compressSelect7zip, cOption7zip2)

    this.renderer.appendChild(destinationDiv, compressSelectZip)
    this.renderer.appendChild(compressSelectZip, cOptionZip0)
    this.renderer.appendChild(compressSelectZip, cOptionZip1)
    this.renderer.appendChild(compressSelectZip, cOptionZip2)
    this.renderer.appendChild(compressSelectZip, cOptionZip3)
    this.renderer.appendChild(compressSelectZip, cOptionZip4)
    this.renderer.appendChild(compressSelectZip, cOptionZip5)*/

    this.renderer.appendChild(destinationDiv,hr1);
    this.renderer.appendChild(destinationDiv,button1)

    
  /* this.renderer.insertBefore(mensiDiv,destinationTypeRadioLocal,);
  
  mensiDiv.appendChild(destinationTypeRadioLocal)*/
  }
RemoveDestinationNew(event: any){
    var target = event.target || event.srcElement || event.currentTarget;
    var inputDestiDiv = <HTMLDivElement>document.getElementById("mensiDiv");
    this.renderer.removeChild(inputDestiDiv,target.parentNode);
}

ShowCompressNew()
  {
    var selectBox = <HTMLSelectElement>document.getElementById("compressSelect")
    var optionRar = <HTMLOptionElement>document.getElementById("compressOptionRar")
    var option7zip = <HTMLOptionElement>document.getElementById("compressOption7zip")
    var optionZip = <HTMLOptionElement>document.getElementById("compressOptionZip")
    var dropRar = <HTMLSelectElement>document.getElementById("compressSelectRar")
    var dropZip = <HTMLSelectElement>document.getElementById("compressSelectZip")
    var drop7Zip = <HTMLSelectElement>document.getElementById("compressSelect7zip")

    if(selectBox.selectedIndex == 1){
      dropRar.style.display = 'block';
      drop7Zip.style.display = 'none'; 
      dropZip.style.display = 'none'; 
    }
    else if(selectBox.selectedIndex == 2){
      drop7Zip.style.display = 'block';
      dropZip.style.display = 'none'; 
      dropRar.style.display = 'none'; 
    }
    else if(selectBox.selectedIndex == 3){
      dropZip.style.display = 'block';
      drop7Zip.style.display = 'none'; 
      dropRar.style.display = 'none'; 
    }
  }
}
