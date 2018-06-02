import { Component, OnInit } from '@angular/core';
import { Time } from '@angular/common';

@Component({
  selector: 'app-email-info',
  templateUrl: './email-info.component.html',
  styleUrls: ['./email-info.component.css']
})
export class EmailInfoComponent implements OnInit {

  //Emails
  private send : string = "Minute";
  private sendEvery : number;
  private executionDates : {id : number, date: Date, time: Time}[] = [];
  private exceptionDates : {id : number, date: Date, time: Time}[] = [];
  private sendTill : Date;

  constructor() { }

  
  AddSendDate()
  {
    var i :number = this.executionDates.length;
    this.executionDates.push({id:i, date: undefined, time: undefined});
  }
  RemoveSendDate(id : number)
  {
    delete this.executionDates[id];
  }
  RemoveExceptionEmailDate(id: number)
  {
    delete this.exceptionDates[id];
  }
  AddExceptionEmailDate(){
    var i :number = this.exceptionDates.length;
    this.exceptionDates .push({id:i, date: undefined, time: undefined});
  }
  ngOnInit() {
  }
}
