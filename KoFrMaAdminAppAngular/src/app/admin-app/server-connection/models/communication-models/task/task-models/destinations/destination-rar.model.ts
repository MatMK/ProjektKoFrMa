import { IDestination } from "../idestitnation.interface";
import { IDestinationPath } from "../idestination-path.interface";

export class DestinationRar implements IDestination
{
    constructor(public $type : string = "DestinationRar"){}
    /** Path to where the rar archive will be stored */
    public Path: IDestinationPath;
    public CompressionLevel: number;
    /** After what number of MiBs will be the archive split (optional, if not entered or null archive won't be split) */
    public SplitAfter: number;
}