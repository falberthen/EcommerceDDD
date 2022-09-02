import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { RestService } from 'src/app/core/services/http/rest.service';
import { OpenQuoteRequest } from 'src/app/core/models/requests/OpenQuoteRequest';
import { AddQuoteItemRequest } from 'src/app/core/models/requests/AddQuoteItemRequest';
import { environment } from 'src/environments/environment';
import { RemoveQuoteItemRequest } from 'src/app/core/models/requests/RemoveQuoteItemRequest';
@Injectable({
  providedIn: 'root'
})
export class QuoteService extends RestService {

  controllerName = 'quotes';

  constructor(http: HttpClient) {
    super(http, environment.apiUrl);
  }

  public openQuote(request: OpenQuoteRequest): Promise<any>{
    return this.post(this.controllerName, request)
    .toPromise();
  }

  public addQuoteItem(quoteId: string, request: AddQuoteItemRequest): Promise<any>{
    return this.put(this.controllerName + '/' + quoteId + '/items', request)
    .toPromise();
  }

  public removeQuoteItem(request: RemoveQuoteItemRequest): Promise<any>{
    return this.delete(this.controllerName + '/' + request.quoteId + '/items/' + request.productId)
    .toPromise();
  }

  public cancelQuote(quoteId: string): Promise<any>{
    return this.delete(this.controllerName + '/' + quoteId)
    .toPromise();
  }

  public placeOrder(quoteId: string, currencyCode: string): Promise<any>{
    return this.put(this.controllerName + '/' + quoteId + '/placeorder/' + currencyCode)
    .toPromise();
  }

  public getOpenQuote(customerId: string, currency: string): Promise<any> {
    return this.get(this.controllerName + '/' + customerId + '/quote/' + currency)
    .toPromise();
  }
}
