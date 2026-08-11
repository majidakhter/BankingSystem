import { Component, OnInit, HostListener, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { MemberShipService } from '../../../../core/services/membership.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css',
  imports: [CommonModule, RouterModule, FormsModule],
})
export class DashboardComponent implements OnInit {
  keyCloakUserId: string = '';
  userProfile: any = null;
  userAccounts: any[] = [];
  primaryAccount: any = null;

  userName: string = '';
  accountNo: string = '';
  accountBalance: number = 0;
  accountType: string = '';
  accountStatus: string = '';

  lastLoginTime: string = '';
  showBalance: boolean = true;
  isLoading: boolean = false;
  activeCategory: string = 'accounts';
  isUserMenuOpen: boolean = false;

  // Quick Send Money Form State
  transferType: string = 'Domestic';
  bankSelection: string = 'ownbank'; // 'ownbank' | 'otherbank'
  toAccount: string = '';
  fromAccount: string = '';
  sendAmount: number | null = null;

  allBankAccounts: any[] = [];
  beneficiariesList: any[] = [];

  // Recent transactions layout
  recentTransactions: any[] = [];

  private membershipService = inject(MemberShipService);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  get filteredAllBankAccounts(): any[] {
    const fromNo = (this.accountNo || '').toString().trim();
    if (!fromNo) {
      return this.allBankAccounts;
    }
    return this.allBankAccounts.filter((a: any) => a.accountNo && a.accountNo.toString().trim() !== fromNo);
  }

  get filteredBeneficiariesList(): any[] {
    const fromNo = (this.accountNo || '').toString().trim();
    if (!fromNo) {
      return this.beneficiariesList;
    }
    return this.beneficiariesList.filter((b: any) => b.accountNo && b.accountNo.toString().trim() !== fromNo);
  }

  ngOnInit(): void {
    this.extractUserIdAndLoadData();

    // Subscribe to currentUser$ so when login completes or session hydrates, data loads automatically!
    this.membershipService.currentUser$.subscribe((user: any) => {
      if (user && (!this.keyCloakUserId || this.keyCloakUserId.length < 10)) {
        this.extractUserIdAndLoadData();
      }
    });

    this.setLastLoginTime();
  }

  extractUserIdAndLoadData(): void {
    const token = this.membershipService.getToken();
    const decodedToken = token ? this.membershipService.decodeToken(token) : null;
    const currentUser: any = this.membershipService.getUser();

    const extractedId = currentUser?.id || currentUser?.userId || currentUser?.keyCloakUserId || this.membershipService.getKeyCloakUserId() || decodedToken?.sub || '';
    if (extractedId) {
      this.keyCloakUserId = extractedId;
      this.loadUserData();
    } else {
      setTimeout(() => {
        const retryToken = this.membershipService.getToken();
        const retryDecoded = retryToken ? this.membershipService.decodeToken(retryToken) : null;
        const retryUser: any = this.membershipService.getUser();
        const retryId = retryUser?.id || retryUser?.userId || retryUser?.keyCloakUserId || this.membershipService.getKeyCloakUserId() || retryDecoded?.sub || '';
        if (retryId) {
          this.keyCloakUserId = retryId;
          this.loadUserData();
        }
      }, 100);
    }

    console.log('Dashboard extractUserIdAndLoadData - Keycloak User ID:', this.keyCloakUserId);
  }

  toggleUserMenu(event?: Event): void {
    if (event) {
      event.stopPropagation();
    }
    this.isUserMenuOpen = !this.isUserMenuOpen;
  }

  closeUserMenu(): void {
    this.isUserMenuOpen = false;
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    const target = event.target as HTMLElement;
    if (!target.closest('.user-menu-dropdown')) {
      this.isUserMenuOpen = false;
    }
  }

  setLastLoginTime(): void {
    const now = new Date();
    this.lastLoginTime = now.toLocaleString('en-GB', {
      day: '2-digit',
      month: '2-digit',
      year: '2-digit',
      hour: '2-digit',
      minute: '2-digit',
      hour12: true
    });
  }

