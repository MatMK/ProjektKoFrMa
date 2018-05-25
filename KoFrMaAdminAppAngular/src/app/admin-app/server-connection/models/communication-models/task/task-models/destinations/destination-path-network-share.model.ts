import { IDestinationPath } from "../idestination-path.interface";
import { NetworkCredential } from "../../network-credential.model";

export class DestinationPathNetworkShare implements IDestinationPath
{
    /*Cesta ke sdílenému úložišti určeného pro zálohu*/
    public Path : string;
    /*Přístupové údaje ke sdílenému úložišti (nepovinné)*/
    public NetworkCredential : NetworkCredential
}