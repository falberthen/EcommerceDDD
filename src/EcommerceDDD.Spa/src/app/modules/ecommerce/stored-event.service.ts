import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RestService } from 'src/app/core/services/http/rest.service';
import { CustomerStoredEventData } from 'src/app/core/models/CustomerStoredEventData';
import { StoredEventData } from 'src/app/core/models/StoredEventData';
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
