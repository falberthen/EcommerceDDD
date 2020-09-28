export class PlaceOrderRequest {
  customerId: string;
  currency: string;

  constructor(customerId: string, currency: string) {
    this.customerId = customerId;
    this.currency = currency;
  }
}
