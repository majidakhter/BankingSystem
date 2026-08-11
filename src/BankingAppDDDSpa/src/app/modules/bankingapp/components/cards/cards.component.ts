import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule, ActivatedRoute } from '@angular/router';
import { MemberShipService } from '../../../../core/services/membership.service';

@Component({
  selector: 'app-cards',
  templateUrl: './cards.component.html',
  styleUrl: './cards.component.css',
  imports: [CommonModule, FormsModule, RouterModule]
})
export class CardsComponent implements OnInit {
  activeTab: string = 'credit'; // 'credit' | 'debit' | 'forex' | 'prepaid'
  
  keyCloakUserId: string = '';
  userName: string = 'MD MAJID AKHTER';
  accountNo: string = '1000004';
  
  // Credit Card State
  creditCardNumber: string = '5372 06** **** 9855';
  creditLimit: number = 100000;
  upcomingPayment: number = 0.00;
  totalOutstanding: number = -0.44;
  
  // Debit Card State
  debitCardNumber: string = '5129 **** **** 5748';
  rewardPoints: number = 0;
  
  isLoading: boolean = false;
  actionMessage: string = '';

  private membershipService = inject(MemberShipService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private cdr = inject(ChangeDetectorRef);

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      if (params['type']) {
        this.activeTab = params['type'];
      }
    });

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
    }
  }

  loadData(): void {
    if (!this.keyCloakUserId) return;

    this.membershipService.getUserProfile(this.keyCloakUserId).subscribe({
      next: (res: any) => {
        const data = res?.result || res?.data || res;
        if (data) {
          const fullName = data.fullName || data.name || `${data.firstName || ''} ${data.lastName || ''}`.trim();
          if (fullName) {
            this.userName = fullName.toUpperCase();
          }
        }
        this.cdr.detectChanges();
      },
      error: (err: any) => console.error('Error loading profile in cards component:', err)
    });

    this.membershipService.getAccountDetails(this.keyCloakUserId).subscribe({
      next: (res: any) => {
        const accountList = Array.isArray(res) ? res : (res?.result || res?.data || (res ? [res] : []));
        if (accountList && accountList.length > 0) {
          const primary = accountList[0];
          const rawNo = primary.accountNo !== undefined ? primary.accountNo : primary.AccountNo;
          if (rawNo) {
            this.accountNo = rawNo.toString();
          }
        }
        this.cdr.detectChanges();
      },
      error: (err: any) => console.error('Error loading account details in cards component:', err)
    });
  }

  setTab(tab: string): void {
    this.activeTab = tab;
    this.actionMessage = '';
  }

  onAction(actionName: string): void {
    this.actionMessage = `${actionName} request processed successfully!`;
    setTimeout(() => {
      this.actionMessage = '';
      this.cdr.detectChanges();
    }, 2500);
  }

  navigateTo(path: string): void {
    this.router.navigate([path]);
  }
}
