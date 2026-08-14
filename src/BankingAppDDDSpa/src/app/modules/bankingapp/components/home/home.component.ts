import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MemberShipService } from '../../../../core/services/membership.service';
import { Role } from '../../../../core/domain/user.model';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
  imports: [CommonModule]
})
export class HomeComponent implements OnInit {
  isLoggedIn: boolean = false;
  isAdmin: boolean = false;
  isOperator: boolean = false;
  showOperatorActions: boolean = false;

  private membershipService = inject(MemberShipService);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  ngOnInit(): void {
    this.checkUserRole();

    this.membershipService.currentUser$.subscribe(() => {
      this.checkUserRole();
    });

    this.membershipService.userRole$.subscribe(() => {
      this.checkUserRole();
    });
  }

  checkUserRole(): void {
    this.isLoggedIn = this.membershipService.isLoggedIn();
    const roleStr = this.membershipService.getUserRole();
    const roleValue = this.membershipService.userRoleValue;

    this.isAdmin = this.membershipService.isAdmin() || roleStr === 'ADMIN' || roleValue === Role.ADMIN;
    this.isOperator = this.membershipService.isOperator() || roleStr === 'OPERATOR' || roleValue === Role.OPERATOR;

    // DEPOSIT, WITHDRAW, and CLOSE buttons are visible strictly for logged-in OPERATOR role, HIDDEN for Admin
    this.showOperatorActions = this.isLoggedIn && this.isOperator && !this.isAdmin;
    this.cdr.detectChanges();
  }

  navigateTo(path: string): void {
    this.router.navigate([path]);
  }
}
