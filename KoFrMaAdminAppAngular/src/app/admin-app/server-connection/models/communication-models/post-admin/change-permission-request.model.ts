import { IRequest } from "./i-request.model";
import { ChangePermission } from "../../sql-data/change-permission.model";

export class ChangePermissionRequest implements IRequest
{
    constructor(
        public $type : string,
        public changePermission : ChangePermission
    )
    {

    }
}