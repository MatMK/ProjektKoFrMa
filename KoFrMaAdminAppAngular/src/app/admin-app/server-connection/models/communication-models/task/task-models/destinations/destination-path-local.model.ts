import { IDestinationPath } from "../idestination-path.interface";
import { NetworkCredential } from "../../network-credential.model";

export class DestinationPathLocal implements IDestinationPath
{
    constructor(public $type : string = "DestinationPathLocal"){}
    /* Cesta k lokální složce*/
    public Path : string;
}