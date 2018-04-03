import { AdminInfo } from "./models/communication-models/admin-info.model";
import { tbAdminAccounts } from "./models/sql-data/data/tb-admin-accounts.model";
import { tbDaemons } from "./models/sql-data/data/tb-daemons.model";
import { MainTask } from "./models/communication-models/task/main-task.model";

export class Data
{
    public LoggedIn : boolean = false;
    public ServerRootURL : string;
    public Loading : boolean = false;
    public adminInfo : AdminInfo = new AdminInfo();
    public AdminAccounts : tbAdminAccounts[];
    public Daemons : tbDaemons[];
    public Tasks : MainTask[];
    public ErrorMessage : string;
}