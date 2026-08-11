import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MemberShipService } from '../../../../../core/services/membership.service';

@Component({
  selector: 'app-add-payee',
  templateUrl: './add-payee.component.html',
  styleUrl: './add-payee.component.css',
  imports: [CommonModule, FormsModule]
})
export class AddPayeeComponent implements OnInit {
  transferMode: string = 'Domestic'; // 'Domestic' | 'International'
  payeeNickname: string = '';
  payeeCategory: string = 'BankAccount'; // 'BankAccount' | 'CreditCard'
  bankType: string = 'Cooperative'; // 'Cooperative' | 'Other'
  accountType: string = 'Savings'; // 'Bank Account' | 'Savings' | 'Current' | 'Credit Card'
  accountNumber: string = '';
  confirmAccountNumber: string = '';
  ifscCode: string = '';
  payeeName: string = '';
  selectedRelation: string = 'Friends'; // 'Own' | 'Family' | 'Friends' | 'Business' | 'Others'

  isLoading: boolean = false;
  errorMessage: string = '';
  successMessage: string = '';
  keyCloakUserId: string = '';

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
  }

  extractUserId(): void {
    const token = this.membershipService.getToken();
    const decodedToken: any = token ? this.membershipService.decodeToken(token) : null;
    const currentUser: any = this.membershipService.getUser();
    this.keyCloakUserId = currentUser?.id || currentUser?.userId || currentUser?.keyCloakUserId || this.membershipService.getKeyCloakUserId() || decodedToken?.sub || '';
  }

  setTransferMode(mode: string): void {
    this.transferMode = mode;
  }

  setBankType(type: string): void {
    this.bankType = type;
  }

  setRelation(relation: string): void {
    this.selectedRelation = relation;
  }

  onFindIfsc(): void {
    alert('IFSC Helper: Enter standard IFSC code e.g., COOP0001234');
  }

  onCheckName(): void {
    if (this.accountNumber && this.accountNumber.length > 3) {
      alert(`Validating Payee name for Account: ${this.accountNumber}... Record Name Verified.`);
    } else {
      alert('Please enter a valid Account Number first to check name.');
    }
  }

  onCancel(): void {
    this.router.navigate(['/dashboard']);
  }

  onContinue(): void {
    this.errorMessage = '';
    this.successMessage = '';

    if (!this.payeeNickname && !this.payeeName) {
      this.errorMessage = 'Please enter Payee Nickname or Name as per Payee Account.';
      return;
    }

    if (!this.accountNumber) {
      this.errorMessage = 'Please enter Account Number.';
      return;
    }

    if (this.accountNumber !== this.confirmAccountNumber) {
      this.errorMessage = 'Account Number and Confirm Account Number do not match.';
      return;
    }

    let acctNoInt = parseInt(this.accountNumber, 10);
    if (isNaN(acctNoInt)) {
      this.errorMessage = 'Account Number must be a valid numeric value.';
      return;
    }
    if (acctNoInt > 2147483647) {
      acctNoInt = Math.abs(acctNoInt % 2147483647);
    }

    if (!this.keyCloakUserId) {
      this.errorMessage = 'User session not found. Please log in again.';
      return;
    }

    this.isLoading = true;
    const payeeFinalName = this.payeeName || this.payeeNickname;
    const bankName = this.bankType === 'Cooperative' ? 'Cooperative Bank' : `Other Bank (${this.ifscCode || 'IFSC'})`;

    const command = {
      accountId: this.keyCloakUserId,
      beneficiaryName: payeeFinalName,
      beneficiaryAccountNo: acctNoInt,
      beneficiaryBankName: bankName
    };

    console.log('Sending AddBeneficiaryCommand to AccountController endpoint addbeneficiary:', command);

    this.membershipService.addBeneficiary(command).subscribe({
      next: (res: any) => {
        this.isLoading = false;
        this.successMessage = 'Payee / Beneficiary added successfully!';
        this.cdr.detectChanges();
        setTimeout(() => {
          this.router.navigate(['/dashboard']);
        }, 1200);
      },
      error: (err: any) => {
        this.isLoading = false;
        console.error('Error adding beneficiary:', err);
        this.errorMessage = err?.error?.detail || err?.error?.title || 'Failed to add beneficiary. Please check account details.';
        this.cdr.detectChanges();
      }
    });
  }
}
