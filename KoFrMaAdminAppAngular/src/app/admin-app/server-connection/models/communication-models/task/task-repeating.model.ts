import { Time } from "@angular/common";

export class TaskRepeating
{
    public Repeating : Time;
    public RepeatTill : Date;
    public ExecutionTimes : Date[];
    public ExceptionDates : Date[];
}
