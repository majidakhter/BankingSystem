import { Component, Inject, OnInit, PLATFORM_ID, HostListener, ChangeDetectorRef } from '@angular/core';
import { MemberShipService } from '../../core/services/membership.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent implements OnInit {

  isAdmin = false;
  isOperator = false;
  isCustomer = false;
  isUser = false;
  userName: string = 'User';
  lastLoginTime: string = '';
  activeDropdown: string | null = null;

  constructor(
    public authService: MemberShipService,
    private router: Router,
    private cdr: ChangeDetectorRef,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  toggleDropdown(menu: string, event?: Event): void {
    if (event) {
      event.stopPropagation();
      event.preventDefault();
    }
    this.activeDropdown = this.activeDropdown === menu ? null : menu;
  }

  closeDropdowns(): void {
    this.activeDropdown = null;
  }

  navigateToUrl(url: string, event?: Event): void {
    if (event) {
      event.preventDefault();
      event.stopPropagation();
    }
    this.closeDropdowns();
    this.router.navigateByUrl(url);
  }

  onChangePassword(): void {
    alert('Change Password dialog / action.');
  }

  @HostListener('document:click')
  onDocumentClick(): void {
    this.activeDropdown = null;
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  setLastLoginTime(): void {
    const now = new Date();
    this.lastLoginTime = now.toLocaleString('en-GB', {
      day: '2-digit',
      month: '2-digit',
      year: '2-digit',
      hour: '2-digit',
      minute: '2-digit',
      hour12: true
    });
  }

  updateRoles(): void {
    const roleStr = this.authService.getUserRole();
    this.isAdmin = this.authService.isAdmin() || roleStr === 'ADMIN';
    this.isOperator = this.authService.isOperator() || roleStr === 'OPERATOR';
    this.isCustomer = !this.isAdmin && !this.isOperator;
    this.isUser = this.isCustomer;
    this.cdr.detectChanges();
  }

  ngOnInit(): void {
    this.setLastLoginTime();
    this.updateRoles();

    this.authService.userRole$.subscribe(() => {
      this.updateRoles();
    });

    this.authService.currentUser$.subscribe((user: any) => {
      if (user) {
        this.userName = user.name || user.fullName || user.email || 'User';
      }
      this.updateRoles();
    });

    const user: any = this.authService.getUser();
    if (user) {
      this.userName = user.name || user.fullName || user.email || 'User';
    }
  }
}
