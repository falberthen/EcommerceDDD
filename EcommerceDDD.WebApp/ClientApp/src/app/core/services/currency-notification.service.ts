import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CurrencyNotificationService {

  private messageSource = new BehaviorSubject('');
  public currentCurrency = this.messageSource.asObservable();

  changeCurrency(currencyCode: string) {
    this.messageSource.next(currencyCode)
  }
}
