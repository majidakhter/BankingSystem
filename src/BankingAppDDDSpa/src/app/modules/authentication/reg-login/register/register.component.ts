import { Component, inject, OnInit, ViewChild } from '@angular/core';
import { MemberShipService } from '../../../../core/services/membership.service';
import { Router } from '@angular/router';
import { CommonModule } from "@angular/common";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AddressComponent } from '../address/address.component';
import { ApiResponse } from '@core/domain/apiresponse';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
  imports: [ReactiveFormsModule, CommonModule, AddressComponent],
})
export class RegisterComponent implements OnInit {
  registerForm!: FormGroup;
  branches: any[] = [];
  errorMessage: string | null = null;
  successMessage: string | null = null;
  selectedImage: File | null = null;
  private authService = inject(MemberShipService);
  private router = inject(Router);
  private formBuilder = inject(FormBuilder);
  @ViewChild(AddressComponent) childAddressComponent!: AddressComponent;

  ngOnInit() {
    this.registerForm = this.formBuilder.group({
      userName: ['', Validators.required],
      password: ['', Validators.required],
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phoneNo: ['1234567890'],
      dateofBirth: ['1990-01-01'],
      gender: ['male'],
      ssnNumber: ['999-99-9999'],
      branchId: ['', Validators.required],
      accountType: ['1'],
      amount: [500],
      address: [{
        street: '123 Main St',
        city: 'New York',
        state: 'NY',
        zipcode: '10001',
        country: 'USA'
      }]
    });

    // Fetch branches dynamically on initialization
    this.authService.getBranchDetails().subscribe({
      next: (res: any) => {
        if (Array.isArray(res)) {
          this.branches = res;
        } else if (res?.result && Array.isArray(res.result)) {
          this.branches = res.result;
        } else if (res?.data && Array.isArray(res.data)) {
          this.branches = res.data;
        }
        console.log('Loaded branches:', this.branches);
      },
      error: (err) => {
        console.error('Failed to load branches:', err);
      }
    });
  }

  onImageSelected(event: any): void {
    if (event.target.files && event.target.files.length > 0) {
      this.selectedImage = event.target.files[0];
    }
  }

  onSubmit(): void {
    console.log('Register onSubmit fired. Form valid:', this.registerForm.valid);

    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      if (this.childAddressComponent?.addressForm) {
        this.childAddressComponent.addressForm.markAllAsTouched();
      }
      this.errorMessage = 'Please fill in all required fields correctly before submitting.';
      return;
    }

    this.errorMessage = null;
    this.successMessage = null;

    const formValues = this.registerForm.value;
    const addressValues = this.childAddressComponent?.addressForm?.value || formValues.address || {};

    const formData = new FormData();
    formData.append('userData.UserName', formValues.userName || '');
    formData.append('userData.Password', formValues.password || '');
    formData.append('userData.Email', formValues.email || '');
    formData.append('userData.FirstName', formValues.firstName || '');
    formData.append('userData.LastName', formValues.lastName || '');
    formData.append('userData.PhoneNo', formValues.phoneNo || '1234567890');
    
    const dobValue = formValues.dateofBirth ? new Date(formValues.dateofBirth).toISOString().split('T')[0] : '1990-01-01';
    formData.append('userData.DateOfBirth', dobValue);
    formData.append('userData.UserType', '1');
    formData.append('userData.Gender', formValues.gender || 'male');
    formData.append('userData.SSNumber', formValues.ssnNumber || '999-99-9999');

    if (formValues.branchId) {
      formData.append('branchId', formValues.branchId);
    }

    if (this.selectedImage) {
      formData.append('userData.ProfileImage', this.selectedImage, this.selectedImage.name);
    } else {
      formData.append('userData.ProfileImage', new Blob([], { type: 'image/png' }), 'default.png');
    }

    formData.append('addressData.street', addressValues.street || '123 Main St');
    formData.append('addressData.city', addressValues.city || 'New York');
    formData.append('addressData.state', addressValues.state || 'NY');
    formData.append('addressData.zipCode', addressValues.zipcode || addressValues.zipCode || '10001');
    formData.append('addressData.country', addressValues.country || 'USA');

    const accountTypeIdVal = parseInt(formValues.accountType, 10) || 1;
    formData.append('accountTypeId', accountTypeIdVal.toString());
    formData.append('amount', (formValues.amount || 500).toString());

    this.authService.openaccount(formData).subscribe({
      next: (res: ApiResponse) => {
        this.successMessage = 'Registration & Account opening successful! Please proceed to login.';
        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 1500);
      },
      error: (err) => {
        this.errorMessage = err?.error?.message || err?.error?.title || err?.message || 'Registration failed. Please try again.';
      }
    });
  }
}
