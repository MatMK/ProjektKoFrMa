import { Component, OnInit } from '@angular/core';

import { ServerConnectionService } from '../../../../server-connection/server-connection.service';
import { AddAdmin } from '../../../../server-connection/models/communication-models/add-admin.model';
import { Router } from '@angular/router';

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
  private Permissions : PermInterface[] = [{name:'Add Admins', number:1}, {name: 'Add Tasks', number:2}]

  onAreaListControlChanged(list){
    try{
      list.selectedPermission.selected.map(item => item.number);
    }
    catch{}
  }
  addAdmin()
  {
    if(this.username == null || this.username == undefined)
    {
      alert("Invalid Username");
      return
    }
    if(this.password == undefined || this.password.length < 6)
    {
      alert("Password has to be at least 6 characters long");
      return
    }
    let regex = new RegExp(/^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$/)
    if(!regex.test(this.email))
    {
      alert("Invalid Email");
      return
    }
    let admin : AddAdmin = new AddAdmin()
    admin.Username = this.username;
    admin.Password = this.password;
    admin.Email = this.email;
    admin.Enabled = this.enabled;
    admin.Permissions = this.selectedPermission;
    this.serverConnection.AddAdmin(admin).then(res =>
    {
      if(res)
        this.router.navigate(['backup', 'app', 'admin-accounts']);
      else
        alert('Something went wrong');
    })
  }
  constructor(private serverConnection : ServerConnectionService, private router : Router) { }
  ngOnInit() {
  }

}

interface PermInterface
{
  name : string;
  number : number;
}