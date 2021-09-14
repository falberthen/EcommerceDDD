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

  public getOrders(customerId: string): Promise<any>{
    return this.get(this.controllerName + '/' + customerId)
    .toPromise();
  }

  public getOrderDetails(customerId: string, orderId: string): Promise<any>{
    return this.get(this.controllerName + '/' + customerId + '/' + orderId + '/details')
    .toPromise();
  }

  public placeOrder(quoteId: string, request: PlaceOrderRequest): Promise<any>{
    return this.post(this.controllerName + '/' + quoteId, request)
    .toPromise();
  }
}
