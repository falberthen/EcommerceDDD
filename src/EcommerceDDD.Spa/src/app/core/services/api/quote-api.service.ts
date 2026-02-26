import { Injectable, inject } from '@angular/core';
import { KiotaClientService } from '../kiota-client.service';
import { QuoteViewModel } from 'src/app/clients/models';

@Injectable({
  providedIn: 'root',
})
export class QuoteApiService {
  private kiotaClientService = inject(KiotaClientService);

  getOpenQuote(): Promise<QuoteViewModel | null | undefined> {
    return this.kiotaClientService.client
      .quoteManagement.api.v2.quotes.get();
  }

  createQuote(currencyCode: string) {
    return this.kiotaClientService.client
      .quoteManagement.api.v2.quotes.post({ currencyCode });
  }

  addItem(quoteId: string, productId: string, quantity: number) {
    return this.kiotaClientService.client
      .quoteManagement.api.v2.quotes
      .byQuoteId(quoteId)
      .items.put({ productId, quantity });
  }

  removeItem(quoteId: string, productId: string) {
    return this.kiotaClientService.client
      .quoteManagement.api.v2.quotes
      .byQuoteId(quoteId)
      .items.byProductId(productId)
      .delete();
  }

  cancelQuote(quoteId: string) {
    return this.kiotaClientService.client
      .quoteManagement.api.v2.quotes
      .byQuoteId(quoteId)
      .delete();
  }

  getHistory(quoteId: string) {
    return this.kiotaClientService.client
      .quoteManagement.api.v2.quotes
      .byQuoteId(quoteId)
      .history.get();
  }

  handleError(error: unknown): void {
    this.kiotaClientService.handleError(error);
  }
}
