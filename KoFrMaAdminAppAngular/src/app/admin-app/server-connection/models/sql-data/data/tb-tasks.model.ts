import { Task } from "../../communication-models/task/task.model";
import { TaskRepeating } from "../../communication-models/task/task-repeating.model";

export class tbTasks
{
    public Id : number;
    public IdDaemon : number;
    public Task : string;
    public TimeOfExecution : Date;
    public RepeatInJSON : string;
    public Completed : boolean;
}