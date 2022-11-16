import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { RestService } from 'src/app/core/services/http/rest.service';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { ServiceResponse } from './ServiceResponse';

@Injectable({
  providedIn: 'root'
})
export class OrdersService extends RestService {

  controllerName = 'orders';

  constructor(http: HttpClient) {
    super(http, environment.apiUrl);
  }

  public getOrders(customerId: string): Observable<ServiceResponse>{
    return this.get(this.controllerName + '/' + customerId);
  }

  public getOrderDetails(customerId: string, orderId: string): Observable<ServiceResponse>{
    return this.get(this.controllerName + '/' + customerId + '/' + orderId + '/details')
  }

  public getOrderStoredEvents(aggregateId: string): Observable<ServiceResponse>{
    return this.get('orders/' + aggregateId + '/history');
  }

  public placeOrderFromQuote(quoteId: string): Observable<ServiceResponse>{
    return this.post(this.controllerName + '/' + quoteId);
  }
}
