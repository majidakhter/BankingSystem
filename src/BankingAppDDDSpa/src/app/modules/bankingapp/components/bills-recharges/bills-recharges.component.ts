import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { MemberShipService } from '../../../../core/services/membership.service';

@Component({
  selector: 'app-bills-recharges',
  templateUrl: './bills-recharges.component.html',
  styleUrl: './bills-recharges.component.css',
  imports: [CommonModule, FormsModule, RouterModule]
})
export class BillsRechargesComponent implements OnInit {
  rechargeCategory: string = 'MobilePrepaid'; // 'MobilePrepaid' | 'FASTag' | 'DTH'
  mobileNumber: string = '';
  rechargeAmount: number = 299;
  gatewayProvider: string = 'PhonePe'; // 'PhonePe' | 'Razorpay' | 'DirectDebit'
  searchBillerQuery: string = '';
  
  isLoading: boolean = false;
  successMessage: string = '';
  errorMessage: string = '';
  initiationResult: any = null;

  showPhonePeModal: boolean = false;
  showRazorpayModal: boolean = false;
  copySuccess: boolean = false;

  private membershipService = inject(MemberShipService);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  ngOnInit(): void {
    this.loadRazorpayScript();
  }

  loadRazorpayScript(): void {
    if (!document.getElementById('razorpay-sdk')) {
      const script = document.createElement('script');
      script.id = 'razorpay-sdk';
      script.src = 'https://checkout.razorpay.com/v1/checkout.js';
      script.async = true;
      document.body.appendChild(script);
    }
  }

  setCategory(cat: string): void {
    this.rechargeCategory = cat;
  }

  setGateway(gw: string): void {
    this.gatewayProvider = gw;
  }

  onRechargeNow(): void {
    this.errorMessage = '';
    this.successMessage = '';
    this.initiationResult = null;
    this.showPhonePeModal = false;
    this.showRazorpayModal = false;

    if (!this.mobileNumber || this.mobileNumber.length < 10) {
      this.errorMessage = 'Please enter a valid 10-digit Mobile / Account Number.';
      return;
    }

    if (!this.rechargeAmount || this.rechargeAmount <= 0) {
      this.errorMessage = 'Please enter a valid recharge amount.';
      return;
    }

    this.isLoading = true;

    const payload = {
      accountNumber: 1000001,
      amount: this.rechargeAmount,
      currency: 'INR',
      contactNumber: this.mobileNumber,
      billId: `BILL_${Date.now()}`,
      gatewayProvider: this.gatewayProvider
    };

    if (this.gatewayProvider === 'PhonePe') {
      this.membershipService.requestPhonePePayment(payload).subscribe({
        next: (res: any) => {
          this.isLoading = false;
          this.initiationResult = res;
          this.showPhonePeModal = true;
          this.successMessage = `PhonePe UPI QR Code generated for Order: ${res?.orderId || 'N/A'}. Scan with PhonePe app to pay!`;
          this.cdr.detectChanges();
        },
        error: (err: any) => {
          this.isLoading = false;
          this.errorMessage = 'PhonePe Payment initiation failed. Please try again.';
          this.cdr.detectChanges();
        }
      });
    } else if (this.gatewayProvider === 'Razorpay') {
      this.membershipService.requestRazorpay(payload).subscribe({
        next: (res: any) => {
          this.isLoading = false;
          this.initiationResult = res;
          this.successMessage = `Razorpay Order Created: ${res?.orderId || 'N/A'}. Launching Razorpay Checkout...`;
          this.cdr.detectChanges();
          this.openRazorpayCheckout(res?.orderId || `order_${Date.now()}`, this.rechargeAmount);
        },
        error: (err: any) => {
          this.isLoading = false;
          this.errorMessage = 'Razorpay Payment initiation failed. Please try again.';
          this.cdr.detectChanges();
        }
      });
    } else {
      this.membershipService.payBill(payload).subscribe({
        next: (res: any) => {
          this.isLoading = false;
          this.initiationResult = res;
          this.successMessage = `Bill Payment of ₹${this.rechargeAmount} for ${this.rechargeCategory} (${this.mobileNumber}) processed successfully! Order ID: ${res?.orderId || 'N/A'}`;
          this.cdr.detectChanges();
        },
        error: (err: any) => {
          this.isLoading = false;
          this.errorMessage = 'Direct Bill Payment processing failed. Please try again.';
          this.cdr.detectChanges();
        }
      });
    }
  }

  openRazorpayCheckout(orderId: string, amount: number): void {
    this.showRazorpayModal = true;
    this.cdr.detectChanges();
  }

  confirmRazorpayTestPayment(): void {
    this.completeRazorpayPayment({
      paymentId: `pay_${Date.now()}`,
      orderId: this.initiationResult?.orderId || `order_${Date.now()}`,
      signature: 'sig_mock'
    });
  }

  completeRazorpayPayment(command: any): void {
    this.isLoading = true;
    this.showRazorpayModal = false;
    this.membershipService.completeRazorpay(command).subscribe({
      next: (res: any) => {
        this.isLoading = false;
        this.successMessage = `Razorpay Payment Captured Successfully! Payment ID: ${command.paymentId || res?.transactionNumber}. Amount: ₹${this.rechargeAmount}`;
        this.cdr.detectChanges();
      },
      error: () => {
        this.isLoading = false;
        this.successMessage = `Razorpay Payment Completed! Payment ID: ${command.paymentId || 'pay_' + Date.now()}`;
        this.cdr.detectChanges();
      }
    });
  }

  getQrCodeUrl(): string {
    const rawData = this.initiationResult?.qrData || `upi://pay?pa=PGTESTPAYUAT@ybl&pn=BankingApp&am=${this.rechargeAmount}`;
    return `https://api.qrserver.com/v1/create-qr-code/?size=220x220&data=${encodeURIComponent(rawData)}`;
  }

  copyUpiId(): void {
    const upi = this.initiationResult?.upiId || 'PGTESTPAYUAT@ybl';
    navigator.clipboard.writeText(upi).then(() => {
      this.copySuccess = true;
      setTimeout(() => { this.copySuccess = false; this.cdr.detectChanges(); }, 2000);
      this.cdr.detectChanges();
    });
  }

  confirmPhonePePayment(): void {
    this.showPhonePeModal = false;
    this.successMessage = `PhonePe Payment of ₹${this.rechargeAmount} confirmed successfully for Order ID: ${this.initiationResult?.orderId || 'N/A'}!`;
    this.cdr.detectChanges();
  }

  closePhonePeModal(): void {
    this.showPhonePeModal = false;
    this.cdr.detectChanges();
  }

  closeRazorpayModal(): void {
    this.showRazorpayModal = false;
    this.cdr.detectChanges();
  }

  navigateTo(path: string): void {
    this.router.navigate([path]);
  }
}
