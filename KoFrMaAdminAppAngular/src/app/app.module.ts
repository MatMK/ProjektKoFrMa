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
import { MatSidenavModule, MatToolbarModule, MatListModule, MatTableModule } from '@angular/material'
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterModule, Routes, Router } from '@angular/router';
import { DaemonInfoComponent } from './admin-app/settings/more-info/daemon-info/daemon-info.component';

const routes : Routes = [
  {path: '', redirectTo:'login', pathMatch:'full'},
  {path: 'login', component: LoginComponent},
  {path:':token', children: [
    {path: 'app', component: MainComponent, children: [
      {path:'', redirectTo:'admin-accounts', pathMatch:'full'},
      {path: 'admin-accounts', component: AdminAccountsComponent},
      {path: 'tasks', component: DaemonsComponent},
      {path: 'daemons', component: TasksComponent}
    ]},
    {path: 'daemoninfo/:daemonid', component: DaemonInfoComponent}
  ]},
  {path: '', component: LoginComponent}
]

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    MainComponent,
    DaemonsComponent,
    TasksComponent,
    AdminAccountsComponent,
    DaemonInfoComponent,
  ],
  imports: [
    BrowserModule,
    HttpModule,
    FormsModule, 
    MatSidenavModule,
    MatToolbarModule,
    MatListModule,
    MatTableModule,
    RouterModule.forRoot(routes, {useHash: false}),
    BrowserAnimationsModule
  ],
  providers: [ServerConnectionService, Data],
  bootstrap: [AppComponent]
})
export class AppModule { }
