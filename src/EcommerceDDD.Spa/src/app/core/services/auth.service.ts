import { Injectable, inject } from '@angular/core';
import { Subject } from 'rxjs';
import { LocalStorageService } from './local-storage.service';
import { TokenStorageService } from './token-storage.service';
import { Router } from '@angular/router';
import { NotificationService } from './notification.service';
import {
  LOCAL_STORAGE_ENTRIES,
  ROUTE_PATHS,
} from '@ecommerce/constants/appConstants';
import { KiotaClientService } from './kiota-client.service';
import { CustomerDetails, LoginResult } from 'src/app/clients/models';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private kiotaClientService = inject(KiotaClientService);
  private tokenStorageToken = inject(TokenStorageService);
  private localStorageService = inject(LocalStorageService);
  private router = inject(Router);
  private notificationService = inject(NotificationService);

  private isLogged = new Subject<boolean>();

  // Observable string streams
  isLoggedAnnounced$ = this.isLogged.asObservable();

  public get currentUser(): string | null {
    const storedUser = this.localStorageService.getValueByKey(
      LOCAL_STORAGE_ENTRIES.storedUser
    );
    return storedUser;
  }

  public get currentCustomer(): CustomerDetails | null {
    const storedCustomerData = this.localStorageService.getValueByKey(
      LOCAL_STORAGE_ENTRIES.storedCustomer
    );

    if (storedCustomerData) {
      var storedCustomer = JSON.parse(storedCustomerData);
      return storedCustomer as CustomerDetails;
    }

    return null;
  }

  async login(email: string, password: string): Promise<boolean> {
    let isLogged = false;
    try {
      const result: LoginResult | undefined =
        await this.kiotaClientService.anonymousClient.api.v2.accounts.login.post({
          email,
          password,
        });
      if (result?.accessToken) {
        isLogged = true;
        this.tokenStorageToken.setToken(result.accessToken);
        this.localStorageService.setValue(
          LOCAL_STORAGE_ENTRIES.storedUser,
          email
        );
        this.isLogged.next(isLogged);
        return isLogged;
      }
    } catch (error) {
      this.notificationService.showError('Invalid username or password.');
    }

    return isLogged;
  }

  logout() {
    // remove user from local storage to log user out
    localStorage.removeItem(LOCAL_STORAGE_ENTRIES.storedCustomer);
    localStorage.removeItem(LOCAL_STORAGE_ENTRIES.storedUser);
    this.tokenStorageToken.clearToken();

    // broadcasting to listeners
    this.isLogged.next(false);

    // redirects
    this.router.navigate([ROUTE_PATHS.login]);
  }
}
