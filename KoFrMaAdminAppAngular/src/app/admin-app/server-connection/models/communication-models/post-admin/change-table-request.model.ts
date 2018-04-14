import { IRequest } from "./i-request.model";
import { ChangeTable } from "../../sql-data/change-table.model";

export class ChangeTableRequest implements IRequest
{
    constructor(
        public $type : string,
        public changeTable : ChangeTable
    )
    {

    }
}