import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { RestService } from 'src/app/core/services/http/rest.service';
import { CreateQuoteRequest } from 'src/app/core/models/requests/CreateQuoteRequest';
import { ChangeQuoteRequest } from 'src/app/core/models/requests/ChangeQuoteRequest';
@Injectable({
  providedIn: 'root'
})
export class QuoteService extends RestService {

  controllerName = 'quotes';

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    super(http, baseUrl);
  }

  public createQuote(request: CreateQuoteRequest): Promise<any>{
    return this.post(this.controllerName, request)
    .toPromise();
  }

  public changeQuote(request: ChangeQuoteRequest): Promise<any>{
    return this.put(this.controllerName, request)
    .toPromise();
  }

  public getQuoteDetails(quoteId: string, currency: string): Promise<any> {
    return this.get(this.controllerName + '/' + quoteId + '/details/' + currency)
    .toPromise();
  }

  public getCurrentQuote(customerId: string): Promise<any> {
    return this.get(this.controllerName + '/' + customerId + '/quote')
    .toPromise();
  }
}
