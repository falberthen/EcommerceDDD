import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { RestService } from 'src/app/core/services/http/rest.service';
import { Product } from 'src/app/core/models/Product';
import { environment } from 'src/environments/environment';
import { GetProductsRequest } from 'src/app/core/models/requests/GetProductsRequest';

@Injectable({
  providedIn: 'root'
})
export class ProductService extends RestService {

  controllerName = 'products';

  constructor(http: HttpClient) {
    super(http, environment.apiUrl);
  }

  public getProducts(request: GetProductsRequest): Promise<Product[]>{
    return this.post(this.controllerName, request)
    .toPromise();
  }
}
