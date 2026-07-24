import { TransactionType } from "./transactiontype.model";
import { UserModel } from "./user.model";

export interface Transaction{
    id : number;
    transactionDate : string;
    amount : number;
    transactionType : TransactionType;
    description : string;
    targetAccountNumber : string; 
    status : string;
}

export class TransactionModel implements Transaction{
    id !: number;
    transactionDate !: string;
    amount !: number;
    transactionType !: TransactionType; //deposit,withdraw,fund transfer
    description !: string;
    targetAccountNumber !: string; 
    status !: string;
    userid!: UserModel
}