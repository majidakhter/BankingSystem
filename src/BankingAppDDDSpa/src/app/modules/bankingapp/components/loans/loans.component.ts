import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { LoanService } from '@core/services/loan.service';
import { MemberShipService } from '@core/services/membership.service';

@Component({
  selector: 'app-loans',
  templateUrl: './loans.component.html',
  styleUrl: './loans.component.css',
  imports: [CommonModule, FormsModule, RouterModule]
})
export class LoansComponent implements OnInit {
  activeTab: string = 'apply'; // 'apply' | 'applications' | 'operators' | 'evaluate'

  // Form State: Loan Application
  customerNid: string = '';
  customerBirthDate: string = '1995-05-15';
  monthlyIncome: number = 7500;
  propertyValue: number = 250000;
  propertyStreet: string = '742 Evergreen Terrace';
  propertyCity: string = 'Springfield';
  propertyZip: string = '97477';
  loanAmount: number = 50000;
  loanYears: number = 5;
  interestPercent: number = 7.5;
  loanTypeId: number = 1; // 1 = Personal, 2 = Home, 3 = Education, 4 = Business, 5 = Car

  // Form State: Operator
  operatorCompetence: number = 100000;
  operatorsList: any[] = [];

  // Form State: Underwriter Decision
  targetApplicationId: string = '';
  underwriterId: string = '';
  acceptedAmount: number = 50000;
  rejectionReason: string = 'Income to debt ratio insufficient';

  // Search State
  searchAppId: string = '';
  searchResult: any = null;

  isLoading: boolean = false;
  successMessage: string = '';
  errorMessage: string = '';
  keyCloakUserId: string = '';

  private loanService = inject(LoanService);
  private membershipService = inject(MemberShipService);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  ngOnInit(): void {
    this.extractUserId();
    this.membershipService.currentUser$.subscribe((user: any) => {
      if (user) {
        this.extractUserId();
      }
    });
    this.loadOperators();
  }

  extractUserId(): void {
    const token = this.membershipService.getToken();
    const decodedToken: any = token ? this.membershipService.decodeToken(token) : null;
    const currentUser: any = this.membershipService.getUser();
    this.keyCloakUserId = currentUser?.id || currentUser?.userId || currentUser?.keyCloakUserId || this.membershipService.getKeyCloakUserId() || decodedToken?.sub || '';
    if (this.keyCloakUserId) {
      this.underwriterId = this.keyCloakUserId;
    }
  }

  setTab(tab: string): void {
    this.activeTab = tab;
    this.errorMessage = '';
    this.successMessage = '';
    if (tab === 'operators') {
      this.loadOperators();
    }
  }

  // --- 1. Submit Loan Application (LoanApplicationController) ---
  onSubmitLoanApplication(): void {
    this.errorMessage = '';
    this.successMessage = '';

    if (!this.loanAmount || this.loanAmount <= 0) {
      this.errorMessage = 'Please enter a valid loan amount.';
      return;
    }

    if (!this.keyCloakUserId) {
      this.errorMessage = 'User session not found. Please log in again.';
      return;
    }

    this.isLoading = true;

    const command = {
      operatorId: this.keyCloakUserId,
      loanData: {
        customerData: {
          customerNationalIdentifier: this.customerNid || this.keyCloakUserId,
          customerBirthdate: this.customerBirthDate || '1995-05-15',
          customerMonthlyIncome: this.monthlyIncome || 5000
        },
        assetData: {
          propertyValue: this.propertyValue || 100000,
          propertyAddress: {
            street: this.propertyStreet,
            city: this.propertyCity,
            zipCode: this.propertyZip,
            country: 'USA'
          }
        },
        loanData: {
          loanAmount: this.loanAmount,
          loanNumberOfYears: this.loanYears,
          percent: this.interestPercent
        },
        loanTypeId: this.loanTypeId
      }
    };

    console.log('Submitting LoanApplicationSubmittedCommand:', command);

    this.loanService.createLoanApplication(command).subscribe({
      next: (res: any) => {
        this.isLoading = false;
        this.successMessage = 'Loan application submitted successfully! Reference ID: ' + (res?.result?.value || res?.id || 'Submitted');
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        this.isLoading = false;
        console.error('Error submitting loan application:', err);
        this.errorMessage = err?.error?.detail || err?.error?.title || 'Failed to submit loan application. Please check backend logs.';
        this.cdr.detectChanges();
      }
    });
  }

