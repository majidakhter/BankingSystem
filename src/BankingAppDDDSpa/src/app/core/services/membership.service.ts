import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, Observable, of, tap, catchError } from 'rxjs';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../domain/apiresponse';
import { User, Role } from '../domain/user.model';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';

@Injectable({
  providedIn: 'root',
})
export class MemberShipService {
  private http = inject(HttpClient);
  private router = inject(Router);
  private sanitizer = inject(DomSanitizer);

  private apiUrl = environment.apiUrl || 'http://localhost:5263';
  private userApiUrl = (environment as any).userApiUrl || 'http://localhost:5157';
  private currentUserSubject = new BehaviorSubject<User | null>(this.getUserFromStorage());
  public currentUser$ = this.currentUserSubject.asObservable();

  private userRoleSubject = new BehaviorSubject<Role | null>(this.getRoleFromStorage());
  public userRole$ = this.userRoleSubject.asObservable();

  private headers = new HttpHeaders({
    'Content-Type': 'application/json',
  });

  private getUserFromStorage(): User | null {
    const userJson = localStorage.getItem('user');
    if (!userJson) return null;
    try {
      return JSON.parse(userJson);
    } catch {
      return null;
    }
  }

  private getRoleFromStorage(): Role | null {
    const role = localStorage.getItem('userRole');
    if (role === null || role === undefined) return null;
    if (role === 'ADMIN' || role === 'Admin' || role === '0') return Role.ADMIN;
    return Role.USER;
  }

  public get currentUserValue(): User | null {
    return this.currentUserSubject.value;
  }

  public get userRoleValue(): Role | null {
    return this.userRoleSubject.value;
  }

  public getUser(): User | null {
    return this.currentUserValue || this.getUserFromStorage();
  }

  public getToken(): string | null {
    return localStorage.getItem('token');
  }

  public getKeyCloakUserId(): string | null {
    return localStorage.getItem('keyCloakUserId');
  }

  public getUserRole(): string | null {
    const roleFromStorage = localStorage.getItem('userRole');
    if (roleFromStorage) return roleFromStorage;
    const role = this.userRoleValue ?? this.getRoleFromStorage();
    return role === Role.ADMIN ? 'ADMIN' : 'Customer';
  }

  public isAdmin(): boolean {
    const role = this.getUserRole();
    return role === 'ADMIN' || role === 'Admin' || role === '0';
  }

  public decodeToken(token: string): any {
    if (!token) return null;
    try {
      const parts = token.split('.');
      if (parts.length !== 3) return null;
      const payload = parts[1];
      const base64 = payload.replace(/-/g, '+').replace(/_/g, '/');
      const jsonPayload = decodeURIComponent(
        atob(base64)
          .split('')
          .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
          .join('')
      );
      return JSON.parse(jsonPayload);
    } catch (e) {
      return null;
    }
  }

  public isLoggedIn(): boolean {
    return !!this.currentUserValue;
  }

  login(credentialsOrUsername: any, passwordArg?: string): Observable<any> {
    let username = '';
    let password = '';

    if (typeof credentialsOrUsername === 'string') {
      username = credentialsOrUsername;
      password = passwordArg || '';
    } else if (credentialsOrUsername && typeof credentialsOrUsername === 'object') {
      username = credentialsOrUsername.username || credentialsOrUsername.email || '';
      password = credentialsOrUsername.password || '';
    }

    const loginPayload = {
      username: username,
      password: password,
    };

    return this.http.post<any>(`${this.apiUrl}/api/v2/auth/login`, loginPayload).pipe(
      tap((res: any) => {
        const token = res?.access_token || res?.token || res?.result?.token || res?.data?.token || res?.accessToken;
        const user = res?.result?.user || res?.user || res?.data?.user;

        if (token) {
          localStorage.setItem('token', token);
        }

        const decoded = this.decodeToken(token);
        console.log('Decoded Token on Login:', decoded);

        const rolesRaw = res?.roles || res?.role || decoded?.realm_access?.roles || decoded?.roles || user?.role || [];
        let roles: string[] = [];
        if (Array.isArray(rolesRaw)) {
          roles = rolesRaw.map(r => String(r).toUpperCase());
        } else if (rolesRaw) {
          roles = [String(rolesRaw).toUpperCase()];
        }

        const isAdminUser = roles.some(r => r === 'ADMIN' || r === 'ROLE_ADMIN' || r === '0');
        const userRole = isAdminUser ? Role.ADMIN : Role.USER;
        const userRoleString = isAdminUser ? 'ADMIN' : 'Customer';
        const keyCloakUserId = decoded?.sub || res?.preferred_username || user?.keyCloakUserId || user?.id || '';

        const loggedInUser: User = {
          id: user?.id || user?.userId || keyCloakUserId || '',
          email: user?.email || decoded?.email || username || '',
          name: user?.name || user?.fullName || `${user?.firstName || ''} ${user?.lastName || ''}`.trim() || username || 'User',
          role: userRole,
          token: token,
          image: user?.image || user?.profileImage
        };

        localStorage.setItem('user', JSON.stringify(loggedInUser));
        localStorage.setItem('userRole', userRoleString);
        if (keyCloakUserId) {
          localStorage.setItem('keyCloakUserId', keyCloakUserId);
        }

        this.currentUserSubject.next(loggedInUser);
        this.userRoleSubject.next(userRole);
      })
    );
  }

  getUserProfile(userId: string): Observable<any> {
    const accountUrl = (environment as any).accountApiUrl || 'http://localhost:5210';
    return this.http.get<any>(`${this.userApiUrl}/api/v2/user/getuserprofile/${userId}`).pipe(
      catchError(() => this.http.get<any>(`${accountUrl}/api/v2/user/getaccountdetails/${userId}`))
    );
  }

