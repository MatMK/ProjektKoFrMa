import { Component, OnInit } from '@angular/core';

import { ServerConnectionService } from '../../../../server-connection/server-connection.service';
import { AddAdmin } from '../../../../server-connection/models/communication-models/add-admin.model';
import { Router } from '@angular/router';
import { Data } from '../../../../server-connection/data.model';
import { InputCheck } from '../../../../server-connection/input-check.service';

@Component({
  selector: 'app-add-admin',
  templateUrl: './add-admin.component.html',
  styleUrls: ['./add-admin.component.css']
})
export class AddAdminComponent implements OnInit {
  private username : string;
  private password : string;
  private email : string;
  private enabled : boolean = true;
  private selectedPermission : number[] = [];
  private Permissions = this.data.Permissions;
  private check : InputCheck = new InputCheck();

  onAreaListControlChanged(list){
    try{
      list.selectedPermission.selected.map(item => item.number);
    }
    catch{}
  }
  addAdmin()
  {
    if(!this.check.username(this.username))
    {
      return;
    }
    if(!this.check.password(this.password))
    {
      return;
    }
    if(!this.check.email(this.email))
    {
      return;
    }
    let admin : AddAdmin = new AddAdmin()
    admin.Username = this.username;
    admin.Password = btoa(this.password);
    admin.Email = this.email;
    admin.Enabled = this.enabled;
    admin.Permissions = this.selectedPermission;
    this.serverConnection.AddAdmin(admin).then(res =>
    {
      if(res)
      {
        this.serverConnection.RefreshData([1]);
        this.router.navigate(['backup', 'app', 'admin-accounts']);
      }
      else
        alert('Something went wrong');
    })
  }
  constructor(private serverConnection : ServerConnectionService, private router : Router, private data : Data) { }
  ngOnInit() {
  }

}