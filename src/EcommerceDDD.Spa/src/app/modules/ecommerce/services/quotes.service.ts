import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { RestService } from 'src/app/core/services/http/rest.service';
import { environment } from 'src/environments/environment';
import { OpenQuoteRequest } from '../models/requests/OpenQuoteRequest';
import { AddQuoteItemRequest } from '../models/requests/AddQuoteItemRequest';
import { RemoveQuoteItemRequest } from '../models/requests/RemoveQuoteItemRequest';
import { Observable } from 'rxjs';
import { ServiceResponse } from './ServiceResponse';

@Injectable({
  providedIn: 'root'
})
export class QuotesService extends RestService {

  controllerName = 'quotes';

  constructor(http: HttpClient) {
    super(http, environment.apiUrl);
  }

  public openQuote(request: OpenQuoteRequest): Observable<ServiceResponse>{
    return this.post(this.controllerName, request);
  }

  public addQuoteItem(quoteId: string, request: AddQuoteItemRequest): Observable<ServiceResponse>{
    return this.put(this.controllerName + '/' + quoteId + '/items', request);
  }

  public removeQuoteItem(request: RemoveQuoteItemRequest): Observable<ServiceResponse>{
    return this.delete(this.controllerName + '/' + request.quoteId + '/items/' + request.productId)
  }

  public cancelQuote(quoteId: string): Observable<ServiceResponse>{
    return this.delete(this.controllerName + '/' + quoteId);
  }

  public placeOrder(quoteId: string, currencyCode: string): Observable<ServiceResponse>{
    return this.put(this.controllerName + '/' + quoteId + '/placeorder/' + currencyCode);
  }

  public getOpenQuote(customerId: string, currency: string): Observable<ServiceResponse>{
    return this.get(this.controllerName + '/' + customerId + '/quote/' + currency);
  }

  public getQuoteStoredEvents(aggregateId: string): Observable<ServiceResponse>{
    return this.get('quotes/' + aggregateId + '/history');
  }
}
