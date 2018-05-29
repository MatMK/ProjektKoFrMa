import { IDestinationPath } from "./idestination-path.interface";

export interface IDestination
{
    /** Defines the destination of the backup, it can be archive (ZIP/RAR/7Z) or plain folder */
    Path : IDestinationPath;
}