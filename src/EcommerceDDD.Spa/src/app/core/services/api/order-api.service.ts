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

  placeOrder(orderId: string, quoteId: string) {
    return this.kiotaClientService.client
      .orderProcessing.api.v2.orders
      .byOrderId(orderId)
      .put({ quoteId });
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
