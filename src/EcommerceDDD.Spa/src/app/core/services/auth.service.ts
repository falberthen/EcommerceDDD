import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { RestService } from './http/rest.service';
import { HttpClient } from '@angular/common/http';
import { LocalStorageService } from './local-storage.service';
import { map } from 'rxjs/operators';
import { TokenStorageService } from './token-storage.service';
import { Router } from '@angular/router';
import { NotificationService } from './notification.service';
import { appConstants } from '../../modules/ecommerce/constants/appConstants';
import { environment } from 'src/environments/environment';
import { Customer } from 'src/app/modules/ecommerce/models/Customer';

@Injectable({
  providedIn: 'root',
})
export class AuthService extends RestService {
  private isLogged = new Subject<boolean>();

  // Observable string streams
  isLoggedAnnounced$ = this.isLogged.asObservable();

  constructor(
    http: HttpClient,
    private tokenStorageToken: TokenStorageService,
    private localStorageService: LocalStorageService,
    private router: Router,
    private notificationService: NotificationService
  ) {
    super(http, environment.authUrl);
  }

  public get currentUser(): string | null {
    const storedUser = this.localStorageService.getValueByKey(
      appConstants.storedUser
    );
    return storedUser;
  }

  public get currentCustomer(): Customer | null {
    const storedCustomerData = this.localStorageService.getValueByKey(
      appConstants.storedCustomer
    );

    if (storedCustomerData) {
      var storedCustomer = JSON.parse(storedCustomerData);
      return storedCustomer as Customer;
    }

    return null;
  }

  login(email: string, password: string) {
    return this.post('accounts/login', { email, password }).pipe(
      map((result) => {
        // login successful if there's a jwt token in the response
        if (!result.error) {
          this.tokenStorageToken.setToken(result.accessToken);
          // store user details and jwt token in local storage to keep user logged in between page refreshes
          this.localStorageService.setValue(appConstants.storedUser, email);
          this.isLogged.next(true);
        } else {
          this.notificationService.showError(result.errorDescription);
        }

        return result;
      })
    );
  }

  logout() {
    // remove user from local storage to log user out
    localStorage.removeItem(appConstants.storedCustomer);
    localStorage.removeItem(appConstants.storedUser);
    this.tokenStorageToken.clearToken();

    // broadcasting to listeners
    this.isLogged.next(false);

    // redirects
    this.router.navigate(['/login']);
  }
}
