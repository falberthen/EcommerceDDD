import { Injectable, Inject } from '@angular/core';
import { CustomerHistoryData } from 'app/core/models/CustomerHistoryData';
import { RestService } from 'app/core/services/http/rest.service';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class HistoryViewerService extends RestService {

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    super(http, baseUrl);
  }

  public getCustomerEventHistory(aggegateId: string): Observable<CustomerHistoryData[]>{
    return this.get("customer/" + aggegateId + "/eventhistory/");
  }

}