  // --- 2. Operator Management (OperatorController) ---
  onAddOperator(): void {
    this.errorMessage = '';
    this.successMessage = '';

    if (!this.operatorCompetence || this.operatorCompetence <= 0) {
      this.errorMessage = 'Please enter valid competence level amount.';
      return;
    }

    this.isLoading = true;
    const command = { competenceLevelAmount: this.operatorCompetence };

    this.loanService.addOperator(command).subscribe({
      next: (res: any) => {
        this.isLoading = false;
        this.successMessage = 'Operator registered successfully!';
        this.loadOperators();
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        this.isLoading = false;
        console.error('Error adding operator:', err);
        this.errorMessage = err?.error?.detail || 'Failed to add operator.';
        this.cdr.detectChanges();
      }
    });
  }

  loadOperators(): void {
    this.loanService.getOperators().subscribe({
      next: (res: any) => {
        const list = Array.isArray(res) ? res : (res?.result || res?.data || []);
        this.operatorsList = list;
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        console.error('Error loading operators:', err);
      }
    });
  }

  // --- 3. Evaluate / Accept / Reject Loan Application ---
  onEvaluateApplication(): void {
    if (!this.targetApplicationId) {
      this.errorMessage = 'Please enter a valid Loan Application ID.';
      return;
    }

    this.isLoading = true;
    const command = {
      loanApplicationId: this.targetApplicationId,
      underwriterId: this.underwriterId || this.keyCloakUserId
    };

    this.loanService.evaluateLoanApplication(command).subscribe({
      next: (res: any) => {
        this.isLoading = false;
        this.successMessage = 'Loan Application evaluated successfully!';
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        this.isLoading = false;
        this.errorMessage = err?.error?.detail || 'Evaluation request processed.';
        this.cdr.detectChanges();
      }
    });
  }

  onAcceptApplication(): void {
    if (!this.targetApplicationId) {
      this.errorMessage = 'Please enter a valid Loan Application ID.';
      return;
    }

    this.isLoading = true;
    const command = {
      loanApplicationId: this.targetApplicationId,
      underwriterId: this.underwriterId || this.keyCloakUserId,
      acceptedAmount: this.acceptedAmount || this.loanAmount
    };

    this.loanService.acceptLoanApplication(command).subscribe({
      next: (res: any) => {
        this.isLoading = false;
        this.successMessage = 'Loan Application Accepted!';
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        this.isLoading = false;
        this.errorMessage = err?.error?.detail || 'Loan application decision updated.';
        this.cdr.detectChanges();
      }
    });
  }

  onRejectApplication(): void {
    if (!this.targetApplicationId) {
      this.errorMessage = 'Please enter a valid Loan Application ID.';
      return;
    }

    this.isLoading = true;
    const command = {
      loanApplicationId: this.targetApplicationId,
      underwriterId: this.underwriterId || this.keyCloakUserId,
      reason: this.rejectionReason
    };

    this.loanService.rejectLoanApplication(command).subscribe({
      next: (res: any) => {
        this.isLoading = false;
        this.successMessage = 'Loan Application Rejected.';
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        this.isLoading = false;
        this.errorMessage = err?.error?.detail || 'Rejection updated.';
        this.cdr.detectChanges();
      }
    });
  }

  // --- 4. Search Loan Application ---
  onSearchLoan(): void {
    if (!this.searchAppId) {
      this.errorMessage = 'Please enter a valid Application ID to search.';
      return;
    }

    this.isLoading = true;
    this.loanService.getLoanApplicationById(this.searchAppId).subscribe({
      next: (res: any) => {
        this.isLoading = false;
        this.searchResult = res?.result || res?.data || res;
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        this.isLoading = false;
        this.errorMessage = 'Loan application not found for ID: ' + this.searchAppId;
        this.cdr.detectChanges();
      }
    });
  }
}
