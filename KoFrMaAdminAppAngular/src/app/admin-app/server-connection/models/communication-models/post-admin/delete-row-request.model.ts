import { IRequest } from "./i-request.model";

export class DeleteRowRequest implements IRequest
{
    
    
    constructor(
        public $type : string,
        public TableName : string,
        public Id : number
    ){}
}