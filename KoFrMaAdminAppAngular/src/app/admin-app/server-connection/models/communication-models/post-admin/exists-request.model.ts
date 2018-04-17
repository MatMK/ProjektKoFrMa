import { IRequest } from "./i-request.model";

export class ExistsRequest implements IRequest
{
    constructor(
        public $type : string,
        public TableName : string,
        public Value : string,
        public Column : string
    ){}
}