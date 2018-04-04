import { FileInfoObject } from './file-info-object.model';
import { FolderObject } from './folder-object.model';


export class BackupJournalObject
{
    public RelativePath : string;
    public BackupJournalFiles : FileInfoObject[];
    public BackupJournalFolders : FolderObject[];
}