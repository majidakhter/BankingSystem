import { Component, inject, OnInit } from '@angular/core';
import {CommonModule} from "@angular/common";
import { FormsModule } from '@angular/forms';
import { LoanService } from '@core/services/loan.service';
import { LoanModel } from '../../../../../core/domain/loan.model';

@Component({
  selector: 'app-loan-apply',
  templateUrl: './loan-apply.component.html',
  styleUrl: './loan-apply.component.css',
  imports: [CommonModule, FormsModule],
})
export class LoanApplyComponent implements OnInit{
   loan: LoanModel = new LoanModel();
   successMessage: string = '';
   errorMessage: string = '';
   private loanService= inject(LoanService);
  
  ngOnInit(): void {
    this.loan.startDate = new Date().toISOString().split('T')[0]; //Format as YYYY-MM-DD
  }
  applyLoan() {

    if (this.loan.durationInMonths) {
      this.loan.endDate = this.calculateEndDate();
    }

    this.loanService.saveLoan(this.loan).subscribe({
      next: response => {
        this.successMessage = 'Loan application submitted successfully!';
        this.loan = new LoanModel(); // Reset form
        this.loan.startDate = new Date().toISOString().split('T')[0]; // Reset start date to current date
      },
      error: error => {
        this.errorMessage = 'Failed to submit loan application. Please try again.';
      }
    });
  }


  calculateEndDate(): string {
    const startDate = new Date(this.loan.startDate);
    startDate.setMonth(startDate.getMonth() + this.loan.durationInMonths);
    return startDate.toISOString().split('T')[0]; // Return in YYYY-MM-DD format
  }
}
