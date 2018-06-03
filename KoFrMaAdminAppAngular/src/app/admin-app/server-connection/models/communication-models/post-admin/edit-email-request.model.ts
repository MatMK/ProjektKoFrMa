import { TaskRepeatingNoTimespan } from "../task/task-repeating.model";
import { IRequest } from "./i-request.model";

export class EditEmailRequest implements IRequest
{
    constructor(    
        public $type : string,
        public RecieveMail: boolean,
        public Repeating: TaskRepeatingNoTimespan,
    ){}

}