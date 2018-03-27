import { CanActivate } from '@angular/router';
import { Injectable } from '@angular/core';
import { ServerConnectionService } from './server-connection.service';
import {Router} from "@angular/router";

@Injectable()

export class AuthGuard implements CanActivate {
    constructor(private serverConnectionService : ServerConnectionService, private router : Router) {}
    canActivate() {
        return this.serverConnectionService.IsAuthorized().then(res =>{
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