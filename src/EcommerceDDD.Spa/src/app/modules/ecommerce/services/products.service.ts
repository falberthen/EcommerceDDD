import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { RestService } from '@core/services/http/rest.service';
import { environment } from '@environments/environment';
import { GetProductsRequest } from '../models/requests/GetProductsRequest';
import { Observable, catchError } from 'rxjs';
import { ServiceResponse } from './ServiceResponse';

@Injectable({
  providedIn: 'root',
})
export class ProductsService extends RestService {
  controllerName = 'products';

  constructor(http: HttpClient) {
    super(http, environment.apiUrl);
  }

  public getProducts(request: GetProductsRequest): Observable<ServiceResponse> {
    return this.post(this.controllerName, request).pipe(
      catchError((error) => {
        console.error('Error:', error);
        return [];
      })
    );
  }
}
