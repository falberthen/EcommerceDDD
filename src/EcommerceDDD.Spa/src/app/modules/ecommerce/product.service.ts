import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RestService } from 'src/app/core/services/http/rest.service';
import { Product } from 'src/app/core/models/Product';

@Injectable({
  providedIn: 'root'
})
export class ProductService extends RestService {

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    super(http, baseUrl);
  }

  public getProducts(currency: string): Promise<Product[]>{
    return this.get('products/' + currency)
    .toPromise();
  }
}
