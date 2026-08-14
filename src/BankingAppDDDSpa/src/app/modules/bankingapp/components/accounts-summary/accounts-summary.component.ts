import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { MemberShipService } from '../../../../core/services/membership.service';

@Component({
  selector: 'app-accounts-summary',
  templateUrl: './accounts-summary.component.html',
  styleUrl: './accounts-summary.component.css',
  imports: [CommonModule, RouterModule]
})
export class AccountsSummaryComponent implements OnInit {
  keyCloakUserId: string = '';
  userProfile: any = null;
  accountNo: string = '1000004';
  userName: string = 'MD MAJID AKHTER';
  currentBalance: number = 264320.34;
  showBalance: boolean = true;
  isAccountNoMasked: boolean = true;

  // Branch Info from BranchController / BankController
  branchName: string = 'BASAVANAGUDI- GANDHI BAZAAR';
  branchCode: string = 'COOP0000446';
  branchAddress: string = '742 Gandhi Bazaar Main Road, Basavanagudi, Bengaluru, Karnataka 560004';
  branchPhone: string = '+91 80 2660 1234';

  showRecentTransactions: boolean = false;
  recentTransactions: any[] = [];

  isLoading: boolean = false;
  copySuccess: boolean = false;

  private membershipService = inject(MemberShipService);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  toggleRecentTransactions(): void {
    this.showRecentTransactions = !this.showRecentTransactions;
  }

  ngOnInit(): void {
    this.extractUserIdAndLoad();

    this.membershipService.currentUser$.subscribe((user: any) => {
      if (user) {
        this.extractUserIdAndLoad();
      }
    });
  }

  extractUserIdAndLoad(): void {
    const token = this.membershipService.getToken();
    const decodedToken: any = token ? this.membershipService.decodeToken(token) : null;
    const currentUser: any = this.membershipService.getUser();

    const extractedId = currentUser?.id || currentUser?.userId || currentUser?.keyCloakUserId || this.membershipService.getKeyCloakUserId() || decodedToken?.sub || '';
    if (extractedId) {
      this.keyCloakUserId = extractedId;
      this.loadData();
    } else {
      setTimeout(() => {
        const retryUser: any = this.membershipService.getUser();
        const retryId = retryUser?.id || retryUser?.userId || retryUser?.keyCloakUserId || this.membershipService.getKeyCloakUserId() || '';
        if (retryId) {
          this.keyCloakUserId = retryId;
          this.loadData();
        }
      }, 100);
    }
  }

  loadData(): void {
    if (!this.keyCloakUserId) return;

    this.isLoading = true;

    // 1. Fetch User Profile
    this.membershipService.getUserProfile(this.keyCloakUserId).subscribe({
      next: (res: any) => {
        const data = res?.result || res?.data || res;
        if (data) {
          this.userProfile = data;
          const fullName = data.fullName || data.name || `${data.firstName || ''} ${data.lastName || ''}`.trim();
          if (fullName) {
            this.userName = fullName.toUpperCase();
          }
        }
        this.cdr.detectChanges();
      },
      error: (err: any) => console.error('Error loading profile in accounts-summary:', err)
    });

    // 2. Fetch Account Details (UserAccountController)
    this.membershipService.getAccountDetails(this.keyCloakUserId).subscribe({
      next: (res: any) => {
        this.isLoading = false;
        const accountList = Array.isArray(res) ? res : (res?.result || res?.data || (res ? [res] : []));
        if (accountList && accountList.length > 0) {
          const primary = accountList[0];
          const rawNo = primary.accountNo !== undefined ? primary.accountNo : primary.AccountNo;
          this.accountNo = rawNo ? rawNo.toString() : this.accountNo;
          this.currentBalance = primary.currentBalance !== undefined ? primary.currentBalance : (primary.CurrentBalance !== undefined ? primary.CurrentBalance : this.currentBalance);

          // Populate recent transactions list
          const txList: any[] = [];
          const rawTxList = primary.transactionDetail || primary.TransactionDetail || primary.creditsCollection || primary.CreditsCollection || [];
          if (Array.isArray(rawTxList) && rawTxList.length > 0) {
            rawTxList.forEach((tx: any) => {
              txList.push({
                id: tx.transactionNumber || tx.TransactionNumber || tx.id || tx.Id || Math.floor(100000 + Math.random() * 900000),
                amount: tx.transactionAmount !== undefined ? tx.transactionAmount : (tx.TransactionAmount !== undefined ? tx.TransactionAmount : (tx.amount || 0)),
                date: tx.transactionDate ? new Date(tx.transactionDate).toLocaleString('en-GB') : (tx.date || new Date().toLocaleString('en-GB'))
              });
            });
          }

          if (txList.length === 0) {
            this.recentTransactions = [
              { id: 1100021, amount: 50000.00, date: '14/08/2026, 18:36:38' },
              { id: 1100019, amount: 999.00, date: '14/08/2026, 18:02:37' }
            ];
          } else {
            this.recentTransactions = txList;
          }
        }
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        this.isLoading = false;
        console.error('Error loading account details:', err);
        this.cdr.detectChanges();
      }
    });

    // 3. Fetch Branch Details from BranchController / BankController
    this.membershipService.getBranchDetails().subscribe({
      next: (res: any) => {
        console.log('BranchController / BankController API response:', res);
        const branches = Array.isArray(res) ? res : (res?.result || res?.data || (res ? [res] : []));
        if (branches && branches.length > 0) {
          const b = branches[0];
          this.branchName = b.name || b.Name || b.branchName || this.branchName;
          this.branchCode = b.branchCode || b.BranchCode || b.ifscCode || this.branchCode;
          const street = b.street || b.Street || '';
          const city = b.city || b.City || '';
          const zip = b.zipCode || b.ZipCode || '';
          const fullAddr = [street, city, zip].filter(Boolean).join(', ');
          if (fullAddr) {
            this.branchAddress = fullAddr;
          }
          if (b.phoneNumber || b.PhoneNumber) {
            this.branchPhone = b.phoneNumber || b.PhoneNumber;
          }
        }
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        console.error('Error fetching branch details:', err);
      }
    });
  }

  toggleMaskedAccountNo(): void {
    this.isAccountNoMasked = !this.isAccountNoMasked;
  }

  toggleBalance(): void {
    this.showBalance = !this.showBalance;
  }

  copyBranchIfsc(): void {
    const textToCopy = `Branch & IFSC: ${this.branchName}, ${this.branchCode}`;
    navigator.clipboard.writeText(textToCopy).then(() => {
      this.copySuccess = true;
      setTimeout(() => (this.copySuccess = false), 2000);
    });
  }

  get maskedAccountNumber(): string {
    if (!this.accountNo) return '**** **** **0612';
    if (!this.isAccountNoMasked) return this.accountNo;
    const lastDigits = this.accountNo.slice(-4);
    return `**** **** **${lastDigits}`;
  }

  navigateTo(path: string): void {
    this.router.navigate([path]);
  }
}
