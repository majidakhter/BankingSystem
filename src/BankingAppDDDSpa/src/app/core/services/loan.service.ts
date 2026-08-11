import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable, inject } from "@angular/core";
import { Observable, catchError, throwError } from 'rxjs';
import { LoanModel } from '../domain/loan.model';
import { MemberShipService } from './membership.service';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class LoanService {
  baseUrl = "http://localhost:8084/api/loans/";
  private userService = inject(MemberShipService);
  private http = inject(HttpClient);

  private get loanApiUrl(): string {
    return (environment as any).loanApiUrl || 'http://localhost:5211';
  }

  private getAuthHeaders(): HttpHeaders {
    const token = this.userService.getToken();
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    });
  }

  // --- 1. LoanApplicationController Endpoints ---

  // POST /api/v2/loanapplication/createloanapplication
  createLoanApplication(command: any): Observable<any> {
    const headers = this.getAuthHeaders();
    const url = `${this.loanApiUrl}/api/v2/loanapplication/createloanapplication`;
    return this.http.post<any>(url, command, { headers }).pipe(
      catchError(() => this.http.post<any>(`http://localhost:5263/api/v2/loanapplication/createloanapplication`, command, { headers }))
    );
  }

  // PUT /api/v2/loanapplication/evaluateloanapplication
  evaluateLoanApplication(command: any): Observable<any> {
    const headers = this.getAuthHeaders();
    const url = `${this.loanApiUrl}/api/v2/loanapplication/evaluateloanapplication`;
    return this.http.put<any>(url, command, { headers });
  }

  // PUT /api/v2/loanapplication/acceptloanapplication
  acceptLoanApplication(command: any): Observable<any> {
    const headers = this.getAuthHeaders();
    const url = `${this.loanApiUrl}/api/v2/loanapplication/acceptloanapplication`;
    return this.http.put<any>(url, command, { headers });
  }

  // PUT /api/v2/loanapplication/rejectloanapplication
  rejectLoanApplication(command: any): Observable<any> {
    const headers = this.getAuthHeaders();
    const url = `${this.loanApiUrl}/api/v2/loanapplication/rejectloanapplication`;
    return this.http.put<any>(url, command, { headers });
  }

  // GET /api/v2/loanapplication/getloanbyid/{loanapplicationid}
  getLoanApplicationById(loanApplicationId: string): Observable<any> {
    const headers = this.getAuthHeaders();
    const url = `${this.loanApiUrl}/api/v2/loanapplication/getloanbyid/${loanApplicationId}`;
    return this.http.get<any>(url, { headers });
  }

  // GET /api/v2/loanapplication/getloanbyparam/{applicationnumber}/{customeridentifier}/{decisionbyid}/{registeredbyid}
  getLoanApplicationByParam(
    appNumber: string,
    customerIdentifier: string,
    decisionById: string,
    registeredById: string
  ): Observable<any> {
    const headers = this.getAuthHeaders();
    const url = `${this.loanApiUrl}/api/v2/loanapplication/getloanbyparam/${appNumber}/${customerIdentifier}/${decisionById}/${registeredById}`;
    return this.http.get<any>(url, { headers });
  }

  // POST /api/v2/loanapplication/createdebtorInfo
  createDebtorInfo(command: any): Observable<any> {
    const headers = this.getAuthHeaders();
    const url = `${this.loanApiUrl}/api/v2/loanapplication/createdebtorInfo`;
    return this.http.post<any>(url, command, { headers });
  }

  // --- 2. OperatorController Endpoints ---

  // POST /api/v2/operator
  addOperator(command: { competenceLevelAmount: number }): Observable<any> {
    const headers = this.getAuthHeaders();
    const url = `${this.loanApiUrl}/api/v2/operator`;
    return this.http.post<any>(url, command, { headers });
  }

  // GET /api/v2/operator/getoperators
  getOperators(): Observable<any> {
    const headers = this.getAuthHeaders();
    const url = `${this.loanApiUrl}/api/v2/operator/getoperators`;
    return this.http.get<any>(url, { headers });
  }

  // --- 3. LoanController Endpoints ---

  // GET /api/v2/loan/getloanbyid/{loanapplicationid}
  getLoanById(loanApplicationId: string | number): Observable<any> {
    const headers = this.getAuthHeaders();
    const url = `${this.loanApiUrl}/api/v2/loan/getloanbyid/${loanApplicationId}`;
    return this.http.get<any>(url, { headers }).pipe(
      catchError(() => this.http.get<any>(`${this.baseUrl}${loanApplicationId}`, { headers }))
    );
  }

  // --- Legacy / Compatibility methods ---
  getLoans(): Observable<LoanModel[]> {
    const headers = this.getAuthHeaders();
    return this.http.get<LoanModel[]>(this.baseUrl, { headers });
  }

  getLoansByUserId(userId: number): Observable<LoanModel[]> {
    const headers = this.getAuthHeaders();
    return this.http.get<LoanModel[]>(`${this.baseUrl}user/${userId}`, { headers });
  }

  saveLoan(loan: LoanModel): Observable<LoanModel> {
    const headers = this.getAuthHeaders();
    return this.http.post<LoanModel>(`${this.baseUrl}save`, loan, { headers });
  }

  updateLoan(id: number, loan: LoanModel): Observable<LoanModel> {
    const headers = this.getAuthHeaders();
    return this.http.put<LoanModel>(`${this.baseUrl}update/${id}`, loan, { headers });
  }

  deleteLoan(id: number): Observable<void> {
    const headers = this.getAuthHeaders();
    return this.http.delete<void>(`${this.baseUrl}delete/${id}`, { headers });
  }

  makeLoanPayment(loanId: number, paymentAmount: number): Observable<string> {
    const url = `${this.baseUrl}${loanId}/payment`;
    const params = new HttpParams().set('paymentAmount', paymentAmount.toString());
    return this.http.put<string>(url, {}, { params });
  }
}