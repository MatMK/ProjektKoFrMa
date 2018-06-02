import { IDestination } from "../idestitnation.interface";
import { IDestinationPath } from "../idestination-path.interface";

export class DestinationPlain implements IDestination
{
    constructor(public $type : string = "DestinationPlain"){}
    /*Nastavení cesty, kde bude umístěna záloha jako obyčejná složka*/
    public Path : IDestinationPath;
}