import { BrowserModule } from '@angular/platform-browser';
import { NgModule, Component } from '@angular/core';
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
import { MatSidenavModule, MatToolbarModule, MatListModule, MatTableModule, MatPaginator, MatTableDataSource, MatPaginatorModule, MatSelectModule } from '@angular/material'
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterModule, Routes, Router, CanActivate,RouterLink } from '@angular/router';
import { DaemonInfoComponent } from './admin-app/settings/more-info/daemon-info/daemon-info.component';
import { AuthGuard } from './admin-app/server-connection/auth.service';
import { PermissionGuard } from './admin-app/server-connection/perm.service';
import { MatRadioModule, MatRadioButton } from '@angular/material/radio';
import { NotFoundComponent } from './admin-app/not-found/not-found.component';
import { ToolbarComponent } from './admin-app/settings/toolbar/toolbar.component';
import { AddTaskComponent } from './admin-app/settings/more-info/daemon-info/add-task/add-task.component';

const routes : Routes = [
  {path: '', redirectTo:'login', pathMatch:'full'},
  {path: 'login', component: LoginComponent},
  {path: 'backup', component: ToolbarComponent, canActivate: [AuthGuard], children : [
    {path: 'app', component: MainComponent, children: [
      {path:'', redirectTo:'tasks', pathMatch:'full'},
      {path: 'admin-accounts', component: AdminAccountsComponent},
      {path: 'tasks', component: TasksComponent},
      {path: 'daemons', component: DaemonsComponent},
      ]},
    {path: 'daemoninfo/:daemonid', component: DaemonInfoComponent},
    {path: 'add-task/:daemonid', component:AddTaskComponent}
  ]},
  {path: ':unknown', component: NotFoundComponent}
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
    NotFoundComponent,
    ToolbarComponent,
    AddTaskComponent
  ],
  imports: [
    BrowserModule,
    HttpModule,
    FormsModule, 
    MatSidenavModule,
    MatToolbarModule,
    MatListModule,
    MatTableModule,
    MatRadioModule,
    MatPaginatorModule,
    MatSelectModule,
    RouterModule.forRoot(
        routes,
       {useHash: false},
      ),
    RouterModule.forRoot(routes, {useHash: false}),
    BrowserAnimationsModule
    
  ],
  providers: [ServerConnectionService, Data, AuthGuard, PermissionGuard ],
  bootstrap: [AppComponent]
})
export class AppModule { }
