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
  searchBillerQuery: string = '';
  
  isLoading: boolean = false;
  successMessage: string = '';
  errorMessage: string = '';

  private membershipService = inject(MemberShipService);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  ngOnInit(): void {}

  setCategory(cat: string): void {
    this.rechargeCategory = cat;
  }

  onRechargeNow(): void {
    this.errorMessage = '';
    this.successMessage = '';

    if (!this.mobileNumber || this.mobileNumber.length < 10) {
      this.errorMessage = 'Please enter a valid 10-digit Mobile / Account Number.';
      return;
    }

    this.isLoading = true;
    setTimeout(() => {
      this.isLoading = false;
      this.successMessage = `Recharge request of ₹299 for ${this.rechargeCategory} (${this.mobileNumber}) initiated successfully!`;
      this.cdr.detectChanges();
    }, 1000);
  }

  navigateTo(path: string): void {
    this.router.navigate([path]);
  }
}
