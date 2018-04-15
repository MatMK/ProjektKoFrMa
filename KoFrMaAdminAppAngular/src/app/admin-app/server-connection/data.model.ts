import { AdminInfo } from "./models/communication-models/admin-info.model";
import { tbAdminAccounts } from "./models/sql-data/data/tb-admin-accounts.model";
import { tbDaemons } from "./models/sql-data/data/tb-daemons.model";
import { MainTask } from "./models/communication-models/task/main-task.model";
import { MatTableDataSource } from "@angular/material";
import { AdminData } from "./models/sql-data/admin-data.model";

export class Data
{
    public ServerRootURL : string;
    public Loading : boolean = false;
    public adminInfo : AdminInfo = new AdminInfo();
    public Data : AdminData = new AdminData();
    public ErrorMessage : string;
    public Permissions : PermInterface[] = [{name:'Add Admins', number:1}, {name: 'Add Tasks', number:2}];
}