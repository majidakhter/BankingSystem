import { Component, OnInit, inject } from '@angular/core';
import {CommonModule, DatePipe, CurrencyPipe} from "@angular/common";
import { FormsModule } from '@angular/forms';
import { Transaction } from '../../../../../core/domain/transaction.model';
import { TransactionService } from '@core/services/transaction.service';
import { MemberShipService } from '@core/services/membership.service';
import { TransactionType } from '@core/domain/transactiontype.model';

@Component({
  selector: 'app-transaction-list',
  templateUrl: './transaction-list.component.html',
  styleUrl: './transaction-list.component.css',
  imports: [CommonModule, FormsModule, DatePipe,CurrencyPipe ]
})
export class TransactionListComponent  implements OnInit {
  transactions: Transaction[] = [];
  errorMessage: string = '';
  isAuthorized: boolean = false;
  private transactionService = inject(TransactionService);
  private userService = inject(MemberShipService);
  
  ngOnInit(): void {
    this.fetchTransactions1();
  }
  
  fetchTransactions(): void {
      this.transactionService.getTransactions().subscribe({
       next: (data: Transaction[]) => {
        this.transactions = data;
      },
      error: (error) => {
        this.errorMessage = 'Error fetching transaction data';
        console.error(error);
      }
    });
  }

  fetchTransactions1(): void {
    this.transactions = [
      {
        id : 1078618765,
        transactionDate : '2026-07-23',
        amount : 5750,
        transactionType : TransactionType.DEPOSIT, //deposit,withdraw,fund transfer
        description : 'deposited by friend',
        targetAccountNumber : '0001278654',
        status : 'APPROVED',
      }
    ];
  }
  
  changeTransactionStatus(transactionId: number, status: string): void {
    this.transactionService.updateTransactionStatus(transactionId, status).subscribe({
      next: () => {
        this.transactions = this.transactions.map(transaction =>
          transaction.id === transactionId ? { ...transaction, status: status } : transaction
        );
      },
      error: (error) => {
        this.errorMessage = `Failed to update transaction status. Please try again.`;
      }
    });
  }

  deleteTransaction(id: number): void {
    if (confirm('Are you sure you want to delete this transaction?')) {
      this.transactionService.deleteTransaction(id).subscribe(() => {
        this.transactions = this.transactions.filter(transaction => transaction.id !== id);
      });
    }
  }
}
