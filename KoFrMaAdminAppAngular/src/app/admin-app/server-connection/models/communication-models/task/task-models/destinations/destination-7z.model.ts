import { IDestinationPath } from "../idestination-path.interface";
import { IDestination } from "../idestitnation.interface";

export class Destination7z implements IDestination
{
    /* Adresa FTP serveru a podsložka pro nahrání zálohy */
    public path : IDestination;
    /* Přístupové údaje k FTP serveru */
    public compressionLevel : number;
}