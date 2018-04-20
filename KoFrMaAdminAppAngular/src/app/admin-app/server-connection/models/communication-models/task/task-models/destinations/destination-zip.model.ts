import { IDestination } from "../idestitnation.interface";
import { IDestinationPath } from "../idestination-path.interface";

export class DestinationZip implements IDestination
{
    public path : IDestinationPath;
    public CompressionLevel : number;
}