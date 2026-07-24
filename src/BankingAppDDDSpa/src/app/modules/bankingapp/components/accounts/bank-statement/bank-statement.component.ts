import { Component, OnInit, inject } from '@angular/core';
import { Transaction } from '../../../../../core/domain/transaction.model';
import { TransactionService } from '@core/services/transaction.service';
import { MemberShipService } from '@core/services/membership.service';
import { Router } from '@angular/router';
import { DatePipe, CurrencyPipe, CommonModule } from '@angular/common';

@Component({
  selector: 'app-bank-statement',
  templateUrl: './bank-statement.component.html',
  styleUrl: './bank-statement.component.css',
  imports: [DatePipe, CurrencyPipe, CommonModule]
})
export class BankStatementComponent implements OnInit{
  transactions: Transaction[] = [];
  userId: number = 0;
  isAuthorized: boolean = false;
  private transactionService = inject(TransactionService);
  private userService = inject(MemberShipService);
  private router = inject(Router);

  ngOnInit(): void {
    this.loadUserTransactions();
  }

  loadUserTransactions(): void {
    const user = this.userService.getUser(); 
    if (user) {
      this.userId = Number(user.id);
      this.transactionService.getTransactionsByUserId(this.userId).subscribe({
        next:(transactions) => {
          this.transactions = transactions;
        },
        error:(error) => {
          console.error('Failed to load transactions:', error);
        }
      });
    }
  }

  printStatement() {
    const printContents = document.getElementById('print-section')?.innerHTML;
    const originalContents = document.body.innerHTML;
  
    if (printContents) {
      document.body.innerHTML = printContents;
      window.print();
      document.body.innerHTML = originalContents;
    }
  }
}
