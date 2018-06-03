import { FileInfoObject } from './file-info-object.model';
import { FolderObject } from './folder-object.model';
import { ISource } from './../task-models/isource.interface'

export class BackupJournalObject implements ISource
{
    public $type : string = "BackupJournalObject";
    public RelativePath : string;
    public BackupJournalFiles : FileInfoObject[];
    public BackupJournalFolders : FolderObject[];
}