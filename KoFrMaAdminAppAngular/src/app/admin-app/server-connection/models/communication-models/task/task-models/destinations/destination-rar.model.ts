import { IDestination } from "../idestitnation.interface";

export class DestinationRar implements IDestination
{
    /** Path to where the rar archive will be stored */
    public Path: any;
    public CompressionLevel: any;
    /** After what number of MiBs will be the archive split (optional, if not entered or null archive won't be split) */
    public SplitAfter: number;
}