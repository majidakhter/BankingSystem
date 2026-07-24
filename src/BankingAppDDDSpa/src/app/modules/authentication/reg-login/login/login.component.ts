import { Component, OnInit, inject} from '@angular/core';
import { MemberShipService } from '../../../../core/services/membership.service';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup,ReactiveFormsModule, Validators } from '@angular/forms';
import {CommonModule} from "@angular/common";
@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
  imports: [ReactiveFormsModule, CommonModule],
})
export class LoginComponent implements OnInit{

  loginForm!: FormGroup;
  errorMessage: string | null = null;
  successMessage: string | null = null;
  private authService = inject(MemberShipService);
  private router = inject(Router);
  private formBuilder = inject(FormBuilder);

  ngOnInit() {
    this.loginForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required]
    });
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      return;
    }

    const { email, password } = this.loginForm.value;

    this.authService.login(email, password).subscribe({
      next: (response) => {
       

        // Check user role and navigate accordingly
        if (this.authService.isAdmin()) {
          this.router.navigate(['/home']);
        } else if (this.authService.isUser()) {
          this.router.navigate(['/user-profile']);
        }
      },
      error: (err) => {
        this.errorMessage = 'Login failed. Please check your credentials.';
        this.successMessage = null;
      }
    });
  }

}
