import { AdminInfo } from './admin-info.model';
import { SetTask } from './task/set-task.model';
import { AddAdmin } from './add-admin.model';
export class PostAdmin
{
    constructor(   
    public adminInfo : AdminInfo,
    public setTasks : SetTask[],
    public getData : number[],
    public addAdmin : AddAdmin
    ){}
}