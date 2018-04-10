import { TaskRepeating } from './task-repeating.model';
import { ScriptInfo } from './script-info.model';
import { NetworkCredential } from './network-credential.model';

export class SetTask
{
    public ExecutionTimes : TaskRepeating;
    public DaemonId : number;
    public TimeToBackup : Date;
    public SourceOfBackup : string;
    public WhereToBackup : string[];
    public TimerValue : number;
    public LogLevel : number;
    public CompressionLevel : number;
    public NetworkCredentials : NetworkCredential;
    public InProgress : boolean;
    public ScriptBefore : ScriptInfo;
    public ScriptAfter : ScriptInfo;
    public TemporaryFolderMaxBuffer : number; 

    public CompressionType : string;
}