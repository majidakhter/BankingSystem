import { Injectable, inject } from "@angular/core";
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TransactionModel } from '../domain/transaction.model';
import { MemberShipService } from './membership.service';


@Injectable({
  providedIn: 'root'
})
export class TransactionService {
  baseUrl = 'http://localhost:8084/api/transactions/';
  apiUrl = 'http://localhost:5210/api/v2/account/'
  private http = inject(HttpClient);
  private userService = inject(MemberShipService);

  private getAuthHeaders(): HttpHeaders {
    const token = this.userService.getToken();
    console.log(token);
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    });
  }


  getTransactions(): Observable<TransactionModel[]> {
    const headers = this.getAuthHeaders();
    return this.http.get<TransactionModel[]>(this.baseUrl);
  }


  getTransactionById(id: number): Observable<TransactionModel> {
    const headers = this.getAuthHeaders();
    return this.http.get<TransactionModel>(`${this.baseUrl}${id}`, { headers });
  }


  getTransactionsByUserId(userId: number): Observable<TransactionModel[]> {
    const headers = this.getAuthHeaders();
    return this.http.get<TransactionModel[]>(`${this.baseUrl}user/${userId}`, { headers });
  }

  depositMoney(accountNo: number, amount: number, description: string): Observable<any> {
    const url = `${this.apiUrl}deposit`;
    const desc = description && description.trim() ? description.trim() : 'Deposit funds';

    const payload = {
      accountNumber: Number(accountNo),
      amount: Number(amount),
      description: desc
    };

    return this.http.post(url, payload);
  }

  withdrawMoney(accountNo: number, amount: number, description: string): Observable<any> {
    const url = `${this.apiUrl}withdraw`;
    const desc = description && description.trim() ? description.trim() : 'Withdraw funds';

    const payload = {
      accountNumber: Number(accountNo),
      amount: Number(amount),
      description: desc
    };

    return this.http.post(url, payload);
  }

  transferMoney(senderId: number, receiverId: number, amount: number, description: string): Observable<any> {
    const url = `${this.baseUrl}transfer`;

    // Setting up query parameters
    let params = new HttpParams()
      .set('senderId', senderId.toString())
      .set('receiverId', receiverId.toString())
      .set('amount', amount.toString())
      .set('description', description);

    // Making the POST request with parameters
    return this.http.post(url, {}, { params });
  }

  updateTransactionStatus(transactionId: number, status: string): Observable<void> {
    const headers = this.getAuthHeaders();
    return this.http.put<void>(`${this.baseUrl}${transactionId}/status`, { status }, { headers });
  }

  deleteTransaction(id: number): Observable<void> {
    const headers = this.getAuthHeaders();
    return this.http.delete<void>(`${this.baseUrl}${id}`, { headers });
  }
}