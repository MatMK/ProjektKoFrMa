import { IRequest } from "./i-request.model";

export class ChangePasswordRequest implements IRequest
{
    constructor(
        public $type : string = "ChangePasswordRequest",
        public newPasswordInBase64 : string,
        public targetUsername : string
    )
    {
        
    }
}