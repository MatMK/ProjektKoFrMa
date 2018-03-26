import { BackupJournalObject } from './BackupJournal/backup-journal-object.model';
import { NetworkCredential } from './network-credential.model'
import { ScriptInfo } from './script-info.model';

export class Task
{
    public IDTask : number;
    public TimeToBackup : Date;
    public SourceOfBackup : string;
    public BackupJournalSource : BackupJournalObject;
    public WhereToBackup : string[];
    public TimerValue : number;
    public LogLevel : number;
    public CompressionLevel : number;
    public NetworkCredentials : NetworkCredential;
    public InProgress : boolean;
    public ScriptBefore : ScriptInfo;
    public ScriptAfter : ScriptInfo;
    public TemporaryFolderMaxBuffer : number;
}