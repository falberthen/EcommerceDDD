import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class LocalStorageService {
  public setValue(key: string, value: any) {
    localStorage.setItem(key, value);
  }

  public getValueByKey(key: string): string | null {
    const value = localStorage.getItem(key);
    return value;
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
