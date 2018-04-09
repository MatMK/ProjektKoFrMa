import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-add-admin',
  templateUrl: './add-admin.component.html',
  styleUrls: ['./add-admin.component.css']
})
export class AddAdminComponent implements OnInit {
  private username : string;
  private password : string;
  private email : string;
  private enabled : boolean;
  private permission : number[];
  private Permissions : PermInterface[] = [{name:'Add Admins', number:1}, {name: 'Add Tasks', number:2}]
  constructor() { }

  ngOnInit() {
  }

}

interface PermInterface
{
  name : string;
  number : number;
}