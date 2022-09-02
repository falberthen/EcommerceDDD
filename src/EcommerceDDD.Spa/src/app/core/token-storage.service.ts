import { Injectable } from '@angular/core';
import { LocalStorageService } from './services/local-storage.service';

const TOKEN_KEY = 'auth-token';

@Injectable({
  providedIn: 'root'
})
export class TokenStorageService {

  constructor(private localStorageService: LocalStorageService) { }

  public setToken(token: string) {
    this.localStorageService
      .setValue(TOKEN_KEY, token);
  }

  public getToken(): string {
    return this.localStorageService
      .getValueByKey(TOKEN_KEY);
  }

  public clearToken() {
    this.localStorageService
      .clearKey(TOKEN_KEY);
      return;
  }
}
