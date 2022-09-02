import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { RestService } from 'src/app/core/services/http/rest.service';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class OrderService extends RestService {

  controllerName = 'orders';

  constructor(http: HttpClient) {
    super(http, environment.apiUrl);
  }

  public getOrders(customerId: string): Promise<any>{
    return this.get(this.controllerName + '/' + customerId)
    .toPromise();
  }

  public getOrderDetails(customerId: string, orderId: string): Promise<any>{
    return this.get(this.controllerName + '/' + customerId + '/' + orderId + '/details')
    .toPromise();
  }
}
