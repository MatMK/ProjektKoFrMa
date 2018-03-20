import { NgModule }      from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { Http, Response, RequestOptions, Headers, HttpModule, URLSearchParams } from '@angular/http';
import { HttpClient, HttpParams } from '@angular/common/http'

import { AppComponent }  from './app.component';

@NgModule({
  imports:      [ BrowserModule, HttpModule ],
  declarations: [ AppComponent ],
  bootstrap:    [ AppComponent ]
})
export class AppModule { }
