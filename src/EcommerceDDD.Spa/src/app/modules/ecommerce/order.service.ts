import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RestService } from 'src/app/core/services/http/rest.service';
import { PlaceOrderRequest } from 'src/app/core/models/requests/PlaceOrderRequest';

@Injectable({
  providedIn: 'root'
})
export class OrderService extends RestService {

  controllerName = 'orders';

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    super(http, baseUrl);
  }

  public getOrders(customerId: string): Observable<any>{
    return this.get(this.controllerName + '/' + customerId);
  }

  public getOrderDetails(customerId: string, orderId: string): Observable<any>{
    return this.get(this.controllerName + '/' + customerId + '/' + orderId + '/details');
  }

  public placeOrder(cartId: string, request: PlaceOrderRequest): Observable<any>{
    return this.post(this.controllerName + '/' + cartId, request);
  }
}
