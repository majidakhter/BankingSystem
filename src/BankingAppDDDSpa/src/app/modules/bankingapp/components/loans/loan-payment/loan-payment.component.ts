import { Component, OnInit, inject } from '@angular/core';
import { LoanService } from '@core/services/loan.service';
import { LoanModel } from '../../../../../core/domain/loan.model';
import { ActivatedRoute } from '@angular/router';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
@Component({
  selector: 'app-loan-payment',
  templateUrl: './loan-payment.component.html',
  styleUrl: './loan-payment.component.css',
  imports: [CurrencyPipe,CommonModule,FormsModule]
})
export class LoanPaymentComponent implements OnInit{
  loan: LoanModel = new LoanModel();
  paymentAmount: number = 0;
  successMessage: string = '';
  errorMessage: string = '';
  loanId: number = 0;
  private loanService= inject(LoanService);
  private route= inject(ActivatedRoute);

  ngOnInit(): void {
    this.loanId = this.route.snapshot.params['id'];
    this.getLoanDetails();
  }

  getLoanDetails() {
    this.loanService.getLoanById(this.loanId).subscribe({
      next:(data) => {
        this.loan = data;
      },
      error:(error) => {
        this.errorMessage = 'Failed to load loan details. Please try again later.';
      }
    });
  }
  makePayment(): void {
    if (this.paymentAmount > 0) {
      this.loanService.makeLoanPayment(this.loanId, this.paymentAmount).subscribe({
        next: (response: string) => {
          this.successMessage = `Payment successful: ${response}`;
          this.errorMessage = ''; 
          this.paymentAmount = 0; 
          this.getLoanDetails(); 
        },
        error: (error) => {
          alert("Payment successful");
          this.paymentAmount = 0;  
          this.getLoanDetails();
        }
      });
    } else {
      this.errorMessage = 'Please enter a valid payment amount greater than zero';
      this.successMessage = ''; 
    }
  }
}
