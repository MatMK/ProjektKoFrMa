import { MatTableDataSource } from "@angular/material";
import { tbDaemons } from "./data/tb-daemons.model";
import { tbAdminAccounts } from "./data/tb-admin-accounts.model";
import { MainTask } from "../communication-models/task/main-task.model";

export class AdminData
{
    public tbAdminAccounts : MatTableDataSource<tbAdminAccounts> = new MatTableDataSource<tbAdminAccounts>();
    public tbDaemons : MatTableDataSource<tbDaemons> = new MatTableDataSource<tbDaemons>();
    public tbTasks : MatTableDataSource<MainTask> = new MatTableDataSource<MainTask>();
}