import { TaskRepeating } from './task-repeating.model';
import { ScriptInfo } from './script-info.model';
import { NetworkCredential } from './network-credential.model';
import { ISource } from './task-models/isource.interface';
import { IDestination } from './task-models/idestitnation.interface';

export class SetTask {
    public executionTimes: TaskRepeating
    /** Id daemona který má task dokončit */
    public daemonId: number;
    /** Čas, kdy se má úloha spustit (server může předáváat démonu ulohy napřed) */
    public timeToBackup: Date;
    /** Co zálohovat, pokud se jedná o full zálohu je zde path ke složce, pokud je záloha diferenciální/inkrementální je toto pole prázdné */
    public sources: ISource;
    public destinations: IDestination[];
    /**
     * Jaké data chce server vrátit až se dokončí úloha, viz. třída DebugLog (LogOperations)
     *  0 = Don't create log
     * 1 = Fatal errors only that shuts down whole service/program
     * 2 = Errors that cause some process to fail
     * 3 = Errors that program can handle
     * 4 = Basic info about operations that program runs
     * 5 = Debug info that could lead to fixing or optimizing some processes
     * 6 = Tracing info for every process that is likely to fail
     * 7 = Tracing info about everything program does
     * 8 = Tracing info including loop cycles
     * 9 = Tracing info including large loop cycles that will slow down the process a lot
     * 10 = Program will be more like a log writer than actually doing the process
    */
    public logLevel: number;
    public scriptBefore: ScriptInfo
    public scriptAfter: ScriptInfo
    /** Velikost v MB jak velká může v jednu chvíli maximálně být dočasná složka na disku C pokud se zálohuje do archivu nebo na vzdálené úložiště. */
    public temporaryFolderMaxBuffer?: number;
}