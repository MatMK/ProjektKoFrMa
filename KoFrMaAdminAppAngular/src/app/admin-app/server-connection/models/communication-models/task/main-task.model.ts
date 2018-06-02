import { Task } from "./task.model";
import { TaskRepeatingNoTimespan } from "./task-repeating.model";

export class MainTask
{
    public Id : number;
    public IdDaemon : number;
    public Task : Task;
    public TimeOfExecution : Date;
    public RepeatInJSON : TaskRepeatingNoTimespan;
    public Completed : boolean;
}