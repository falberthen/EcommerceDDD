import { Injectable, Inject } from '@angular/core';
import { RestService } from 'app/core/services/http/rest.service';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Product } from 'app/core/models/Product';

@Injectable({
  providedIn: 'root'
})
export class ProductService extends RestService {

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    super(http, baseUrl);
  }

  public getProducts(currency: string): Observable<Product[]>{
    return this.get('products/' + currency);
  }

}
