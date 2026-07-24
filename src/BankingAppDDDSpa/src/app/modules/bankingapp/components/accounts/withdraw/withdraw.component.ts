import { Component, inject } from '@angular/core';
import {CommonModule} from "@angular/common";
import { FormsModule } from '@angular/forms';
import { TransactionService } from '@core/services/transaction.service'; 
@Component({
  selector: 'app-withdraw',
  templateUrl: './withdraw.component.html',
  styleUrl: './withdraw.component.css',
  imports: [CommonModule, FormsModule]
})
export class WithdrawComponent {
  userId: number = 0;
  amount: number = 0;
  description: string = '';
  message: string = '';
  errorMessage: string = '';
  private transactionService = inject(TransactionService);

  makeWithdrawal(): void {
    if (this.amount <= 0) {
      this.errorMessage = 'Withdrawal amount must be greater than zero.';
      return;
    }

    this.transactionService.withdrawMoney(this.userId, this.amount, this.description).subscribe({
      next: (response) => {
        alert("Your withdraw of "+`${this.amount}`+" is pending approval. Once approved by the admin, your balance will be updated.");
        // this.message = `Successfully withdrew ${this.amount} for user ${this.userId}.`;
        this.errorMessage = '';
        this.clearForm();
      },
      error: (error) => {
        this.errorMessage = 'An error occurred during the withdrawal. Please try again.';
        this.message = '';
        console.error(error);
      }
    });
  }

  clearForm(): void {
    this.userId = 0;
    this.amount = 0;
    this.description = '';
  }
}
