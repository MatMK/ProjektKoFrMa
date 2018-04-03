import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Injectable } from '@angular/core';
import { ServerConnectionService } from './server-connection.service';
import { Router } from '@angular/router';
import { Data } from './data.model'


@Injectable()

export class AuthGuard implements CanActivate 
{
    constructor(private serverConnectionService : ServerConnectionService, private router : Router, private data : Data) {}
    canActivate() {
        if(this.data.LoggedIn)
        {
            return this.serverConnectionService.IsAuthorized().then(res =>{
                    if(res)
                    {
                        return true;
                    }
                }
            )
        }
        this.router.navigate(['login']);
        return false;
    }
  }