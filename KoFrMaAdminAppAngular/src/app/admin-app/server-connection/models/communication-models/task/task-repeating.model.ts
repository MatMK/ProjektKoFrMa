import { Time } from "@angular/common";
import { ExceptionDate } from "./exception-date.model";

export class TaskRepeating
{
    public Repeating : number;
    public RepeatTill : Date;
    public ExecutionTimes : Date[];
    public ExceptionDates : ExceptionDate[];
}
