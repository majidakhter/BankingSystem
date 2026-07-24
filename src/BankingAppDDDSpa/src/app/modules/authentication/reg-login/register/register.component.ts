import { Component, inject, OnInit } from '@angular/core';
import { MemberShipService } from '../../../../core/services/membership.service';
import { Router } from '@angular/router';
import {CommonModule} from "@angular/common";
import { FormBuilder, FormGroup,ReactiveFormsModule, Validators } from '@angular/forms';
import { AddressComponent } from '../address/address.component';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
  imports: [ReactiveFormsModule, CommonModule, AddressComponent],
})
export class RegisterComponent implements OnInit {
  registerForm!: FormGroup;
  errorMessage: string | null = null;
  successMessage: string | null = null;
  selectedImage: File | null = null;
  private authService = inject(MemberShipService);
  private router = inject(Router);
  private formBuilder = inject(FormBuilder);

    ngOnInit() {
    this.registerForm = this.formBuilder.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      mobileNo: [''],
      address: [''],
      dob: [''],
      gender: [''],
      image: [''],
      nid: [''],
      accountType: [''],
      createDate: [new Date()],
    });
  }
  onImageSelected(event: any): void {
      this.selectedImage = event.target.files[0];
    }

  onSubmit() { 
    if (this.registerForm.invalid) {
      return;
    }

    const { firstName, lastName, email, mobileNo, address, dob, gender, image, nid, accountType, createDate } = this.registerForm.value;

    this.authService.register({ firstName, lastName, email, mobileNo, address, dob, gender, image, nid, accountType, createDate }).subscribe(
   {

    next: AuthResponse => {
      this.successMessage = 'Registration successful! Please check your email to activate your account.';
      this.router.navigate(['/login']);
    },
    error:error => {
      this.errorMessage = 'Registration failed. Please try again.';
    }

   }
    );
  }


  

}
