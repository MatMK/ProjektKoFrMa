import { ISource } from "../isource.interface";
import { NetworkCredential } from "../../network-credential.model";

export class SourceMySQL implements ISource
{
	constructor(public $type : string = "SourceMySQL") {}
	/** Address of the MySQL server from where the backup will be made */
	public ServerName: string;
	/** Credential to the MySQL server */
	public NetworkCredential: NetworkCredential
	/** Name of the database that will be backuped */
	public DatabaseName: string;
}