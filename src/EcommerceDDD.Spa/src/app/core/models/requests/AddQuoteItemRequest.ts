export class AddQuoteItemRequest {
  productId: string;
  quantity: number;
  currencyCode: string;

  constructor(productId: string, quantity: number, currencyCode: string) {
    this.productId = productId;
    this.quantity = quantity;
    this.currencyCode = currencyCode;
  }
}
