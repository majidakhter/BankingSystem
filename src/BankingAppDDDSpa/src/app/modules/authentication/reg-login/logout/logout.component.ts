import { Component, OnInit, inject } from '@angular/core';
import { MemberShipService } from '../../../../core/services/membership.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-logout',
  templateUrl: './logout.component.html',
  styleUrl: './logout.component.css'
})
export class LogoutComponent implements OnInit{

  private authService = inject(MemberShipService);
  private router = inject(Router);

  ngOnInit(): void {
    this.logout();
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['login']);
  }

}
