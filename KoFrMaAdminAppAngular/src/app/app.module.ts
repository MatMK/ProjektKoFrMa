import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpModule} from '@angular/http';
import { FormsModule } from '@angular/forms';

import { AppComponent } from './app.component';
import { ServerConnectionService } from './admin-app/server-connection/server-connection.service';
import { LoginComponent } from './admin-app/login/login.component';
import { Data } from './admin-app/server-connection/data.model';
import { MainComponent } from './admin-app/settings/main/main.component';
import { DaemonsComponent } from './admin-app/settings/tabs/daemons/daemons.component';
import { TasksComponent } from './admin-app/settings/tabs/tasks/tasks.component';
import { AdminAccountsComponent } from './admin-app/settings/tabs/admin-accounts/admin-accounts.component';
import { MatSidenavModule, MatToolbarModule, MatListModule } from '@angular/material'
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    MainComponent,
    DaemonsComponent,
    TasksComponent,
    AdminAccountsComponent,
  ],
  imports: [
    BrowserModule,
    HttpModule,
    FormsModule, 
    MatSidenavModule,
    MatToolbarModule,
    MatListModule,
    BrowserAnimationsModule
  ],
  providers: [ServerConnectionService, Data],
  bootstrap: [AppComponent]
})
export class AppModule { }
