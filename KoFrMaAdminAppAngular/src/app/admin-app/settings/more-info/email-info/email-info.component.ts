import { Component, OnInit } from '@angular/core';
import { Time } from '@angular/common';
import { MatCheckbox } from '@angular/material'
import { Data } from '../../../server-connection/data.model';
import { ServerConnectionService } from '../../../server-connection/server-connection.service';
import { EditEmailRequest } from '../../../server-connection/models/communication-models/post-admin/edit-email-request.model';
import { ExceptionDate } from '../../../server-connection/models/communication-models/task/exception-date.model';
import { TaskRepeatingNoTimespan } from '../../../server-connection/models/communication-models/task/task-repeating.model';
import { Router } from '@angular/router';



@Component({
  selector: 'app-email-info',
  templateUrl: './email-info.component.html',
  styleUrls: ['./email-info.component.css']
})
export class EmailInfoComponent implements OnInit {

  //Emails
  private send : number = 3600;
  private options : {value : number, text : string}[] = [
    {value: 1 , text:"Seconds"},
    {value: 60 , text:"Minutes"},
    {value: 3600 , text:"Hours"},
    {value: 86400, text:"Days"},
    {value: 604800 , text:"Weeks"}]
  private sendEvery : number;
  private executionDates : {id : number, date: Date, time: Time}[] = [];
  private exceptionDates : {id : number, dateStart: Date, timeStart: Time, dateEnd: Date, timeEnd: Time, }[] = [];
  private sendTill : Date;
  private emailCheck : boolean;
  
  constructor(private service : ServerConnectionService, private data : Data, private router : Router) { 
    this.service.GetMail().then(res => {
      console.log(res);
      this.emailCheck = res.RecieveMail;
    });
  }
  Apply()
  {
    var email : EditEmailRequest = new EditEmailRequest("EditEmailRequest",null,null)
    email.RecieveMail = this.emailCheck;
    if(this.emailCheck)
    {
      email.Repeating = new TaskRepeatingNoTimespan;
      email.Repeating.ExceptionDates = [];
      email.Repeating.ExecutionTimes = [];
      var i : number = 0;
      this.executionDates.forEach(element => {
        if(element != undefined && element.date != undefined && element.time != undefined)
        {
          i++;
          email.Repeating.ExecutionTimes.push(this.parseDate(element.date, element.time));
        }
      });
      if(i == 0 )
      {
        alert("Please use at least one send date");
        return;
      }
      this.exceptionDates.forEach(element => {
        if(this.exceptionDates != undefined)
        {
          var i : ExceptionDate = new ExceptionDate();
          let exceptionDate : ExceptionDate = new ExceptionDate();
          exceptionDate.Start = this.parseDate(element.dateStart, element.timeStart)
          exceptionDate.End = this.parseDate(element.dateEnd, element.timeEnd)
          email.Repeating.ExceptionDates.push(exceptionDate)
        }      
      });
      email.Repeating.RepeatTill = this.sendTill;
      email.Repeating.Repeating = this.send * this.sendEvery;
    }
    this.data.Loading = true;    
    this.service.ChangeEmail(email)
    this.router.navigate(['backup', 'app','tasks']);
  }
  parseDate(date : Date, time : Time) : Date
  {
    return new Date(date+"T"+time)
  }
  AddSendDate()
  {
    var i :number = this.executionDates.length;
    this.executionDates.push({id:i, date: undefined, time: undefined});
  }
  RemoveSendDate(id : number)
  {
    delete this.executionDates[id];
  }
  RemoveExceptionDate(id: number)
  {
    delete this.exceptionDates[id];
  }
  AddExceptionEmailDate(){
    var i :number = this.exceptionDates.length;
    this.exceptionDates .push({id:i, dateStart: undefined, timeStart: undefined, dateEnd: undefined, timeEnd: undefined});
  }
  private onDateChange(value : Date)
  {
    this.sendTill= value;
  }
  ngOnInit() {
    
  }
}
