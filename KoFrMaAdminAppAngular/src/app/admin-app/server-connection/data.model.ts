import { AdminInfo } from "./models/communication-models/admin-info.model";

export class Data
{
    public LoggedIn : boolean = false;
    public ServerRootURL : string = 'http://localhost:49849/';
    public Loading : boolean = false;
    public adminInfo : AdminInfo = new AdminInfo();
}