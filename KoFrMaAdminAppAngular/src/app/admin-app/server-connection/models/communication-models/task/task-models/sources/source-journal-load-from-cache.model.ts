import { ISource } from "../isource.interface";

export class SourceJournalLoadFromCache implements ISource
{
    /** ID Tasku na který se bude navazovat, pokud je soubor na 100% v cachi daemona */
    public journalID: number;
}