import { IDestination } from "../idestitnation.interface";

export class DestinationRar implements IDestination
{
    /** Nastavení cesty, kde bude výsledný rar */
    public path: IDestination;
    public compressionLevel: number;
}