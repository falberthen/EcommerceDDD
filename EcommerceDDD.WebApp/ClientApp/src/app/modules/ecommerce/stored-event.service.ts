import { Injectable, Inject } from '@angular/core';
import { CustomerStoredEventData } from 'app/core/models/CustomerStoredEventData';
import { RestService } from 'app/core/services/http/rest.service';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { StoredEventData } from 'app/core/models/StoredEventData';
@Injectable({
  providedIn: 'root'
})
export class StoredEventService extends RestService {

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    super(http, baseUrl);
  }

  public getCustomerStoredEvents(aggregateId: string): Observable<CustomerStoredEventData[]>{
    return this.get('customers/' + aggregateId + '/events');
  }

  public getOrderStoredEvents(aggregateId: string): Observable<StoredEventData[]>{
    return this.get('orders/' + aggregateId + '/events');
  }
}
