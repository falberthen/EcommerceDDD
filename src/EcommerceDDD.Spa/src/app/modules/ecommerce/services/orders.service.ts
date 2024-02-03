import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { RestService } from '@core/services/http/rest.service';
import { environment } from '@environments/environment';
import { ServiceResponse } from './ServiceResponse';

@Injectable({
  providedIn: 'root',
})
export class OrdersService extends RestService {
  controllerName = 'orders';

  constructor(http: HttpClient) {
    super(http, environment.apiUrl);
  }

  public getOrders(customerId: string): Observable<ServiceResponse> {
    return this.get(this.controllerName + '/' + customerId).pipe(
      catchError((error) => {
        console.error('Error:', error);
        return [];
      })
    );
  }

  public getOrderStoredEvents(
    aggregateId: string
  ): Observable<ServiceResponse> {
    return this.get('orders/' + aggregateId + '/history').pipe(
      catchError((error) => {
        console.error('Error:', error);
        return [];
      })
    );
  }

  public placeOrder(customerId: string, quoteId: string): Observable<ServiceResponse> {
    return this.post(this.controllerName + '/' + customerId  + '/' + quoteId).pipe(
      catchError((error) => {
        console.error('Error:', error);
        return [];
      })
    );
  }
}
