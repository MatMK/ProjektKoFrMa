import { tbAdminAccounts } from './data/tb-admin-accounts.model';
import { tbTasks } from './data/tb-tasks.model';
import { tbDaemons } from './data/tb-daemons.model';
import { tbCompletedTasks } from './data/tb-completed-tasks';
import { tbServerExceptions } from './data/tb-server-exceptions.model';

export class SqlData
{
    public tbAdminAccounts : tbAdminAccounts[];
    public tbTasks : tbTasks[];
    public tbDaemons : tbDaemons[];
    public tbTasksCompleted : tbCompletedTasks[];
    public tbServerExceptions : tbServerExceptions[];
}