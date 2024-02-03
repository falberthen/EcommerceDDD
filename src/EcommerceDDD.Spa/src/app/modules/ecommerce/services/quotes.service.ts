import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { RestService } from '@core/services/http/rest.service';
import { environment } from '@environments/environment';
import { OpenQuoteRequest } from '../models/requests/OpenQuoteRequest';
import { AddQuoteItemRequest } from '../models/requests/AddQuoteItemRequest';
import { RemoveQuoteItemRequest } from '../models/requests/RemoveQuoteItemRequest';
import { ServiceResponse } from './ServiceResponse';

@Injectable({
  providedIn: 'root',
})
export class QuotesService extends RestService {
  controllerName = 'quotes';

  constructor(http: HttpClient) {
    super(http, environment.apiUrl);
  }

  public openQuote(request: OpenQuoteRequest): Observable<ServiceResponse> {
    return this.post(this.controllerName, request).pipe(
      catchError((error) => {
        console.error('Error:', error);
        return [];
      })
    );
  }

  public addQuoteItem(
    quoteId: string,
    request: AddQuoteItemRequest
  ): Observable<ServiceResponse> {
    return this.put(this.controllerName + '/' + quoteId + '/items', request).pipe(
      catchError((error) => {
        console.error('Error:', error);
        return [];
      })
    );
  }

  public removeQuoteItem(
    request: RemoveQuoteItemRequest
  ): Observable<ServiceResponse> {
    return this.delete(
      this.controllerName +
        '/' +
        request.quoteId +
        '/items/' +
        request.productId
    ).pipe(
      catchError((error) => {
        console.error('Error:', error);
        return [];
      })
    );
  }

  public cancelQuote(quoteId: string): Observable<ServiceResponse> {
    return this.delete(this.controllerName + '/' + quoteId).pipe(
      catchError((error) => {
        console.error('Error:', error);
        return [];
      })
    );
  }

  public getOpenQuote(
    customerId: string
  ): Observable<ServiceResponse> {
    return this.get(
      this.controllerName + '/' + customerId + '/quote'
    ).pipe(
      catchError((error) => {
        console.error('Error:', error);
        return [];
      })
    );
  }

  public getQuoteStoredEvents(
    aggregateId: string
  ): Observable<ServiceResponse> {
    return this.get('quotes/' + aggregateId + '/history').pipe(
      catchError((error) => {
        console.error('Error:', error);
        return [];
      })
    );
  }
}
