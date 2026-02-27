import { Injectable, inject } from '@angular/core';
import { KiotaClientService } from '../kiota-client.service';

@Injectable({
  providedIn: 'root',
})
export class OrderApiService {
  private kiotaClientService = inject(KiotaClientService);

  getOrders() {
    return this.kiotaClientService.client
      .orderProcessing.api.v2.orders.get();
  }

  getOrderHistory(orderId: string) {
    return this.kiotaClientService.client
      .orderProcessing.api.v2.orders
      .byOrderId(orderId)
      .history.get();
  }

  placeOrder(quoteId: string) {
    return this.kiotaClientService.client
      .orderProcessing.api.v2.orders.quote
      .byQuoteId(quoteId)
      .post();
  }

  confirmDelivery(orderId: string) {
    return this.kiotaClientService.client
      .orderProcessing.api.v2.orders
      .byOrderId(orderId)
      .confirmDelivery.post();
  }

  handleError(error: unknown): void {
    this.kiotaClientService.handleError(error);
  }
}
