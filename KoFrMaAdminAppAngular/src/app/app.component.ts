import { Component } from '@angular/core';
import { Data } from './admin-app/server-connection/data.model';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  constructor(private data : Data){}
  
  private title = 'app';
  public Token : string;
}