  loadUserData(): void {
    const currentUser: any = this.membershipService.getUser();

    if (currentUser) {
      this.userName = currentUser.name || currentUser.fullName || `${currentUser.firstName || ''} ${currentUser.lastName || ''}`.trim() || currentUser.email || 'User';
    }

    if (this.keyCloakUserId) {
      this.isLoading = true;

      // 1. Call UserAccountController endpoint getaccountdetails/{userId} (returns UserAccountDTO with BeneficiariesDetail)
      this.membershipService.getAccountDetails(this.keyCloakUserId).subscribe({
        next: (res: any) => {
          this.isLoading = false;
          console.log('UserAccountController getaccountdetails raw API response:', res);
          const accountList = Array.isArray(res) ? res : (res?.result || res?.data || res?.value || (res ? [res] : []));
          if (accountList && accountList.length > 0) {
            this.userAccounts = accountList.map((acct: any) => {
              const acctNoRaw = acct.accountNo !== undefined ? acct.accountNo : acct.AccountNo;
              const acctNoStr = acctNoRaw !== undefined && acctNoRaw !== null ? acctNoRaw.toString() : '';
              const balanceVal = acct.currentBalance !== undefined ? acct.currentBalance : (acct.CurrentBalance !== undefined ? acct.CurrentBalance : 0);
              const typeId = acct.accountTypeId !== undefined ? acct.accountTypeId : acct.AccountTypeId;
              const typeStr = typeId === 1 ? 'Savings Account' : (typeId === 2 ? 'Current Account' : (typeId === 3 ? 'Loan Account' : 'Savings Account'));
              const statusId = acct.accountStatusId !== undefined ? acct.accountStatusId : acct.AccountStatusId;
              const statusStr = (statusId === 1 || statusId === true) ? 'Active' : 'Inactive';

              return {
                accountNo: acctNoStr,
                balance: balanceVal,
                accountType: typeStr,
                status: statusStr,
                userFullName: acct.userFullName || acct.UserFullName || '',
                transactionDetail: acct.transactionDetail || acct.TransactionDetail || [],
                beneficiariesDetail: acct.beneficiariesDetail || acct.BeneficiariesDetail || []
              };
            });

            this.primaryAccount = this.userAccounts[0];
            if (this.primaryAccount) {
              this.accountNo = this.primaryAccount.accountNo;
              this.accountBalance = this.primaryAccount.balance;
              this.accountType = this.primaryAccount.accountType;
              this.accountStatus = this.primaryAccount.status;
              if (this.primaryAccount.userFullName) {
                this.userName = this.primaryAccount.userFullName;
              }
              this.fromAccount = `${this.accountNo} - ${this.accountType}`;

              const txns = this.primaryAccount.transactionDetail;
              if (txns && Array.isArray(txns) && txns.length > 0) {
                this.recentTransactions = txns.map((tx: any) => ({
                  id: tx.transactionNumber !== undefined ? tx.transactionNumber : (tx.TransactionNumber !== undefined ? tx.TransactionNumber : (tx.id || 'N/A')),
                  amount: tx.transactionAmount !== undefined ? tx.transactionAmount : (tx.TransactionAmount !== undefined ? tx.TransactionAmount : 0),
                  date: tx.transactionDate || tx.TransactionDate ? new Date(tx.transactionDate || tx.TransactionDate).toLocaleString('en-GB') : 'N/A'
                }));
              } else {
                this.recentTransactions = [];
              }
            }

            // Extract Beneficiaries directly from UserAccountDTO combined payload
            const beneficiariesExtracted: any[] = [];
            accountList.forEach((acct: any) => {
              const bList = acct.beneficiariesDetail || acct.BeneficiariesDetail || [];
              if (Array.isArray(bList)) {
                bList.forEach((b: any) => {
                  const acctNo = b.beneficaryAccountNo !== undefined ? b.beneficaryAccountNo : b.BeneficaryAccountNo;
                  const acctNoStr = acctNo ? acctNo.toString() : '';
                  const name = b.beneficaryName || b.BeneficaryName || 'Beneficiary';
                  const bank = b.beneficaryBankName || b.BeneficaryBankName || 'External Bank';
                  beneficiariesExtracted.push({
                    id: b.id || b.Id,
                    accountNo: acctNoStr,
                    name: name,
                    bankName: bank,
                    label: `${acctNoStr} - ${name} (${bank})`
                  });
                });
              }
            });
            this.beneficiariesList = beneficiariesExtracted;

            // Ensure toAccount is set to valid filtered option
            this.updateDefaultToAccount();
          } else {
            this.userAccounts = [];
            this.recentTransactions = [];
            this.beneficiariesList = [];
          }
          this.cdr.detectChanges();
        },
        error: (err: any) => {
          this.isLoading = false;
          console.error('Error fetching account details from UserAccountController:', err);
          this.cdr.detectChanges();
        }
      });

      // 2. Call UserController endpoint getuserprofile/{userId}
      this.membershipService.getUserProfile(this.keyCloakUserId).subscribe({
        next: (res: any) => {
          const data = res?.result || res?.data || res?.value || res;
          if (data) {
            this.userProfile = data;
            if (data.fullName || data.name) {
              this.userName = data.fullName || data.name;
            } else if (data.firstName || data.lastName) {
              this.userName = `${data.firstName || ''} ${data.lastName || ''}`.trim();
            }
          }
          this.cdr.detectChanges();
        },
        error: (err: any) => {
          console.error('Error fetching user profile from UserController:', err);
          this.cdr.detectChanges();
        }
      });

      // 3. Load all bank accounts from Accounts table for Own Bank selection
      this.loadAllBankAccounts();
    }
  }

