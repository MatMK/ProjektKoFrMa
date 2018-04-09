import { AdminInfo } from "./models/communication-models/admin-info.model";
import { tbAdminAccounts } from "./models/sql-data/data/tb-admin-accounts.model";
import { tbDaemons } from "./models/sql-data/data/tb-daemons.model";
import { MainTask } from "./models/communication-models/task/main-task.model";
import { MatTableDataSource } from "@angular/material";

export class Data
{
    public ServerRootURL : string;
    public Loading : boolean = false;
    public adminInfo : AdminInfo = new AdminInfo();
    public AdminAccounts : MatTableDataSource<tbAdminAccounts> = new MatTableDataSource<tbAdminAccounts>();
    public Daemons : MatTableDataSource<tbDaemons> = new MatTableDataSource<tbDaemons>();
    public Tasks : MatTableDataSource<MainTask> = new MatTableDataSource<MainTask>();
    public ErrorMessage : string;
}