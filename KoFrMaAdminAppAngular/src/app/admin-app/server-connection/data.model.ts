import { AdminInfo } from "./models/communication-models/admin-info.model";
import { tbAdminAccounts } from "./models/sql-data/data/tb-admin-accounts.model";

export class Data
{
    public LoggedIn : boolean = true;
    public ServerRootURL : string = 'http://localhost:49849/';
    public Loading : boolean = false;
    public adminInfo : AdminInfo = new AdminInfo();
    public AdminAccounts : tbAdminAccounts[];
}