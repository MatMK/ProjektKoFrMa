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
import { MatSidenavModule, MatToolbarModule, MatListModule, MatTableModule, 
  MatPaginator, MatTableDataSource, MatPaginatorModule, MatSelectModule,
  MatCheckboxModule} from '@angular/material'
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterModule, Routes, Router, CanActivate,RouterLink, RouterLinkActive } from '@angular/router';
import { DaemonInfoComponent } from './admin-app/settings/more-info/daemon-info/daemon-info.component';
import { AuthGuard } from './admin-app/server-connection/auth.service';
import { PermissionGuard } from './admin-app/server-connection/perm.service';
import { MatRadioModule, MatRadioButton } from '@angular/material/radio';
import { NotFoundComponent } from './admin-app/not-found/not-found.component';
import { ToolbarComponent } from './admin-app/settings/toolbar/toolbar.component';
import { AddTaskComponent } from './admin-app/settings/more-info/daemon-info/add-task/add-task.component';
import { AddAdminComponent } from './admin-app/settings/tabs/admin-accounts/add-admin/add-admin.component';
import { ErrorReport } from './admin-app/server-connection/error-report.service';
import { ToastsManager } from 'ng2-toastr';
import { ServerExceptionsComponent } from './admin-app/settings/tabs/server-exceptions/server-exceptions.component';
import { CompletedTasksComponent } from './admin-app/settings/tabs/completed-tasks/completed-tasks.component';
import { CompletedTaskInfoComponent } from './admin-app/settings/more-info/completed-task-info/completed-task-info.component';
import { ExceptionInfoComponent } from './admin-app/settings/more-info/exception-info/exception-info.component';

const routes : Routes = [
  {path: '', redirectTo:'login', pathMatch:'full'},
  {path: 'login', component: LoginComponent},
  {path: 'backup', component: ToolbarComponent, canActivate: [AuthGuard], children : [
    {path: 'app', component: MainComponent, children: [
      {path:'', redirectTo:'tasks', pathMatch:'full'},
      {path: 'admin-accounts', component: AdminAccountsComponent},
      {path: 'tasks', component: TasksComponent},
      {path: 'daemons', component: DaemonsComponent},
      {path: 'server-exceptions', component: ServerExceptionsComponent},
      {path: 'completed-tasks', component: CompletedTasksComponent},
      ]},
    {path: 'daemoninfo/:daemonid', component: DaemonInfoComponent},
    {path: 'add-task/:daemonid', component:AddTaskComponent},
    {path: 'completed-task-info/:completedTaskId', component:CompletedTaskInfoComponent},
    {path: 'add-admin', component: AddAdminComponent, canActivate: [PermissionGuard], data: {roles: [1]}},
  ]},
  {path: '**', component: NotFoundComponent}
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
    AddTaskComponent,
    AddAdminComponent,
    ServerExceptionsComponent,
    CompletedTasksComponent,
    CompletedTaskInfoComponent,
    ExceptionInfoComponent

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
    MatCheckboxModule,
    RouterModule.forRoot(
        routes,
       {useHash: false},
      ),
    RouterModule.forRoot(routes, {useHash: false}),
    BrowserAnimationsModule,
  ],
  providers: [ServerConnectionService, Data, AuthGuard, PermissionGuard, ErrorReport ],
  bootstrap: [AppComponent],

})
export class AppModule { }
