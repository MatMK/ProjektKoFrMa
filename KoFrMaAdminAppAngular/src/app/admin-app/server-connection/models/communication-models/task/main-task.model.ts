import { Task } from "./task.model";
import { TaskRepeating } from "./task-repeating.model";

export class MainTask
{
    public Id : number;
    public IdDaemon : number;
    public Task : Task;
    public TimeOfExecution : Date;
    public RepeatInJSON : TaskRepeating;
    public Completed : boolean;
}