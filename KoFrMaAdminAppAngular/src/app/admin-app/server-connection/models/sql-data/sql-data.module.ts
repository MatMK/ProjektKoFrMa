import { tbAdminAccounts } from './data/tb-admin-accounts.module';
import { tbTasks } from './data/tb-tasks.module';
import { tbDaemons } from './data/tb-daemons.module';

export class SqlData
{
    public AdminAccounts : tbAdminAccounts;
    public Tasks : tbTasks;
    public Daemons : tbDaemons;
}