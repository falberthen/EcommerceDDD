export class RemoveQuoteItemRequest {
  quoteId: string;
  productId: string;

  constructor(quoteId: string, productId: string) {
    this.quoteId = quoteId;
    this.productId = productId;
  }
}
