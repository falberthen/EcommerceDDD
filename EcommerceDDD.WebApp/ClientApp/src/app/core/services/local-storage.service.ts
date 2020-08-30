import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LocalStorageService {

  public setValue(key: string, value: any) {
    localStorage.setItem(key, value);
  }

  public getValueByKey(key: string): string {
    return localStorage.getItem(key);
  }

  public clearKey(key: string): void {
    localStorage.removeItem(key);
  }

  public clearLocalStorage() {
    localStorage.clear();
  }
}
