import { ISource } from "../isource.interface";
import { NetworkCredential } from "../../network-credential.model";

export class SourceMSSQL implements ISource
{
    /** Address of the MSSQL server from where the backup will be made */
    public ServerName: string;
    /** Credential to the MSSQL server */
    public NetworkCredential: NetworkCredential
    /** Name of the database that will be backuped */
    public DatabaseName: string;
}