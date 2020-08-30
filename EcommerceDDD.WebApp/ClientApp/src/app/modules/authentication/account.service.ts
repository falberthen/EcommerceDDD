
import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { RestService } from 'app/core/services/http/rest.service';
import { CustomerRegistration } from 'app/core/models/CustomerRegistration';

@Injectable({
  providedIn: 'root'
})
export class AccountService extends RestService {

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    super(http, baseUrl);
  }

  public registerAccount(customerRegistration: CustomerRegistration) {
    return this.post("customers/register", customerRegistration);
  }

}
