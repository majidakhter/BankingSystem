import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Inject, Injectable, PLATFORM_ID } from '@angular/core';
import { BehaviorSubject, map, Observable } from 'rxjs';
import { User } from '../domain/user.model';
import { Router } from '@angular/router';
import { isPlatformBrowser } from '@angular/common';
import { AuthResponse } from '../domain/AuthResponse';

@Injectable({
  providedIn: 'root'
})
export class MemberShipService {

  //private apiUrl = 'http://localhost:8084';
  private apiUrl = 'http://localhost:5157';
  private headers = new HttpHeaders({ 'Content-Type': 'application/json' });

  private userRoleSubject: BehaviorSubject<string | null> = new BehaviorSubject<string | null>(null);
  public userRole$: Observable<string | null> = this.userRoleSubject.asObservable();

  private currentUserSubject: BehaviorSubject<User | null> = new BehaviorSubject<User | null>(null);
  public currentUser$: Observable<User | null> = this.currentUserSubject.asObservable();
  

  constructor(
    @Inject(PLATFORM_ID) private platformId: Object,
    private http: HttpClient,
    private router: Router
  ) {
    // Initialize the role from localStorage
    if (this.isBrowser()) {
      const storedRole = localStorage.getItem('userRole');
      this.userRoleSubject.next(storedRole);
    }
    // const storedRole = this.isBrowser() ? localStorage.getItem('userRole') : null;
    // this.userRoleSubject.next(storedRole);
  }
  private isBrowser(): boolean {
    return isPlatformBrowser(this.platformId);
  }

  

  decodeToken(token: string): any {
    const payload = token.split('.')[1];
    return JSON.parse(atob(payload));
  }

  login(email: string, password: string): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${this.apiUrl}/login`, { email, password }, { headers: this.headers })
      .pipe(
        map((response: AuthResponse) => {
          if (this.isBrowser() && response.token) {
            localStorage.setItem('authToken', response.token);
            const decodedToken = this.decodeToken(response.token);
            localStorage.setItem('userRole', decodedToken.role);
            this.userRoleSubject.next(decodedToken.role); // Update role in BehaviorSubject
            localStorage.setItem('user', JSON.stringify(response.user));
          }
          return response;
        })
      );
  }

  getUserRole(): string | null {
    if (this.isBrowser()) {
      return localStorage.getItem('userRole');
    }
    return null;
  }

  getUser(): User | null {
    
    let user = localStorage.getItem('user');
    if (user != null) {
      return JSON.parse(user);
    } else {
      return null;
    }
  }
  isAdmin(): boolean {
    return this.getUserRole() === 'ADMIN';
  }

  isUser(): boolean {
    return this.getUserRole() === 'USER';
  }

  // updateUserInLocalStorage(updatedUser: User): void {
  //   if (this.isBrowser()) {
  //     localStorage.setItem('user', JSON.stringify(updatedUser));
  //   }
  // }

  
  register(user: { firstName: string; lastName: string, email: string; mobileNo: string; address: string; dob: Date; gender: string; image: string; nid: string; accountType: string; createDate: Date; }): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/api/v2/customer`,
      user, { headers: this.headers }).pipe(
        map((response: AuthResponse) => {
          if (this.isBrowser() && response.token) {
            localStorage.setItem('authToken', response.token); // Store JWT token
            console.log(localStorage.getItem('authToken')+"***********************");
          }
          return response;
        })
      );
  }
  registerAdmin(user: { firstName: string; lastName: string, email: string; password: string; mobileNo: string; address: string; dob: Date; gender: string; image: string; nid: string}): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/register/admin`,
      user, { headers: this.headers }).pipe(
        map((response: AuthResponse) => {
          if (this.isBrowser() && response.token) {
            localStorage.setItem('authToken', response.token); // Store JWT token
            console.log(localStorage.getItem('authToken')+"admin");
          }
          return response;
        })
      );
  }
  getToken(): string | null {
    if (this.isBrowser()) {
      return localStorage.getItem('authToken');
    }
    return null;
  }
  isTokenExpired(token: string): boolean {
    const decodedToken = this.decodeToken(token);
    const expiry = decodedToken.exp * 1000; 
    return Date.now() > expiry;
  }

  isLoggedIn(): boolean {
    const token = this.getToken();
    if (token && !this.isTokenExpired(token)) {
      return true;
    }
    this.logout(); 
    return false;
  }
  
  logout(): void {
    if (this.isBrowser()) {
      localStorage.removeItem('authToken');
      localStorage.removeItem('userRole');
      this.userRoleSubject.next(null); 
    }
    this.router.navigate(['/login']);
  }

  hasRole(roles: string): boolean {
    const userRole = this.getUserRole();
    return userRole ? roles.includes(userRole) : false;
  }

}
