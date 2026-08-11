import { Component, OnInit, inject } from '@angular/core';
import { MemberShipService } from '../../../../core/services/membership.service';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from "@angular/common";
import { AuthResponse } from '../../../../core/domain/AuthResponse';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
  imports: [ReactiveFormsModule, CommonModule],
})
export class LoginComponent implements OnInit {

  loginForm!: FormGroup;
  errorMessage: string | null = null;
  successMessage: string | null = null;
  isSubmitting: boolean = false;

  private authService = inject(MemberShipService);
  private router = inject(Router);
  private formBuilder = inject(FormBuilder);

  ngOnInit() {
    this.loginForm = this.formBuilder.group({
      username: ['', [Validators.required]],
      password: ['', [Validators.required]]
    });
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.errorMessage = null;
    this.successMessage = null;
    this.isSubmitting = true;

    const { username, password } = this.loginForm.value;

    this.authService.login(username, password).subscribe({
      next: (response: AuthResponse) => {
        const token = response?.token || response?.access_token || response?.accessToken || this.authService.getToken();
        if (token) {
          const decodedToken = this.authService.decodeToken(token);
          const keyCloakUserId = decodedToken?.sub || this.authService.getKeyCloakUserId();
          console.log('Decoded Keycloak JWT Token payload:', decodedToken);
          console.log('Extracted Keycloak User ID (sub):', keyCloakUserId);

          this.successMessage = 'Login successful!';

          const role = this.authService.getUserRole();
          if (this.authService.isAdmin() || role === 'ADMIN') {
            this.router.navigate(['/home']);
          } else {
            this.router.navigate(['/dashboard']);
          }
        } else {
          this.isSubmitting = false;
          this.errorMessage = 'Login failed. No authentication token was received.';
        }
      },
      error: (err) => {
        this.isSubmitting = false;
        this.errorMessage = err?.error?.message || err?.error?.title || err?.message || 'Login failed. Please check your credentials.';
        this.successMessage = null;
      }
    });
  }

}
