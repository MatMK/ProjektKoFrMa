export class ChangeTable
{
    constructor(
        public TableName : string,
        public Id : number,
        public ColumnName : string,
        public Value
    ){}
}