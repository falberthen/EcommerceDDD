import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RestService } from 'src/app/core/services/http/rest.service';
import { SaveCartRequest } from 'src/app/core/models/requests/SaveCartRequest';

@Injectable({
  providedIn: 'root'
})
export class CartService extends RestService {

  controllerName = 'carts';

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    super(http, baseUrl);
  }

  public saveCart(request: SaveCartRequest): Observable<any>{
    return this.post(this.controllerName, request);
  }

  public getCartDetails(customerId: string, currency: string): Observable<any>{
    return this.get(this.controllerName + '/' + customerId + '/details/' + currency);
  }

}
