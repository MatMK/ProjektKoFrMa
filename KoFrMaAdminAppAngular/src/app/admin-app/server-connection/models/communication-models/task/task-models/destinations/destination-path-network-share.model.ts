import { IDestinationPath } from "../idestination-path.interface";
import { NetworkCredential } from "../../network-credential.model";

export class DestinationPathNetworkShare implements IDestinationPath
{
    /*Cesta ke sdílenému úložišti určeného pro zálohu*/
    public path : string;
    /*Přístupové údaje ke sdílenému úložišti (nepovinné)*/
    public networkCredential : NetworkCredential
}