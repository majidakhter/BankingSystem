import { User } from "./user.model";

export interface Loan{
    id : number;
    loanType : string;
    loanAmount : number;
    interestRate : number;
    monthlyPayment : number;
    durationInMonths : number;
    startDate : string;
    endDate : string;
    balanceRemaining : number;
    paymentsMade : number
}

export class LoanModel implements Loan{
    id !: number;
    loanType !: string;
    loanAmount !: number;
    interestRate !: number;
    monthlyPayment !: number;
    durationInMonths !: number;
    startDate !: string;
    endDate !: string;
    balanceRemaining !: number;
    status !: string;
    paymentsMade !: number
    user !: User;
}