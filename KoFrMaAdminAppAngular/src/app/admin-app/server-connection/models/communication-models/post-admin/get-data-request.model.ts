import { IRequest } from "./i-request.model";

export class GetDataRequest implements IRequest
{
    constructor(
        public $type : string,
        public getData : number[],
    )
    {

    }
}