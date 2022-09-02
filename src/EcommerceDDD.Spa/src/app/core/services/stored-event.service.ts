import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RestService } from 'src/app/core/services/http/rest.service';
import { CustomerStoredEventData } from 'src/app/core/models/CustomerStoredEventData';
import { environment } from 'src/environments/environment';
import { QuoteStoredEventData } from 'src/app/core/models/QuoteStoredEventData';
import { OrderStoredEventData } from 'src/app/core/models/OrderStoredEventData';
@Injectable({
  providedIn: 'root'
})
export class StoredEventService extends RestService {

  constructor(http: HttpClient) {
    super(http, environment.apiUrl);
  }

  public getCustomerStoredEvents(aggregateId: string): Observable<CustomerStoredEventData[]>{
    return this.get('customers/' + aggregateId + '/history');
  }

  public getQuoteStoredEvents(aggregateId: string): Observable<QuoteStoredEventData[]>{
    return this.get('quotes/' + aggregateId + '/history');
  }

  public getOrderStoredEvents(aggregateId: string): Observable<OrderStoredEventData[]>{
    return this.get('orders/' + aggregateId + '/history');
  }
}
