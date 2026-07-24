import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
    providedIn: 'root',
})
export class CustomerApiService {
  private apiUrl = 'https://api.example.com/customers'; // Replace with your actual API URL

  constructor(private http: HttpClient) {}
}