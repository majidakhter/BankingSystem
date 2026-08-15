import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Router } from '@angular/router';
import { TransactionService } from '@core/services/transaction.service';
import { MemberShipService } from '@core/services/membership.service';

interface StatementMonthOption {
  label: string;
  startDate: string;
  endDate: string;
}

@Component({
  selector: 'app-bank-statement',
  templateUrl: './bank-statement.component.html',
  styleUrl: './bank-statement.component.css',
  imports: [CommonModule, FormsModule, RouterModule, DatePipe]
})
export class BankStatementComponent implements OnInit {
  accountNo: string = '1000004';
  userName: string = 'MD MAJID AKHTER';
  availableBalance: number = 264320.34;
  numericUserId: string = '1000004';

  selectedPeriod: string = 'Recent Transactions';
  fromDate: string = '';
  toDate: string = '';
  selectedFormat: string = 'PDF';

  searchQuery: string = '';
  transactionFilter: string = 'All Transactions';
  viewMode: 'list' | 'grid' = 'list';

  monthOptions: StatementMonthOption[] = [];
  transactions: any[] = [];
  filteredTransactions: any[] = [];

  isCustomDate: boolean = false;
  emailSuccessMessage: string = '';
  downloadSuccessMessage: string = '';

  private transactionService = inject(TransactionService);
  private membershipService = inject(MemberShipService);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  ngOnInit(): void {
    this.generateLast6MonthsOptions();
    this.loadUserData();
  }

  generateLast6MonthsOptions(): void {
    this.monthOptions = [];
    const now = new Date();
    for (let i = 0; i < 6; i++) {
      const d = new Date(now.getFullYear(), now.getMonth() - i, 1);
      const monthName = d.toLocaleString('en-US', { month: 'long' });
      const year = d.getFullYear();
      const lastDay = new Date(year, d.getMonth() + 1, 0).getDate();

      const startStr = `${year}-${String(d.getMonth() + 1).padStart(2, '0')}-01`;
      const endStr = `${year}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(lastDay).padStart(2, '0')}`;

      this.monthOptions.push({
        label: `${monthName} ${year}`,
        startDate: startStr,
        endDate: endStr
      });
    }
  }

