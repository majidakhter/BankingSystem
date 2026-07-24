import { Component,OnInit, inject } from '@angular/core';
import { FormBuilder, FormGroup,ReactiveFormsModule, Validators } from '@angular/forms';
import { MemberShipService } from '../../../../core/services/membership.service';
import { Router } from '@angular/router';
import {CommonModule} from "@angular/common";

@Component({
  selector: 'app-admin-register',
  templateUrl: './admin-register.component.html',
  styleUrl: './admin-register.component.css',
  imports: [ReactiveFormsModule, CommonModule],
})
export class AdminRegisterComponent implements OnInit{

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
      password: ['', Validators.required],
      confirmPassword: ['', Validators.required],
      mobileNo: [''],
      address: [''],
      dob: [''],
      gender: [''],
      image: [''],
      nid: ['']
    }
    ,{ validators: this.passwordMatchValidator });
  }
  
  onImageSelected(event: any): void {
      this.selectedImage = event.target.files[0];
    }

  passwordMatchValidator(formGroup: FormGroup) {
    const password = formGroup.get('password')?.value;
    const confirmPassword = formGroup.get('confirmPassword')?.value;
    return password === confirmPassword ? null : { mismatch: true };
  }

  onSubmit() {
    if (this.registerForm.invalid) {
      return;
    }

    const { firstName, lastName, email, password, mobileNo, address, dob, gender, image, nid } = this.registerForm.value;

    this.authService.registerAdmin({ firstName, lastName, email, password, mobileNo, address, dob, gender, image, nid,  }).subscribe(
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
