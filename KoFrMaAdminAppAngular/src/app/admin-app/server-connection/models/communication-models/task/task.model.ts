import { BackupJournalObject } from './journal-backup/backup-journal-object.model';
import { NetworkCredential } from './network-credential.model'
import { ScriptInfo } from './script-info.model';
import { ISource } from './task-models/isource.interface';
import { IDestination } from './task-models/idestitnation.interface';

export class Task
{
    iDTask: number;
    /** Čas, kdy se má úloha spustit (server může předáváat démonu ulohy napřed) */
    timeToBackup: Date;
    /** Co zálohovat, pokud se jedná o full zálohu je zde path ke složce, pokud je záloha diferenciální/inkrementální je toto pole prázdné */
    sources: ISource;
    /** Pokud se jedná o diferenciální/inkrementální zálohu, je zde kompletní log zálohy na kterou je potřeba navázat */
    destinations: IDestination[];
    /** Jaké data chce server vrátit až se dokončí úloha, viz. třída DebugLog (LogOperations)0 = Don't create log1 = Fatal errors only that shuts down whole service/program2 = Errors that cause some process to fail3 = Errors that program can handle4 = Basic info about operations that program runs5 = Debug info that could lead to fixing or optimizing some processes6 = Tracing info for every process that is likely to fail7 = Tracing info about everything program does8 = Tracing info including loop cycles9 = Tracing info including large loop cycles that will slow down the process a lot10 = Program will be more like a log writer than actually doing the process */
    logLevel: number;
    /** Určuje, jestli úloha právě probíhá aby timer nespustil stejnou úlohu několikrát */
    inProgress: boolean;
    /** Script, který se spustí před začátkem zálohy */
    scriptBefore: ScriptInfo;
    /** Script, který se spustí po záloze */
    scriptAfter: ScriptInfo;
    /** Velikost v MB jak velká může v jednu chvíli maximálně být dočasná složka na disku C pokud se zálohuje do archivu nebo na vzdálené úložiště. */
    temporaryFolderMaxBuffer?: number;
}