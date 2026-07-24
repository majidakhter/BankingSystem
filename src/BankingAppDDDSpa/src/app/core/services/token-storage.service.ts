import { Injectable, inject } from '@angular/core';
import { LocalStorageService } from './local-storage.service';

const TOKEN_KEY = 'auth-token';
@Injectable({
  providedIn: 'root'
})

export class TokenStorageService{
  private localStorageService = inject(LocalStorageService);
  

  public setToken(token: string): void {
    this.localStorageService.setItem(TOKEN_KEY, token);
  }

  public getToken(): string | null {
    return this.localStorageService.getItem(TOKEN_KEY);
  }

  public clearToken(): void {
    this.localStorageService.clearKey(TOKEN_KEY);
    return;
  }
}