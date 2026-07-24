import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable, inject } from "@angular/core";
import { Observable } from 'rxjs';
import { LoanModel } from '../domain/loan.model';
import { MemberShipService } from './membership.service';

@Injectable({
    providedIn:'root'
})
export class LoanService{
   baseUrl = "http://localhost:8084/api/loans/"
   private userService = inject(MemberShipService);
   private http= inject(HttpClient);

  private getAuthHeaders(): HttpHeaders {
    const token = this.userService.getToken();
    console.log(token);
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    });
  }

  getLoans(): Observable<LoanModel[]> {
    const headers = this.getAuthHeaders();
    return this.http.get<LoanModel[]>(this.baseUrl);
  }

  
  getLoanById(id: number): Observable<LoanModel> {
    const headers = this.getAuthHeaders();
    return this.http.get<LoanModel>(`${this.baseUrl}${id}`, {headers});
  }

  
  getLoansByUserId(userId: number): Observable<LoanModel[]> {
    const headers = this.getAuthHeaders();
    return this.http.get<LoanModel[]>(`${this.baseUrl}user/${userId}`, {headers});
  }

  
  saveLoan(loan: LoanModel): Observable<LoanModel> {
    const headers = this.getAuthHeaders();
    return this.http.post<LoanModel>(`${this.baseUrl}save`, loan, {headers});
  }

  updateLoan(id: number, loan: LoanModel): Observable<LoanModel> {
    const headers = this.getAuthHeaders();
    return this.http.put<LoanModel>(`${this.baseUrl}update/${id}`, loan, {headers});
  }  
  
  deleteLoan(id: number): Observable<void> {
    const headers = this.getAuthHeaders();
    return this.http.delete<void>(`${this.baseUrl}delete/${id}`, {headers});
  }

  makeLoanPayment(loanId: number, paymentAmount: number): Observable<string> {
    
    const url = `${this.baseUrl}${loanId}/payment`;

    // Set up query parameters
    const params = new HttpParams().set('paymentAmount', paymentAmount.toString());

    // Make the PUT request with parameters
    return this.http.put<string>(url, {}, { params });
  }
}