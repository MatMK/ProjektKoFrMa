import { IDestinationPath } from "../idestination-path.interface";
import { NetworkCredential } from "../../network-credential.model";

export class DestinationPathSFTP implements IDestinationPath
{
    public path : string;
    public networkCredential : NetworkCredential
}