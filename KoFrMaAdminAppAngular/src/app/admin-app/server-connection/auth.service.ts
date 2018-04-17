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
        return this.serverConnectionService.IsAuthorized(false).then(res =>{
                if(res)
                {
                    return true;
                }
                else
                {
                    this.router.navigate(['login']);
                    return false;
                }
            }
        )
    }
  }