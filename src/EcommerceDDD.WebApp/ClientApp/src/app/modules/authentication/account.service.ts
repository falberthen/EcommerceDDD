
import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { RestService } from 'app/core/services/http/rest.service';
import { RegisterCustomerRequest } from 'app/core/models/requests/RegisterCustomerRequest';
import { UpdateCustomerRequest } from 'app/core/models/requests/UpdateCustomerRequest';

@Injectable({
  providedIn: 'root'
})
export class AccountService extends RestService {

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    super(http, baseUrl);
  }

  public registerAccount(request: RegisterCustomerRequest) {
    return this.post("customers/register", request);
  }

  public updateCustomer(customerId: string, request: UpdateCustomerRequest) {
    return this.put("customers/" + customerId, request);
  }

}
