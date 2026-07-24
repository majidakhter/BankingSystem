import { Component, Inject, OnInit, PLATFORM_ID } from '@angular/core';
import { MemberShipService } from '../../core/services/membership.service';
import { Router } from '@angular/router';


@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})

export class HeaderComponent implements OnInit{

  isAdmin = false;
  isUser = false;

  userRole: string | null = null;


  constructor( public authService: MemberShipService,
    private router:Router,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  logout(): void {
    this.authService.logout(); // Call the logout method from AuthService
    this.router.navigate(['/login']);
  }


  ngOnInit(): void {
    this.authService.userRole$.subscribe(role => {
      this.isAdmin = role === 'ADMIN';
      // this.isUser = this.authService.isUser();
      this.isUser = role === 'USER';
    });
  }
}
