import { tbAdminAccounts } from './data/tb-admin-accounts.model';
import { tbTasks } from './data/tb-tasks.model';
import { tbDaemons } from './data/tb-daemons.model';

export class SqlData
{
    public tbAdminAccounts : tbAdminAccounts[];
    public tbTasks : tbTasks[];
    public tbDaemons : tbDaemons[];
}