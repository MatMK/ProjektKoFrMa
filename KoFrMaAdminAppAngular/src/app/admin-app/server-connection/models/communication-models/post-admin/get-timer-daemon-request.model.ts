import { IRequest } from "./i-request.model";

export class GetTimerDaemonRequest implements IRequest
{
    constructor(
        public $type : string,
        public DaemonId : number
    )
    {
    }
}