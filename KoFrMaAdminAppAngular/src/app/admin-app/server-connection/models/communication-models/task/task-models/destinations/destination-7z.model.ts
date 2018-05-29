import { IDestinationPath } from "../idestination-path.interface";
import { IDestination } from "../idestitnation.interface";

export class Destination7z implements IDestination
{
    /** Path to where the 7z archive will be stored */
    public Path: IDestinationPath;
    public CompressionLevel: number;
    /** After what number of MiBs will be the archive split (optional, if not entered or null archive won't be split) */
    public SplitAfter: number;
}