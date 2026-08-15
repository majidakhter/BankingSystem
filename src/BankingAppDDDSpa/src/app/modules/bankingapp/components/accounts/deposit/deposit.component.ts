import { Component, inject } from '@angular/core';
import { CommonModule } from "@angular/common";
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { TransactionService } from '@core/services/transaction.service';

@Component({
  selector: 'app-deposit',
  templateUrl: './deposit.component.html',
  styleUrl: './deposit.component.css',
  imports: [CommonModule, FormsModule, RouterModule],
})
export class DepositComponent {
  accountNo: number = 0;
  amount: number = 0;
  description: string = '';
  message: string = '';
  errorMessage: string = '';
  private transactionService = inject(TransactionService);

  makeDeposit(): void {
    if (this.amount <= 0) {
      this.errorMessage = 'Deposit amount must be greater than zero.';
      return;
    }
    const desc = this.description && this.description.trim() ? this.description.trim() : 'Deposit funds';
    this.transactionService.depositMoney(this.accountNo, this.amount, desc).subscribe({
      next: (response) => {
        alert("Your deposit of " + `${this.amount}` + " is pending approval. Once approved by the admin, your balance will be updated.");
        this.errorMessage = '';
        this.clearForm();
      },
      error: (error) => {
        this.errorMessage = 'An error occurred during the deposit. Please try again.';
        this.message = '';
        console.error(error);
      }
    });
  }
  clearForm(): void {
    this.accountNo = 0;
    this.amount = 0;
    this.description = '';
  }
}
