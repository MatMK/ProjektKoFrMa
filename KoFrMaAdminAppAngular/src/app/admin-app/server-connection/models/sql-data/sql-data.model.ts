import { tbAdminAccounts } from './data/tb-admin-accounts.model';
import { tbTasks } from './data/tb-tasks.model';
import { tbDaemons } from './data/tb-daemons.model';

export class SqlData
{
    public AdminAccounts : tbAdminAccounts[];
    public Tasks : tbTasks[];
    public Daemons : tbDaemons[];
}