  loadAllBankAccounts(): void {
    this.membershipService.getAllAccounts().subscribe({
      next: (res: any) => {
        const rawList = Array.isArray(res) ? res : (res?.result || res?.data || res?.value || []);
        this.allBankAccounts = rawList.map((a: any) => {
          const acctNo = a.accountNo !== undefined ? a.accountNo : a.AccountNo;
          const acctNoStr = acctNo ? acctNo.toString() : '';
          const typeId = a.accountTypeId !== undefined ? a.accountTypeId : a.AccountTypeId;
          return {
            accountNo: acctNoStr,
            label: `${acctNoStr} (${typeId === 2 ? 'Current Account' : 'Savings Account'})`
          };
        });

        console.log('Loaded all bank accounts from Accounts table:', this.allBankAccounts);
        this.updateDefaultToAccount();
        this.cdr.detectChanges();
      },
      error: (err: any) => console.error('Error fetching all bank accounts:', err)
    });
  }

  updateDefaultToAccount(): void {
    if (this.bankSelection === 'ownbank') {
      const available = this.filteredAllBankAccounts;
      if (available.length > 0) {
        this.toAccount = available[0].accountNo;
      }
    } else if (this.bankSelection === 'otherbank') {
      const available = this.filteredBeneficiariesList;
      if (available.length > 0) {
        this.toAccount = available[0].accountNo;
      }
    }
  }

  onBankTypeChange(): void {
    this.updateDefaultToAccount();
  }

  toggleBalance(): void {
    this.showBalance = !this.showBalance;
  }

  selectCategory(category: string): void {
    this.activeCategory = category;
  }

  onQuickSendMoney(): void {
    if (!this.sendAmount || this.sendAmount <= 0) {
      alert('Please enter a valid transfer amount.');
      return;
    }
    this.router.navigate(['/transfer'], {
      queryParams: {
        amount: this.sendAmount,
        toAccount: this.toAccount,
        bankType: this.bankSelection
      }
    });
  }

  navigateTo(path: string): void {
    this.router.navigate([path]);
  }

  logout(): void {
    this.membershipService.logout();
  }
}
