import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { RestService } from '@core/services/http/rest.service';
import { environment } from '@environments/environment';
import { RegisterCustomerRequest } from '../models/requests/RegisterCustomerRequest';
import { UpdateCustomerRequest } from '../models/requests/UpdateCustomerRequest';
import { Observable, catchError } from 'rxjs';
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
    return this.get(this.controllerName).pipe(
      catchError((error) => {
        console.error('Error:', error);
        return [];
      })
    );
  }

  public registerCustomer(
    request: RegisterCustomerRequest
  ): Observable<ServiceResponse> {
    return this.post(this.controllerName, request).pipe(
      catchError((error) => {
        console.error('Error:', error);
        return [];
      })
    );
  }

  public updateCustomer(
    customerId: string,
    request: UpdateCustomerRequest
  ): Observable<ServiceResponse> {
    return this.put(
      this.controllerName + '/' + customerId, request)
    .pipe(
      catchError((error) => {
        console.error('Error:', error);
        return [];
      })
    );
  }

  public getCustomerStoredEvents(
    aggregateId: string
  ): Observable<ServiceResponse> {
    return this.get(this.controllerName + '/' + aggregateId + '/history').pipe(
      catchError((error) => {
        console.error('Error:', error);
        return [];
      })
    );
  }
}
