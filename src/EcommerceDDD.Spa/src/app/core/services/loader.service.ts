import { Injectable } from '@angular/core';
import { Observable, ReplaySubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LoaderService {
  private httpLoading$ = new ReplaySubject<boolean>(1);
  isLoading = false;

  constructor() { }

  httpProgress(): Observable<boolean> {
    return this.httpLoading$.asObservable();
  }

  setHttpProgressStatus(inprogess: boolean) {
    if(inprogess)
      this.isLoading = true;

    this.httpLoading$.next(inprogess);
  }
}

//https://imdurgeshpal.medium.com/show-loader-spinner-on-http-request-in-angular-using-interceptor-68f57a9557a4