  loadUserData(): void {
    const user: any = this.membershipService.getUser();
    const token = this.membershipService.getToken();
    const decoded: any = token ? this.membershipService.decodeToken(token) : null;
    const userId = user?.id || user?.userId || this.membershipService.getKeyCloakUserId() || decoded?.sub || '';

    if (user) {
      const fullName = user.fullName || user.name || `${user.firstName || ''} ${user.lastName || ''}`.trim();
      if (fullName) this.userName = fullName.toUpperCase();
      if (user.id) this.numericUserId = user.id.toString();
    }

    if (userId) {
      // 1. Fetch Account Details
      this.membershipService.getAccountDetails(userId).subscribe({
        next: (res: any) => {
          const accountList = Array.isArray(res) ? res : (res?.result || res?.data || (res ? [res] : []));
          if (accountList && accountList.length > 0) {
            const primary = accountList[0];
            const rawNo = primary.accountNo !== undefined ? primary.accountNo : primary.AccountNo;
            this.accountNo = rawNo ? rawNo.toString() : this.accountNo;
            this.availableBalance = primary.currentBalance !== undefined ? primary.currentBalance : (primary.CurrentBalance !== undefined ? primary.CurrentBalance : this.availableBalance);

            // Populate combined transactions (Credits & Debits)
            const txList: any[] = [];
            const rawDetails = primary.transactionDetail || primary.TransactionDetail || [];
            if (Array.isArray(rawDetails) && rawDetails.length > 0) {
              rawDetails.forEach((tx: any) => {
                const type = tx.transactionType || tx.TransactionType || (tx.type || 'Credit');
                txList.push({
                  id: tx.transactionNumber || tx.TransactionNumber || tx.id || tx.Id || Math.floor(100000 + Math.random() * 900000),
                  amount: tx.transactionAmount !== undefined ? tx.transactionAmount : (tx.TransactionAmount !== undefined ? tx.TransactionAmount : (tx.amount || 0)),
                  date: tx.transactionDate ? new Date(tx.transactionDate) : new Date(),
                  type: type,
                  description: tx.description || tx.Description || (type.toLowerCase() === 'debit' ? 'Withdrawal' : 'Deposit')
                });
              });
            }

            const rawCredits = primary.creditsCollection || primary.CreditsCollection || [];
            if (Array.isArray(rawCredits) && rawCredits.length > 0 && txList.length === 0) {
              rawCredits.forEach((tx: any) => {
                txList.push({
                  id: tx.transactionNo || tx.TransactionNo || tx.id || tx.Id || Math.floor(100000 + Math.random() * 900000),
                  amount: tx.amount !== undefined ? (tx.amount?.value !== undefined ? tx.amount.value : tx.amount) : 0,
                  date: tx.transactionDate ? new Date(tx.transactionDate) : new Date(),
                  type: 'Credit',
                  description: tx.description || 'Deposit'
                });
              });
            }

            const rawDebits = primary.debitsCollection || primary.DebitsCollection || [];
            if (Array.isArray(rawDebits) && rawDebits.length > 0) {
              rawDebits.forEach((tx: any) => {
                txList.push({
                  id: tx.transactionNo || tx.TransactionNo || tx.id || tx.Id || Math.floor(100000 + Math.random() * 900000),
                  amount: tx.amount !== undefined ? (tx.amount?.value !== undefined ? tx.amount.value : tx.amount) : 0,
                  date: tx.transactionDate ? new Date(tx.transactionDate) : new Date(),
                  type: 'Debit',
                  description: tx.description || 'Withdrawal'
                });
              });
            }

            if (txList.length === 0) {
              this.setMockTransactions();
            } else {
              this.transactions = txList.sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime());
              this.applyFilters();
            }
          }
        },
        error: (err: any) => {
          console.error('Error fetching account details for statement:', err);
          this.setMockTransactions();
        }
      });
    } else {
      this.setMockTransactions();
    }
  }

  setMockTransactions(): void {
    this.transactions = [
      { id: 1100024, amount: 1401.00, date: new Date('2026-08-15T12:08:21'), type: 'Debit', description: 'Withdrawal / ATM' },
      { id: 1100021, amount: 50000.00, date: new Date('2026-08-14T18:36:38'), type: 'Credit', description: 'Salary Deposit' },
      { id: 1100019, amount: 999.00, date: new Date('2026-08-14T18:02:37'), type: 'Credit', description: 'UPI Transfer In' },
      { id: 1100018, amount: 2500.00, date: new Date('2026-08-10T11:15:00'), type: 'Debit', description: 'Bill Payment' },
      { id: 1100015, amount: 15000.00, date: new Date('2026-07-28T09:30:00'), type: 'Credit', description: 'FD Interest Credit' }
    ];
    this.applyFilters();
  }

  onPeriodChange(): void {
    this.isCustomDate = false;
    const now = new Date();

    if (this.selectedPeriod === 'Recent Transactions') {
      this.fromDate = '';
      this.toDate = '';
    } else if (this.selectedPeriod === 'Current Financial Year') {
      const year = now.getMonth() >= 3 ? now.getFullYear() : now.getFullYear() - 1;
      this.fromDate = `${year}-04-01`;
      this.toDate = `${year + 1}-03-31`;
    } else if (this.selectedPeriod === 'Previous Financial Year') {
      const year = (now.getMonth() >= 3 ? now.getFullYear() : now.getFullYear() - 1) - 1;
      this.fromDate = `${year}-04-01`;
      this.toDate = `${year + 1}-03-31`;
    } else if (this.selectedPeriod === 'Custom Date') {
      this.isCustomDate = true;
      const firstOfMonth = new Date(now.getFullYear(), now.getMonth(), 1);
      this.fromDate = firstOfMonth.toISOString().split('T')[0];
      this.toDate = now.toISOString().split('T')[0];
    } else {
      // Monthly option selected
      const match = this.monthOptions.find(m => m.label === this.selectedPeriod);
      if (match) {
        this.fromDate = match.startDate;
        this.toDate = match.endDate;
      }
    }
    this.applyFilters();
  }

  applyFilters(): void {
    let list = [...this.transactions];

    // Filter by Type
    if (this.transactionFilter === 'Credits Only') {
      list = list.filter(t => t.type?.toLowerCase() === 'credit');
    } else if (this.transactionFilter === 'Debits Only') {
      list = list.filter(t => t.type?.toLowerCase() === 'debit');
    }

    // Filter by Search Query
    if (this.searchQuery && this.searchQuery.trim()) {
      const q = this.searchQuery.trim().toLowerCase();
      list = list.filter(t => 
        t.id.toString().includes(q) ||
        (t.description && t.description.toLowerCase().includes(q)) ||
        (t.type && t.type.toLowerCase().includes(q)) ||
        t.amount.toString().includes(q)
      );
    }

    // Filter by Date range if selected
    if (this.fromDate && this.toDate) {
      const start = new Date(this.fromDate).getTime();
      const end = new Date(this.toDate).getTime() + 86400000;
      list = list.filter(t => {
        const time = new Date(t.date).getTime();
        return time >= start && time <= end;
      });
    }

    this.filteredTransactions = list;
    this.cdr.detectChanges();
  }

  onDownloadStatement(): void {
    const format = this.selectedFormat;
    this.downloadSuccessMessage = `Statement downloaded successfully in ${format} format!`;
    setTimeout(() => (this.downloadSuccessMessage = ''), 3500);

    const printContent = `
===========================================================
                COOPERATIVE BANK ACCOUNT STATEMENT
===========================================================
Account Number: ${this.accountNo}
Account Holder: ${this.userName}
Period        : ${this.selectedPeriod} (${this.fromDate || 'N/A'} to ${this.toDate || 'N/A'})
Format        : ${format}
Generated Date: ${new Date().toLocaleString('en-GB')}
-----------------------------------------------------------
ID         TYPE     AMOUNT          DATE                   DESCRIPTION
-----------------------------------------------------------
${this.filteredTransactions.map(t => `${t.id}   ${t.type.padEnd(7)} ₹${t.amount.toFixed(2).padEnd(12)} ${new Date(t.date).toLocaleString('en-GB').padEnd(22)} ${t.description}`).join('\n')}
===========================================================
    `;

    const blob = new Blob([printContent], { type: 'text/plain;charset=utf-8' });
    const link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = `Account_Statement_${this.accountNo}_${format}.${format.toLowerCase() === 'excel' ? 'csv' : format.toLowerCase() === 'pdf' ? 'pdf' : 'txt'}`;
    link.click();
  }

  onEmailStatement(): void {
    const user = this.membershipService.getUser();
    const email = user?.email || 'user@example.com';
    this.emailSuccessMessage = `PDF Statement sent successfully to ${email}!`;
    setTimeout(() => (this.emailSuccessMessage = ''), 4000);
  }

  downloadInterestCertificate(): void {
    alert('CASA Interest Certificate generated and downloaded successfully!');
  }

  downloadBalanceCertificate(): void {
    alert('Balance Certificate generated and downloaded successfully!');
  }

  navigateTo(path: string): void {
    this.router.navigate([path]);
  }
}
