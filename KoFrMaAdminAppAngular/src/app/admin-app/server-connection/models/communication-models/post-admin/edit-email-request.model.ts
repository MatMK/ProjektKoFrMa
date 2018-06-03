import { TaskRepeatingNoTimespan } from "../task/task-repeating.model";

export class EditEmailRequest
{
    constructor(){}
    public $type : string = "EditEmailRequest"
    public RecieveMail: boolean;
    public Repeating: TaskRepeatingNoTimespan;
}