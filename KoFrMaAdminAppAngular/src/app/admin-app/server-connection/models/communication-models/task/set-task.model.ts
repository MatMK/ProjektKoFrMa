import { TaskRepeating } from './task-repeating.model';
import { ScriptInfo } from './script-info.model';
import { NetworkCredential } from './network-credential.model';
import { ISource } from './task-models/isource.interface';
import { IDestination } from './task-models/idestitnation.interface';

export class SetTask {
    /** Repeating of task */
    ExecutionTimes: TaskRepeating
    /** Database id if the daemon */
    DaemonId: number;
    /** Source of backup */
    Sources: ISource;
    /** List of backup destinations */
    Destinations: IDestination[];
    /** What data will be saved to log0 = Don't create log1 = Fatal errors only that shuts down whole service/program2 = Errors that cause some process to fail3 = Errors that program can handle4 = Basic info about operations that program runs5 = Debug info that could lead to fixing or optimizing some processes6 = Tracing info for every process that is likely to fail7 = Tracing info about everything program does8 = Tracing info including loop cycles9 = Tracing info including large loop cycles that will slow down the process a lot10 = Program will be more like a log writer than actually doing the process */
    LogLevel: number;
    /** Script that will be run BEFORE the task */
    ScriptBefore: ScriptInfo
    /** Script that will be run AFTER the task */
    ScriptAfter : ScriptInfo
    /** Size in MB how big can at one moment be temporary folder on drive C, is used only when destination is archive and/or is remote */
    TemporaryFolderMaxBuffer?: number;
}