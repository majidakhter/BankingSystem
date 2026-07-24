import { Injectable, inject } from '@angular/core';

@Injectable({
    providedIn: 'root',
})
export class LoanApiService {
  private apiUrl = 'https://api.example.com/loans'; // Replace with your actual API URL
}