import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { RestService } from 'src/app/core/services/http/rest.service';
import { environment } from 'src/environments/environment';
import { RegisterCustomerRequest } from '../models/requests/RegisterCustomerRequest';
import { UpdateCustomerRequest } from '../models/requests/UpdateCustomerRequest';
import { Observable } from 'rxjs';
import { ServiceResponse } from './ServiceResponse';

@Injectable({
  providedIn: 'root',
})
export class CustomersService extends RestService {
  controllerName = 'customers';

  constructor(http: HttpClient) {
    super(http, environment.apiUrl);
  }

  public loadCustomerDetails(): Observable<ServiceResponse> {
    return this.get(this.controllerName);
  }

  public getCustomerStoredEvents(
    aggregateId: string
  ): Observable<ServiceResponse> {
    return this.get(this.controllerName + '/' + aggregateId + '/history');
  }

  public registerCustomer(
    request: RegisterCustomerRequest
  ): Observable<ServiceResponse> {
    return this.post(this.controllerName, request);
  }

  public updateCustomer(
    customerId: string,
    request: UpdateCustomerRequest
  ): Observable<ServiceResponse> {
    return this.put(this.controllerName + '/' + customerId, request);
  }
}
