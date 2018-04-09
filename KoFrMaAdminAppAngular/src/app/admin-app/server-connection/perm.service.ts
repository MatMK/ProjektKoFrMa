import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Injectable } from '@angular/core';
import { ServerConnectionService } from './server-connection.service';
import { Router } from "@angular/router";
import { Data } from './data.model';

@Injectable()


export class PermissionGuard implements CanActivate
{
    constructor(private serverConnectionService : ServerConnectionService, private router : Router, private data : Data) {}
    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot)
    {
        /*this is only for development DONT FORGET TO GET RID OF IT LATER!*/
        return true;
        /*^^^^^^^^^^^^*/
        /*
        let permissions : number[] =  route.data["roles"]
        return this.serverConnectionService.HasPermission(permissions).then(res => {
            if(res)
                return true
            this.data.ErrorMessage = "Excess denied, permission ";
            permissions.forEach(element => {
                this.data.ErrorMessage += element + " ";
            });
            this.data.ErrorMessage = "required";
            alert(this.data.ErrorMessage);
            return false;
        })*/
    }
}