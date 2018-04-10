import { IRequest } from "./i-request.model";
import { SetTask } from "../task/set-task.model";

export class SetTasksRequest implements IRequest
{
    constructor(
        public $type : string,
        public setTasks : SetTask[],
    )
    {

    }
}