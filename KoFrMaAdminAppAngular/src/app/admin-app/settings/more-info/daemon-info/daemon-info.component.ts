import { AppComponent } from './../../../../app.component';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from "@angular/router";
import { MatRadioModule } from '@angular/material/radio';

@Component({
  selector: 'app-daemon-info',
  templateUrl: './daemon-info.component.html',
  styleUrls: ['./daemon-info.component.css']
})
export class DaemonInfoComponent implements OnInit {

  private sourcepath : string;
  private destinationpath : string;
  private backup : string;
  constructor(private activeRoute:ActivatedRoute) {
    this.activeRoute.params.subscribe(params => {
      this.daemonId = params.daemonid;
      //check if it is a number
      if (!(this.daemonId >= 0 || this.daemonId<0))
        this.daemonId = undefined;
    });
   }
  private daemonId : number;
  test()
  {
    console.log(this.backup);
    console.log(this.sourcepath);
    console.log(this.destinationpath);
  }
  ngOnInit() {
  }

}
