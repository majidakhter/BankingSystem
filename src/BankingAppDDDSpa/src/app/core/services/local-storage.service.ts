 import { Injectable } from '@angular/core';

 @Injectable({
   providedIn: 'root'
 })

export class LocalStorageService {
  public setItem(key: string, value: any) {
    localStorage.setItem(key, JSON.stringify(value));
  }

  public getItem(key: string): string | null {
    const item = localStorage.getItem(key);
    return item ? JSON.parse(item) : null;
  }
  public clearKey(key: string): void {
    localStorage.removeItem(key);
  }

  public clearAllKeys(): void {
    localStorage.clear();
  }

  public clearLocalStorage() {
    localStorage.clear();
  }
}