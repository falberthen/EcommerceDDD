
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { RestService } from 'src/app/core/services/http/rest.service';
import { RegisterCustomerRequest } from 'src/app/core/models/requests/RegisterCustomerRequest';
import { UpdateCustomerRequest } from 'src/app/core/models/requests/UpdateCustomerRequest';
import { environment } from 'src/environments/environment';
@Injectable({
  providedIn: 'root'
})
export class AccountService extends RestService {

  constructor(http: HttpClient) {
    super(http, environment.apiUrl);
  }

  public loadCustomerDetails() {
    return this.get("customers")
    .toPromise();
  }

  public registerAccount(request: RegisterCustomerRequest) {
    return this.post("customers", request)
    .toPromise();
  }

  public updateCustomer(customerId: string, request: UpdateCustomerRequest) {
    return this.put("customers/" + customerId, request)
    .toPromise();
  }

}
