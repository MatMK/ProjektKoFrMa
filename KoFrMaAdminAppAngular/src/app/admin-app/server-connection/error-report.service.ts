import {ErrorHandler, Injectable, ComponentFactoryResolver} from "@angular/core";
import { Router } from "@angular/router";


@Injectable()

export class ErrorReport implements ErrorHandler
{

    constructor(private router : Router) { }

    handleError(error : any)
    {
        switch(error.status)
        {
            case 0 : 
                this.showError("No connection");
                break;
            case 400 :
                alert(JSON.parse(error._body).Message);
                break;
            case 401 :
                this.showError("Unauthorized");
                this.router.navigate(["login"]);
                break;
            case 403 : 
                this.showError("Insufficient permission");
                break;
            default :
                this.showError("Oops, something went wrong");
        }
    }
    showError(message : string)
    {
        //replace ↓↓ with something nicer
        alert(message);
    }
}