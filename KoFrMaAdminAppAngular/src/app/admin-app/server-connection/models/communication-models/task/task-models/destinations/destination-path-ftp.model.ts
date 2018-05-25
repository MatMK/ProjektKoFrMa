import { IDestinationPath } from "../idestination-path.interface";
import { NetworkCredential } from "../../network-credential.model";

export class DestinationPathFTP implements IDestinationPath
{
    /* Adresa FTP serveru a podsložka pro nahrání zálohy */
    public Path : string;
    /* Přístupové údaje k FTP serveru */
    public NetworkCredential : NetworkCredential
}