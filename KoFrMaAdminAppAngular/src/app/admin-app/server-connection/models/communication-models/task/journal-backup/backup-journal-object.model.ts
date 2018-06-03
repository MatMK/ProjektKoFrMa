import { FileInfoObject } from './file-info-object.model';
import { FolderObject } from './folder-object.model';
import { ISource } from './../task-models/isource.interface'

export class BackupJournalObject implements ISource
{
    public $type : string = "BackupJournalObject";
		/** Path(s) to the backup source(s) */
		relativePaths: string[];
		/** List of files that were backuped and information about them to make it easy to compare to future backup */
		backupJournalFiles: FileInfoObject[];
		/** List of files were deleted since last time and that need to be deleted before the recovery (only applies to differential/incremental backup) */
		backupJournalFilesDelete: string[];
		/** List of folders (only folder structure without files) that were backuped along with some attributes for future backup */
		backupJournalFolders: FolderObject[];
		/** List of folders (only folder structure without files) that were deleted since last time and that need to be deleted before the recovery (only applies to differential/incremental backup) */
		backupJournalFoldersDelete: string[];
}