import { Injectable, Inject } from '@angular/core';
import { Subject } from 'rxjs';
import { RestService } from './http/rest.service';
import { HttpClient } from '@angular/common/http';
import { LocalStorageService } from './local-storage.service';
import { Customer } from '../models/Customer';
import { map } from 'rxjs/operators';
import { TokenStorageService } from '../token-storage.service';
import { Router } from '@angular/router';
import { NotificationService } from './notification.service';
import { appConstants } from '../constants/appConstants';

@Injectable({
  providedIn: 'root'
})
export class AuthService extends RestService {

    private isLogged = new Subject<boolean>();

    // Observable string streams
    isLoggedAnnounced$ = this.isLogged.asObservable();

    constructor (http: HttpClient,
      private tokenStorageToken: TokenStorageService,
      private localStorageService: LocalStorageService,
      private router: Router,
      private notificationService: NotificationService,
      @Inject('BASE_URL') baseUrl: string) {
        super(http, baseUrl);
    }

    public get currentCustomer(): Customer | null {
      const storedCustomerData = this.localStorageService
        .getValueByKey(appConstants.storedCustomer);

      if(storedCustomerData) {
        var storedCustomer = JSON.parse(storedCustomerData);
        return storedCustomer as Customer;
      }

      return null;
    }

    login(email: string, password: string) {
      return this.post("customers/login", { email, password })
        .pipe(map(result => {
          const data = result.data;

          // login successful if there's a jwt token in the response
          if (data.token) {
            this.tokenStorageToken.saveToken(data.token);

            // store user details and jwt token in local storage to keep user logged in between page refreshes
            this.localStorageService.setValue(appConstants.storedCustomer, JSON.stringify(data));
            this.isLogged.next(true);
          }
          else {
            this.notificationService.showError(result.data.validationResult.errors[0].errorMessage);
          }

        return result;
      }));
    }

    logout() {
      // remove user from local storage to log user out
      localStorage.removeItem(appConstants.storedCustomer);
      this.tokenStorageToken.clearToken();

      // broadcasting to listeners
      this.isLogged.next(false);

      // redirects
      this.router.navigate(["/login"]);
    }
}