  getAccountDetails(userId: string): Observable<any> {
    const accountUrl = (environment as any).accountApiUrl || 'http://localhost:5210';
    return this.http.get<any>(`${accountUrl}/api/v2/user/getaccountdetails/${userId}`).pipe(
      catchError(() => this.http.get<any>(`${this.userApiUrl}/api/v2/user/getaccountdetails/${userId}`))
    );
  }

  addBeneficiary(command: any): Observable<any> {
    const accountUrl = (environment as any).accountApiUrl || 'http://localhost:5210';
    return this.http.post<any>(`${accountUrl}/api/v2/account/addbeneficiary`, command).pipe(
      catchError(() => this.http.post<any>(`${this.userApiUrl}/api/v2/account/addbeneficiary`, command))
    );
  }

  getBranchDetails(): Observable<any> {
    const accountUrl = (environment as any).accountApiUrl || 'http://localhost:5210';
    return this.http.get<any>(`${accountUrl}/api/v2/branch/branchdetails`).pipe(
      catchError(() => this.http.get<any>(`${accountUrl}/api/v2/bank/bankdetails`))
    );
  }

  getBeneficiaries(accountId?: string): Observable<any> {
    const accountUrl = (environment as any).accountApiUrl || 'http://localhost:5210';
    const url = accountId ? `${accountUrl}/api/v2/account/getbeneficiaries/${accountId}` : `${accountUrl}/api/v2/account/getbeneficiaries`;
    return this.http.get<any>(url);
  }

  getAllAccounts(): Observable<any> {
    const accountUrl = (environment as any).accountApiUrl || 'http://localhost:5210';
    return this.http.get<any>(`${accountUrl}/api/v2/account/accountlist`);
  }

  transferFunds(command: any): Observable<any> {
    const accountUrl = (environment as any).accountApiUrl || 'http://localhost:5210';
    return this.http.post<any>(`${accountUrl}/api/v2/transaction`, command, { headers: this.headers });
  }


  getCountries(): Observable<any> {
    const accountUrl = (environment as any).accountApiUrl || 'http://localhost:5210';
    return this.http.get<any>(`${accountUrl}/api/v2/location/countries`).pipe(
      catchError(() => of([
        { id: 1, name: 'India', code: 'IN' },
        { id: 2, name: 'USA', code: 'US' },
        { id: 3, name: 'UK', code: 'GB' },
        { id: 4, name: 'Canada', code: 'CA' },
        { id: 5, name: 'Australia', code: 'AU' }
      ]))
    );
  }

  getStates(): Observable<any> {
    const accountUrl = (environment as any).accountApiUrl || 'http://localhost:5210';
    return this.http.get<any>(`${accountUrl}/api/v2/location/states`).pipe(
      catchError(() => of([
        { id: 1, name: 'West Bengal', code: 'WB', countryId: 1 },
        { id: 2, name: 'Maharashtra', code: 'MH', countryId: 1 },
        { id: 3, name: 'Karnataka', code: 'KA', countryId: 1 },
        { id: 4, name: 'Bihar', code: 'BR', countryId: 1 },
        { id: 5, name: 'Delhi', code: 'DL', countryId: 1 },
        { id: 6, name: 'Tamil Nadu', code: 'TN', countryId: 1 },
        { id: 7, name: 'New York', code: 'NY', countryId: 2 },
        { id: 8, name: 'California', code: 'CA', countryId: 2 }
      ]))
    );
  }



  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('userRole');
    localStorage.removeItem('user');
    localStorage.removeItem('keyCloakUserId');
    this.currentUserSubject.next(null);
    this.userRoleSubject.next(null);
    this.router.navigate(['/login']);
  }

  openaccount(command: any): Observable<any> {
    const formData = command instanceof FormData ? command : this.buildRegisterUserFormData(command);
    return this.http.post<any>(`${this.apiUrl}/api/v2/auth/openaccount`, formData);
  }

  registerAdmin(command: any): Observable<any> {
    return this.http.post<any>(`${this.userApiUrl}/api/v2/user/registeradmin`, command, { headers: this.headers });
  }

  private buildRegisterUserFormData(command: any): FormData {
    const formData = new FormData();
    const data = command.userData || command;

    formData.append('userData.UserName', data.UserName || data.userName || '');
    formData.append('userData.Password', data.Password || data.password || '');
    formData.append('userData.Email', data.Email || data.email || '');
    formData.append('userData.FirstName', data.FirstName || data.firstName || '');
    formData.append('userData.LastName', data.LastName || data.lastName || '');
    formData.append('userData.PhoneNo', data.PhoneNo || data.phoneNo || data.mobileNo || '');

    if (data.DateOfBirth || data.dateOfBirth) {
      formData.append('userData.DateOfBirth', data.DateOfBirth || data.dateOfBirth);
    }

    formData.append('userData.UserType', (data.UserType !== undefined ? data.UserType : 1).toString());
    formData.append('userData.Gender', data.Gender || data.gender || 'Male');
    formData.append('userData.SSNumber', data.SSNumber || data.ssNumber || data.nid || '');

    if (data.ProfileImage && data.ProfileImage instanceof File) {
      formData.append('userData.ProfileImage', data.ProfileImage, data.ProfileImage.name);
    } else {
      const dummyFile = new File([''], 'default.png', { type: 'image/png' });
      formData.append('userData.ProfileImage', dummyFile);
    }

    return formData;
  }
}
