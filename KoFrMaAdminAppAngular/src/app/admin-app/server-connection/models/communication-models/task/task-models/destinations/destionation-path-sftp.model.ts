import { IDestinationPath } from "../idestination-path.interface";
import { NetworkCredential } from "../../network-credential.model";

export class DestinationPathSFTP implements IDestinationPath
{
    constructor(public $type : string = "DestinationPathSFTP"){}
    /** SFTP server address and subfolder where the files will be uploaded */
    public Path : string;
    /** Credential to the SFTP server */
    public NetworkCredential : NetworkCredential;
}