import { IDestination } from "../idestitnation.interface";
import { IDestinationPath } from "../idestination-path.interface";

export class DestinationZip implements IDestination
{
    constructor(public $type : string = "DestinationZip"){}
    /** Path to where the zip archive will be stored */
		public Path: IDestinationPath;
		public CompressionLevel: number;
		/** After what number of MiBs will be the archive split (optional, if not entered or null archive won't be split) */
		public SplitAfter: number;
}