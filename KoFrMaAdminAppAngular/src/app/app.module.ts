import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpModule} from '@angular/http';

import { AppComponent } from './app.component';
import { ServerConnectionService } from './admin-app/server-connection/server-connection.service';
import { LoginComponent } from './admin-app/login/login.component';


@NgModule({
  declarations: [
    AppComponent,
    LoginComponent
  ],
  imports: [
    BrowserModule,
    HttpModule
  ],
  providers: [ServerConnectionService, AppComponent],
  bootstrap: [AppComponent]
})
export class AppModule { }
