import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { Location } from '@angular/common';
import { environment } from '../../../../environments/environment';

export abstract class RestService {
  apiBaseUrl: string;

  httpHeaders = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' })
  }

  constructor(private http: HttpClient, baseUrl: string) {
    this.apiBaseUrl = environment.apiUrl;
  }

  protected get(relativeUrl: string, httpParams?: HttpParams, responseTypeInput?: any): Observable<any> {
    const fullUrl = Location.joinWithSlash(this.apiBaseUrl, relativeUrl);
    return this.http.get(fullUrl, { params: httpParams, responseType: responseTypeInput }).pipe(
      map((response: any) => {
        return response;
      })
    );
  }

  protected delete(relativeUrl: string, body?: any): Observable<any> {
    const fullUrl = Location.joinWithSlash(this.apiBaseUrl, relativeUrl);
    return this.http.delete(fullUrl, body).pipe(
      map((response: any) => {
        return response;
      })
    );
  }

  protected post(relativeUrl: string, body: any, httpParams?: HttpParams): Observable<any> {
    const fullUrl = Location.joinWithSlash(this.apiBaseUrl, relativeUrl);
    return this.http.post(fullUrl, body, this.httpHeaders).pipe(
      map((response: any) => {
        return response;
      })
    );
  }

  protected put(relativeUrl: string, body: any, httpParams?: HttpParams): Observable<any> {
    const fullUrl = Location.joinWithSlash(this.apiBaseUrl, relativeUrl);
    return this.http.put(fullUrl, body, this.httpHeaders).pipe(
      map((response: any) => {
        return response;
      })
    );
  }

  protected patch(relativeUrl: string, body: any, httpParams?: HttpParams): Observable<any> {
    const fullUrl = Location.joinWithSlash(this.apiBaseUrl, relativeUrl);
    return this.http.patch(fullUrl, body, this.httpHeaders).pipe(
      map((response: any) => {
        return response;
      })
    );
  }
}
