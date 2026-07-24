import { Component, OnInit, inject } from '@angular/core';
import { LoanService } from '@core/services/loan.service';
import { LoanModel } from '../../../../../core/domain/loan.model';
import { Router } from '@angular/router';
import { CurrencyPipe } from '@angular/common';

@Component({
  selector: 'app-loan-list',
  templateUrl: './loan-list.component.html',
  styleUrl: './loan-list.component.css',
  imports:[CurrencyPipe]
})
export class LoanListComponent implements OnInit{
  loans: LoanModel[] = [];
  errorMessage: string = '';
  private loanService= inject(LoanService);
  private router= inject(Router);
  
  ngOnInit(): void {
    this.getLoans();
  }
  
  getLoans(): void {
    this.loanService.getLoans().subscribe({
      next:(data: LoanModel[]) => {
        this.loans = data;
      },
      error:(error) => {
        this.errorMessage = 'Error fetching loan data';
        console.error(error);
      }
    });
  }

  
  deleteLoan(id: number): void {
    this.loanService.deleteLoan(id).subscribe({
      next:() => {
        this.loans = this.loans.filter((loan) => loan.id !== id);
      },
      error:(error) => {
        this.errorMessage = 'Failed to delete loan. Please try again later.';
      }
    });
  }

  navigateToPayment(loanId: number) {
    this.router.navigate(['/loan-payment', loanId]);
  }
}
