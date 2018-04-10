import { IRequest } from "./i-request.model";
import { AddAdmin } from "../add-admin.model";

export class AddAdminRequest implements IRequest
{
    constructor(
        public $type : string,
        public addAdmin : AddAdmin,
    )
    {

    }
}