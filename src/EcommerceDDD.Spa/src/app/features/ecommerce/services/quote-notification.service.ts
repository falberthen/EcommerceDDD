import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class QuoteNotificationService {
  private quoteItemsCountSource = new BehaviorSubject<number>(0);
  currentQuoteItemsCount = this.quoteItemsCountSource.asObservable();

  changeQuoteItemsCount(count: number) {
    this.quoteItemsCountSource.next(count);
  }
}
